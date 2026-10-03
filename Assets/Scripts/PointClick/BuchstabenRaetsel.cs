using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEditor;
using UnityEngine;

namespace PointClick
{
    public class BuchstabenRaetsel : MonoBehaviour
    {
        [System.Serializable]
        class SaveData
        {
            public string wort;
            public GegenstandData[] platzierteBuchstaben;
        }
        [Header("Prefab eines aufnehmbaren Buchstabens")]
        [SerializeField] private GameObject buchstabenPrefab;

        [Header("Prefab eines Buchstaben Sockels")]
        [SerializeField] private GameObject buchstabenPlatzPrefab;

        [Header("Parent Objekt der Buchstaben Sockel")]
        [SerializeField] private Transform buchstabenPlatzParent;

        [Header("Breite der Fläche in der Buchstaben verteilt werden")]
        [SerializeField] private int breite = 50;

        [Header("Länge der Fläche in der Buchstaben verteilt werden")]
        [SerializeField] private int laenge = 50;

        [Header("Terrain für Höhenmessung")]
        [SerializeField] private Terrain terrain;

        [Header("Transform des Objektes, das den Weg versperrt")]
        [SerializeField] private Transform door;

        private string savefile;
        private SaveData data;
        private string[] woerter;
        private Gegenstand[] buchstabenGegenstaende;
        private BuchstabenPlatz[] buchstabenPlatzScene;
        private Coroutine checkRoutine;

        void Start()
        {
            savefile = Path.Combine(Application.persistentDataPath, "buchstaben.bin");
            if (buchstabenPrefab == null)
            {
                Debug.LogWarning("BuchstabenErzeugen Fehler: Sie müssen ein Prefab angeben");
                return;
            }

            // Die Woerter aus der Datei laden
            LoadResources();

            // Falls keine Woerter geladen, abbrechen
            if (woerter == null) return;

            // Spielstand laden
            // Falls keine Datei geladen wurde, neu erstellen
            if (!Laden()) Init();

            // Buchstabenplätze anlegen für das letzte Rätsel
            buchstabenPlatzScene = new BuchstabenPlatz[data.platzierteBuchstaben.Length];
            for (int i = 0; i < buchstabenPlatzScene.Length; i++)
            {
                var objekt = Instantiate(buchstabenPlatzPrefab, buchstabenPlatzParent);
                objekt.transform.localRotation = Quaternion.identity;
                // Sockel in Reihe anordnen
                objekt.transform.localPosition = new Vector3((i - buchstabenPlatzScene.Length / 2) * 2, 0, 0);
                var script = objekt.GetComponent<BuchstabenPlatz>();
                if (script == null) { Debug.LogError("Buchstaben Sockel hat kein BuchstabenPlatz Script!"); return; }
                buchstabenPlatzScene[i] = script;
                // Setzen des Gegenstands erzeugt Buchstabenobjekt, falls Gegenstandsname leer oder null ist
                // wird kein Buchstabe platziert = leerer Platz
                if (data.platzierteBuchstaben[i] == null)
                    script.Gegenstand = null;
                else
                    script.Gegenstand = Inventar.FindeGegenstand(data.platzierteBuchstaben[i].gegenstandsName);
            }
            if (checkRoutine != null) StopCoroutine(checkRoutine);
            checkRoutine = StartCoroutine(RaetselPruefRoutine());
        }

        void OnDisable()
        {
            if (checkRoutine != null) StopCoroutine(checkRoutine);
        }

        void LoadResources()
        {
            var textAsset = Resources.Load<TextAsset>("woerter");
            woerter = textAsset.text.Split("\n");
            Debug.Log($"Geladene Woerter ({woerter.Length.ToString()} Stück): {textAsset.text}");
            buchstabenGegenstaende = Resources.LoadAll<Gegenstand>("Items/Buchstaben");
        }

        Gegenstand FindeBuchstabenGegenstand(string buchstabe)
        {
            foreach (var ggst in buchstabenGegenstaende)
                if (ggst.bedingung == buchstabe) return ggst;
            return null;
        }

        void Init()
        {
            Debug.Log("Initialisiere Buchstaben");
            // Neue Datei anlegen und ein zufälliges Wort ermitteln
            data = new SaveData
            {
                wort = woerter[Random.Range(0, woerter.Length)].Trim()
            };
            // Speicher für platzierte Buchstaben anlegen
            data.platzierteBuchstaben = new GegenstandData[data.wort.Length];

            // Die Buchstaben im vorgegebenen Bereich verteilen
            float minX = transform.position.x;
            float maxX = transform.position.x + breite;
            float minZ = transform.position.z;
            float maxZ = transform.position.z + laenge;
            for (int i = 0; i < data.wort.Length; i++)
            {
                float x = Random.Range(minX, maxX);
                float z = Random.Range(minZ, maxZ);
                Vector3 position = new Vector3(x, CalculateHeight(x, z) + 1, z);
                GameObject buchstabe = Instantiate(buchstabenPrefab, position, transform.rotation);
                var script = buchstabe.AddComponent<GegenstandAufheben>();
                script.gegenstand = FindeBuchstabenGegenstand(data.wort[i].ToString());
            }
        }

        float CalculateHeight(float xPos, float zPos)
        {
            return terrain.SampleHeight(new Vector3(xPos, 0, zPos)) + terrain.GetPosition().y;
        }

        bool Laden()
        {
            bool ok = false;
            if (File.Exists(savefile))
            {
                FileStream meinFileStream = new FileStream(savefile, FileMode.Open, FileAccess.Read);
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                try
                {
                    data = binaryFormatter.Deserialize(meinFileStream) as SaveData;
                    Debug.Log("Buchstaben geladen aus " + savefile);
                    ok = true;
                }
                catch (System.Exception ex) { Debug.LogWarning("Fehler beim Laden: " + ex); }
                meinFileStream.Close();
            }
            return ok;
        }

        public void Speichern()
        {
            // Daten der Szenenobjekte für platzierte Buchstaben einsammeln
            for (int i = 0; i < buchstabenPlatzScene.Length; i++)
            {
                data.platzierteBuchstaben[i] = new GegenstandData
                {
                    gegenstandsName = buchstabenPlatzScene[i].Gegenstand == null ? null : buchstabenPlatzScene[i].Gegenstand.name,
                    objectId = buchstabenPlatzScene[i].name
                };
            }
            try
            {
                // Die Daten als Binärdatei speichern, gleich wie im Inventar
                FileStream meinFileStream = new FileStream(savefile, FileMode.Create);
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                binaryFormatter.Serialize(meinFileStream, data);
                meinFileStream.Close();
                Debug.Log("Buchstaben gespeichert unter " + savefile);
            }
            catch (System.Exception e)
            {
                Debug.LogError("Buchstaben konnten nicht gespeichert werden: " + e.Message);
            }
        }
        public void DeleteData()
        {
            try
            {
                File.Delete(savefile);
                Debug.Log("Buchstaben Datei gelöscht");
            }
            catch (System.Exception) { Debug.LogError("Kann Buchstaben Daten nicht löschen. Datei nicht vorhanden?"); }
        }

        IEnumerator RaetselPruefRoutine()
        {
            do
            {
                yield return new WaitForSeconds(1);
            } while (!RaetselGeloest());

            // Raetsel gelöst, Tor öffnen
            foreach (var buchstabenPlatz in buchstabenPlatzScene)
                buchstabenPlatz.RaetselGeloest();
            float t = 0;
            float duration = 3;
            Vector3 startPos = Vector3.zero;
            Vector3 endPos = new Vector3(0, 5, 0);
            while (t < duration)
            {
                float normalized = t / duration;
                door.localPosition = Vector3.Lerp(startPos, endPos, normalized);
                t += Time.deltaTime;
                yield return null;
            }

            // Routine als beendet erklären
            checkRoutine = null;
        }

        bool RaetselGeloest()
        {
            for (int i = 0; i < data.wort.Length; i++)
            {
                if (!buchstabenPlatzScene[i].Vergleiche(data.wort[i].ToString()))
                    return false;
            }
            return true;
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.cyan;
            Vector3 position = transform.position;
            Vector3 cubeSize = new Vector3(breite, 10, laenge);
            UnityEditor.Handles.DrawWireCube(position + cubeSize / 2, cubeSize);
        }

        [MenuItem("Tools/Buchstaben Assets Erstellen")]
        public static void CreateBuchstabenAssets()
        {
            // Großbuchstaben
            int first = 'A';
            int last = 'Z';
            for (int i = first; i <= last; i++)
            {
                CreateBuchstabenAsset("Großes " + (char)i);
            }
            // Kleinbuchstaben
            first = 'a';
            last = 'z';
            for (int i = first; i <= last; i++)
            {
                CreateBuchstabenAsset("Kleines " + (char)i);
            }
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private static void CreateBuchstabenAsset(string name)
        {
            Gegenstand asset = ScriptableObject.CreateInstance<Gegenstand>();
            asset.name = name;
            asset.bedingung = name.Substring(name.Length - 1, 1);
            Debug.Log("Erstelle Asset: " + asset.name);
            AssetDatabase.CreateAsset(
                asset,
                "Assets/Resources/Items/Buchstaben/" + asset.name + ".asset"
            );
        }
#endif
    }
}