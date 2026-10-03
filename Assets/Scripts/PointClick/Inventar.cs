using StarterAssets;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace PointClick
{
    public class Inventar : MonoBehaviour
    {
        [Serializable]
        class InventarGegenstand
        {
            public string gegenstandsname;
            public int anzahl;
            public InventarGegenstand(string gegenstandsname, int anzahl)
            {
                this.gegenstandsname = gegenstandsname;
                this.anzahl = anzahl;
            }
        }
        public const int ANZAHL_SLOTS = 10;
        public const float AUTOSAVE_INTERVAL = 2;

        private string savefileItems;
        // Sicherstellen, dass es eine Welt Datei pro Szene gibt
        private string savefileWorld;

        public static Inventar Instance { get; private set; }
        private void Awake()
        {
            Instance = this;
            LoadResources();
        }

        [Header("Inventar Slot Button Prefab")]
        [SerializeField] private GameObject inventarButton;

        [Header("UI Panel des Inventars")]
        [SerializeField] private GameObject inventarPanel;
        [Header("Parent Objekt der Inventar Slot Buttons")]
        [SerializeField] private Transform slotsParent;
        [Header("Slot des gehaltenen Gegenstandes")]
        [SerializeField] private TMPro.TextMeshProUGUI gehaltenerGegenstandText;
        [Header("Parent Transform des gehaltenen Gegenstands")]
        [SerializeField] private Transform rechteHand;

        [Header("Inventar Öffnen/Schließen Action")]
        public InputActionReference inventarAction;

        [Header("Die Starter Assets Inputs zur Charaktersteuerung")]
        public StarterAssetsInputs inputs;

        private static readonly List<Gegenstand> alleGegenstaende = new();
        private readonly InventarGegenstand[] gegenstaende = new InventarGegenstand[ANZAHL_SLOTS];
        private readonly InventarButton[] slots = new InventarButton[ANZAHL_SLOTS];
        private string savefile;
        private WorldData worldData;
        private GameObject player;
        public GameObject Player => player;

        private static bool zeigeInventar;
        public static bool ZeigeInventar
        {
            get => zeigeInventar;
            set
            {
                zeigeInventar = value;
                Time.timeScale = zeigeInventar ? 0 : 1;
                if (Instance != null)
                {
                    if (Instance.inventarPanel != null)
                        Instance.inventarPanel.SetActive(zeigeInventar);
                    if (Instance.inputs != null)
                    {
                        Instance.inputs.cursorInputForLook = !zeigeInventar;
                    }
                    Cursor.lockState = zeigeInventar ? CursorLockMode.None : CursorLockMode.Locked;
                }
            }
        }

        private static int gehaltenerGegenstandIndex;
        static int GehaltenerGegenstandIndex
        {
            get => gehaltenerGegenstandIndex;
            set
            {
                gehaltenerGegenstandIndex = value;
                if (Instance != null)
                    Instance?.UpdateHand();
            }
        }
        public static Gegenstand GetGegenstandInHand()
        {
            if (Instance == null) return null;
            if (GehaltenerGegenstandIndex < 0 || GehaltenerGegenstandIndex >= ANZAHL_SLOTS) return null;
            InventarGegenstand ggst = Instance.gegenstaende[gehaltenerGegenstandIndex];
            if (ggst == null) return null;
            return FindeGegenstand(ggst.gegenstandsname);
        }
        public static void SetGegenstandInHand(string gegenstandsName)
        {
            if (Instance == null) return;
            if (string.IsNullOrEmpty(gegenstandsName))
                GehaltenerGegenstandIndex = -1;
            for (int i = 0; i < Instance.gegenstaende.Length; i++)
            {
                if (Instance.gegenstaende[i] != null && Instance.gegenstaende[i].gegenstandsname.Equals(gegenstandsName))
                {
                    GehaltenerGegenstandIndex = i;
                    return;
                }
            }
            GehaltenerGegenstandIndex = -1;
        }

        private Coroutine autoSaveRoutine;

        void Start()
        {
            savefileItems = "gegenstaende.bin";
            // Sicherstellen, dass es eine Welt Datei pro Szene gibt
            savefileWorld = "welt" + SceneManager.GetActiveScene().buildIndex + ".bin";
            BuildInventorySlots();
            Init();
            // Inventar am Anfang nicht zeigen
            ZeigeInventar = false;
        }

        void LoadResources()
        {
            alleGegenstaende.Clear();
            alleGegenstaende.AddRange(Resources.LoadAll<Gegenstand>("Items"));
        }

        void BuildInventorySlots()
        {
            for (int i = 0; i < gegenstaende.Length; i++)
            {
                var slot = Instantiate(inventarButton, slotsParent, false).GetComponent<InventarButton>();
                if (i == 0)
                    slot.AddComponent<SelectOnEnable>();
                slots[i] = slot;
            }
        }

        void Init()
        {
            // Spieler finden
            player = GameObject.FindGameObjectWithTag("Player");
            if (player == null)
                Debug.LogWarning("Inventar.Init(): Player not found!");

            savefile = Path.Combine(Application.persistentDataPath, savefileItems);
            // Die Liste laden bzw. neu erzeugen
            ListeLaden();
            // Inventar aktualisieren
            UpdateSlots();
            GehaltenerGegenstandIndex = -1;

            // Welt Zustand laden
            worldData = new WorldData(Path.Combine(Application.persistentDataPath, savefileWorld));
            worldData.Load();

            // Spielerposition setzen
            if (player != null)
            {
                player.transform.position = new Vector3(worldData.playerX, worldData.playerY, worldData.playerZ);
                Debug.Log("Initiale Spielerposition: " + player.transform.position);
            }
        }

        private void OnEnable()
        {
            inventarAction.action?.Enable();
            Cursor.lockState = CursorLockMode.Locked;
            if (autoSaveRoutine != null)
                StopCoroutine(autoSaveRoutine);
            autoSaveRoutine = StartCoroutine(AutoSaveLoop());
        }

        private void OnDisable()
        {
            inventarAction.action?.Disable();
            Cursor.lockState = CursorLockMode.None;
            if (autoSaveRoutine != null)
                StopCoroutine(autoSaveRoutine);
        }

        void Update()
        {
            if (inventarAction.action.WasPressedThisFrame())
                ZeigeInventar = !ZeigeInventar;
        }

        IEnumerator AutoSaveLoop()
        {
            while (true)
            {
                yield return new WaitForSeconds(AUTOSAVE_INTERVAL);
                SaveAllData();
            }
        }

        void UpdateSlots()
        {
            for (int i = 0; i < gegenstaende.Length; i++)
            {
                if (gegenstaende[i] != null)
                    slots[i].Ggst = FindeGegenstand(gegenstaende[i].gegenstandsname);
                else slots[i].Ggst = null;
            }
            UpdateHand();
        }

        void UpdateHand()
        {
            Gegenstand gehaltenerGegenstand = GetGegenstandInHand();
            int anzahl = 0;
            if (gehaltenerGegenstand != null)
                anzahl = gegenstaende[gehaltenerGegenstandIndex].anzahl;

            if (gehaltenerGegenstandText != null)
            {
                if (gehaltenerGegenstand != null)
                    if (anzahl > 1)
                        gehaltenerGegenstandText.text = "In der Hand: " + gehaltenerGegenstand + " x " + anzahl;
                    else
                        gehaltenerGegenstandText.text = "In der Hand: " + gehaltenerGegenstand;
                else
                    gehaltenerGegenstandText.text = "Kein Gegenstand in der Hand";
            }
            if (rechteHand != null)
            {
                foreach (Transform child in rechteHand) Destroy(child.gameObject);
                if (gehaltenerGegenstand != null)
                {
                    var obj = Instantiate(gehaltenerGegenstand.prefab, rechteHand);
                    obj.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
                    // Bei Buchstaben Objekten Text vordefinieren
                    var textSetter = obj.GetComponent<BuchstabenTextSetter>();
                    if (textSetter != null) textSetter.Text = gehaltenerGegenstand.bedingung;
                }
            }
        }

        void ListeLaden()
        {
            bool ok = false;
            if (File.Exists(savefile))
            {
                // Eine neue instanz von FileStream erzeugen
                // Die Datei wird zum lesen geöffnet
                FileStream meinFileStream = new FileStream(savefile, FileMode.Open, FileAccess.Read);
                // Eine instanz von BinaryFormatter erzeugen
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                // Die Daten deserialisieren und ablegen
                InventarGegenstand[] geladeneGegenstaende;
                try
                {
                    geladeneGegenstaende = binaryFormatter.Deserialize(meinFileStream) as InventarGegenstand[];
                    for (int i = 0; i < ANZAHL_SLOTS; i++)
                        if (i < geladeneGegenstaende.Length)
                        {
                            gegenstaende[i] = geladeneGegenstaende[i];
                        }
                        else
                            gegenstaende[i] = null;
                    Debug.Log("Gegenstände geladen aus " + savefile);
                    ok = true;
                }
                catch (Exception ex) { Debug.LogWarning("Fehler beim Laden: " + ex); }

                meinFileStream.Close();
            }

            if (!ok)
            {
                // sonst 10 leere Einträge erzeugen
                Debug.Log("Kein savefile vorhanden, initialisiere Gegenstände");
                for (int i = 0; i < ANZAHL_SLOTS; i++)
                    gegenstaende[i] = null;
            }
        }

        void ListeSpeichern()
        {
            // Eine neue Instanz von FileStream erzeugen
            FileStream meinFileStream = new FileStream(savefile, FileMode.Create);
            // Eine Instanz von BinaryFormatter erzeugen
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            // Die Daten speichern. Dazu wird einfach die Liste serialisiert
            binaryFormatter.Serialize(meinFileStream, gegenstaende);
            meinFileStream.Close();
            Debug.Log("Gegenstände gespeichert unter " + savefile);
        }

        void WeltSpeichern()
        {
            worldData.Save();
        }

        public void SaveAllData()
        {
            WeltSpeichern();
            ListeSpeichern();
        }

        public void DeleteData()
        {
            try
            {
                File.Delete(savefile);
                Debug.Log("Inventar Datei gelöscht");
                worldData.Delete();
                // Szene neu laden
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
            catch (Exception) { Debug.LogError("Kann Speicherstand nicht löschen. Datei nicht vorhanden?"); }
        }

        public static Gegenstand Get(int index)
        {
            if (Instance == null)
            {
                PopupManager.ShowError("Inventar nicht initialisiert!");
                return null;
            }
            if (index < 0 || index >= ANZAHL_SLOTS)
            {
                return null;
            }
            return FindeGegenstand(Instance.gegenstaende[index].gegenstandsname);
        }

        public static bool Add(Gegenstand gegenstand, int anzahl = 1)
        {
            if (Instance == null)
            {
                PopupManager.ShowError("Inventar nicht initialisiert!");
                return false;
            }
            if (gegenstand == null)
            {
                PopupManager.ShowError("Ungültiger Gegenstand!");
                return false;
            }

            // Zuerst prüfen Gleicher Gegenstand schon vorhanden und Anzahl erhöhen
            for (int i = 0; i < Instance.gegenstaende.Length; i++)
            {
                if (Instance.gegenstaende[i] != null
                    && gegenstand.name.Equals(Instance.gegenstaende[i].gegenstandsname))
                {
                    if (Instance.gegenstaende[i].anzahl + anzahl <= gegenstand.maxAnzahl)
                    {
                        Instance.gegenstaende[i].anzahl += anzahl;
                        Instance.UpdateSlots();
                        return true;
                    }
                    else
                    {
                        PopupManager.ShowWarning("Kann nicht noch mehr davon tragen");
                        return false;
                    }
                }
            }
            // Neuer Gegenstand, suche freien Slot
            for (int i = 0; i < Instance.gegenstaende.Length; i++)
            {
                if (Instance.gegenstaende[i] == null)
                {
                    Instance.gegenstaende[i] = new InventarGegenstand(gegenstand.name, anzahl);
                    Instance.UpdateSlots();
                    return true;
                }
            }
            PopupManager.ShowWarning("Inventar voll!");
            return false;
        }

        public static bool Remove(string gegenstandsName, int anzahl)
        {
            if (Instance == null)
            {
                PopupManager.ShowError("Inventar nicht initialisiert!");
                return false;
            }
            if (string.IsNullOrEmpty(gegenstandsName))
            {
                PopupManager.ShowError("Ungültiger Gegenstand!");
                return false;
            }

            for (int i = 0; i < Instance.gegenstaende.Length; i++)
            {
                if (Instance.gegenstaende[i] != null
                    && gegenstandsName.Equals(Instance.gegenstaende[i].gegenstandsname))
                {
                    if (Instance.gegenstaende[i].anzahl < anzahl)
                    {
                        PopupManager.ShowWarning($"Nicht genug {gegenstandsName} im Inventar. Braucht {anzahl}");
                        return false;
                    }
                    Instance.gegenstaende[i].anzahl -= anzahl;
                    if (Instance.gegenstaende[i].anzahl <= 0)
                    {
                        Instance.gegenstaende[i] = null;
                    }
                    Instance.UpdateSlots();
                    return true;
                }
            }
            PopupManager.ShowWarning(gegenstandsName + " nicht im Inventar!");
            return false;
        }

        public static bool PruefeGegenstand(string bedingung, int anzahl)
        {
            Gegenstand gehalten = GetGegenstandInHand();
            if (gehalten == null)
                return false;

            string gehalteneBedingung = gehalten.bedingung;
            Debug.Log($"Teste {gehalten} mit {gehalteneBedingung}");
            if (bedingung == gehalteneBedingung)
            {
                return true;
            }
            return false;
        }

        public static Gegenstand FindeGegenstand(string name)
        {
            if (string.IsNullOrEmpty(name)) return null;
            foreach (Gegenstand g in alleGegenstaende)
                if (g.name == name) return g;
            Debug.LogWarning("Cannot find item: " + name);
            return null;
        }
    }
}