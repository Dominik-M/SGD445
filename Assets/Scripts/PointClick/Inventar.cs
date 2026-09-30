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
        public const int ANZAHL_SLOTS = 10;
        public const float AUTOSAVE_INTERVAL = 2;

        private string savefileItems;
        // Sicherstellen, dass es eine Welt Datei pro Szene gibt
        private string savefileWorld;

        private static Inventar instance;
        private void Awake() => instance = this;

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
        private readonly Gegenstand[] gegenstaende = new Gegenstand[ANZAHL_SLOTS];
        private readonly InventarButton[] slots = new InventarButton[ANZAHL_SLOTS];
        private string savefile;
        private WorldData worldData;

        private static bool zeigeInventar;
        public static bool ZeigeInventar
        {
            get => zeigeInventar;
            set
            {
                zeigeInventar = value;
                Time.timeScale = zeigeInventar ? 0 : 1;
                if (instance != null)
                {
                    if (instance.inventarPanel != null)
                        instance.inventarPanel.SetActive(zeigeInventar);
                    if (instance.inputs != null)
                    {
                        instance.inputs.cursorInputForLook = !zeigeInventar;
                    }
                    Cursor.lockState = zeigeInventar ? CursorLockMode.None : CursorLockMode.Locked;
                }
            }
        }

        private static Gegenstand gehaltenerGegenstand;
        public static Gegenstand GehaltenerGegenstand
        {
            get => gehaltenerGegenstand;
            set
            {
                gehaltenerGegenstand = value;
                if (instance != null)
                {
                    if (instance.gehaltenerGegenstandText != null)
                    {
                        if (gehaltenerGegenstand != null)
                            instance.gehaltenerGegenstandText.text = "In der Hand: " + gehaltenerGegenstand;
                        else
                            instance.gehaltenerGegenstandText.text = "Kein Gegenstand in der Hand";
                    }
                    if (instance.rechteHand != null)
                    {
                        foreach (Transform child in instance.rechteHand) Destroy(child.gameObject);
                        if (gehaltenerGegenstand != null)
                            Instantiate(gehaltenerGegenstand.prefab, instance.rechteHand).transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
                    }
                }
            }
        }

        private Coroutine autoSaveRoutine;

        void Start()
        {
            savefileItems = "gegenstaende.bin";
            // Sicherstellen, dass es eine Welt Datei pro Szene gibt
            savefileWorld = "welt" + SceneManager.GetActiveScene().buildIndex + ".bin";
            LoadResources();
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
            savefile = Path.Combine(Application.persistentDataPath, savefileItems);
            // Die Liste laden bzw. neu erzeugen
            ListeLaden();
            // Inventar aktualisieren
            UpdateSlots();
            GehaltenerGegenstand = null;

            // Welt Zustand laden
            worldData = new WorldData(Path.Combine(Application.persistentDataPath, savefileWorld));
            worldData.Load();

            // Spielerposition setzen
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
                player.transform.position = new Vector3(worldData.playerX, worldData.playerY, worldData.playerZ);
        }

        private void OnEnable()
        {
            inventarAction.action?.Enable();
            Cursor.lockState = CursorLockMode.Locked;
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
                worldData.Save();
                ListeSpeichern();
            }
        }

        void UpdateSlots()
        {
            for (int i = 0; i < gegenstaende.Length; i++)
            {
                slots[i].Ggst = gegenstaende[i];
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
                GegenstandData[] geladeneGegenstaende;
                try
                {
                    geladeneGegenstaende = binaryFormatter.Deserialize(meinFileStream) as GegenstandData[];
                    for (int i = 0; i < ANZAHL_SLOTS; i++)
                        if (i < geladeneGegenstaende.Length && geladeneGegenstaende[i] != null)
                        {
                            gegenstaende[i] = FindGegenstand(geladeneGegenstaende[i].gegenstandsName);
                            if (gegenstaende[i] != null)
                                gegenstaende[i].anzahl = geladeneGegenstaende[i].anzahl;
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
            // Die Speicherdaten aus den Inventargegenständen extrahieren
            GegenstandData[] gegenstaendDaten = new GegenstandData[gegenstaende.Length];
            for (int i = 0; i < gegenstaende.Length; i++)
            {
                if (gegenstaende[i] != null)
                {
                    gegenstaendDaten[i] = new GegenstandData
                    {
                        gegenstandsName = gegenstaende[i].name,
                        anzahl = gegenstaende[i].anzahl
                    };
                }
                else
                {
                    gegenstaendDaten[i] = null;
                }
            }
            // Eine neue Instanz von FileStream erzeugen
            FileStream meinFileStream = new FileStream(savefile, FileMode.Create);
            // Eine Instanz von BinaryFormatter erzeugen
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            // Die Daten speichern. Dazu wird einfach die Liste serialisiert
            binaryFormatter.Serialize(meinFileStream, gegenstaendDaten);
            meinFileStream.Close();
            Debug.Log("Gegenstände gespeichert unter " + savefile);
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

        public static bool Add(Gegenstand gegenstand)
        {
            if (instance == null)
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
            for (int i = 0; i < instance.gegenstaende.Length; i++)
            {
                if (instance.gegenstaende[i] != null
                    && gegenstand.name.Equals(instance.gegenstaende[i].name))
                {
                    if (instance.gegenstaende[i].anzahl + gegenstand.anzahl <= gegenstand.maxAnzahl)
                    {
                        instance.gegenstaende[i].anzahl += gegenstand.anzahl;
                        instance.UpdateSlots();
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
            for (int i = 0; i < instance.gegenstaende.Length; i++)
            {
                if (instance.gegenstaende[i] == null)
                {
                    instance.gegenstaende[i] = gegenstand;
                    gegenstand.anzahl = 1;
                    instance.UpdateSlots();
                    return true;
                }
            }
            PopupManager.ShowWarning("Inventar voll!");
            return false;
        }

        public static bool Remove(string gegenstandsName, int anzahl)
        {
            if (instance == null)
            {
                PopupManager.ShowError("Inventar nicht initialisiert!");
                return false;
            }
            if (string.IsNullOrEmpty(gegenstandsName))
            {
                PopupManager.ShowError("Ungültiger Gegenstand!");
                return false;
            }

            for (int i = 0; i < instance.gegenstaende.Length; i++)
            {
                if (instance.gegenstaende[i] != null
                    && gegenstandsName.Equals(instance.gegenstaende[i].name))
                {
                    if (instance.gegenstaende[i].anzahl < anzahl)
                    {
                        PopupManager.ShowWarning($"Nicht genug {gegenstandsName} im Inventar. Braucht {anzahl}");

                    }
                    instance.gegenstaende[i].anzahl -= anzahl;
                    if (instance.gegenstaende[i].anzahl <= 0)
                    {
                        if (instance.gegenstaende[i].Equals(gehaltenerGegenstand))
                            GehaltenerGegenstand = null;
                        instance.gegenstaende[i] = null;
                    }
                    instance.UpdateSlots();
                    return true;
                }
            }
            PopupManager.ShowWarning(gegenstandsName + " nicht im Inventar!");
            return false;
        }

        public static Gegenstand Get(string gegenstandsName)
        {
            if (instance == null)
            {
                PopupManager.ShowError("Inventar nicht initialisiert!");
                return null;
            }
            if (string.IsNullOrEmpty(gegenstandsName))
            {
                PopupManager.ShowError("Ungültiger Gegenstand!");
                return null;
            }

            for (int i = 0; i < instance.gegenstaende.Length; i++)
            {
                if (instance.gegenstaende[i] != null
                    && gegenstandsName.Equals(instance.gegenstaende[i].name))
                {
                    return instance.gegenstaende[i];
                }
            }
            Debug.LogWarning($"Inventar.Get({gegenstandsName}): Nicht gefunden");
            return null;
        }

        public static bool PruefeGegenstand(string bedingung, int anzahl)
        {
            Gegenstand gehalten = GehaltenerGegenstand;
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

        public static Gegenstand FindGegenstand(string name)
        {
            foreach (Gegenstand g in alleGegenstaende)
                if (g.name == name) return g;
            Debug.LogWarning("Cannot find item: " + name);
            return null;
        }
    }
}