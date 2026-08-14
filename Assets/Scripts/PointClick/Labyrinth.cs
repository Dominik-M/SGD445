using UnityEngine;
using UnityEngine.InputSystem;

public class Labyrinth : GeneratorBehaviour
{
    [Header("Prefab des Bodens des Labyrinths")]
    [SerializeField] private GameObject floorPrefab;
    [Header("Prefab eines Mauerstücks")]
    [SerializeField] private GameObject wallPrefab;
    [Header("Generator Einstellungen")]
    [SerializeField] private int laenge = 10, breite = 10, etagen = 3;
    private float wandbreite, wandhoehe, wanddicke;

    public override void Generate()
    {
        // Parameter prüfen
        if (floorPrefab ==null)
        {
            Debug.LogWarning("Boden Prefab nicht gesetzt!");
            return;
        }
        if (wallPrefab == null)
        {
            Debug.LogWarning("Mauer Prefab nicht gesetzt!");
            return;
        }
        if (laenge <= 0)
        {
            Debug.LogWarning("Ungültige Laenge: " + laenge);
            return;
        }
        if (breite <= 0)
        {
            Debug.LogWarning("Ungültige Breite: " + breite);
            return;
        }
        if (etagen <= 0)
        {
            Debug.LogWarning("Ungültige Hoehe: " + etagen);
            return;
        }

        // Mauernstück Höhe und Breite ermitteln
        // Das untergeordnete Objekt der Mauer beschaffen
        Transform kindElement = wallPrefab.transform.GetChild(0);
        if (kindElement == null)
        {
            Debug.Log("Es wurde kein untergeordentes Objekt gefunden.");
            return;
        }
        // Die Größe beschaffen über den MeshFilter
        MeshFilter meshFilter = kindElement.GetComponent<MeshFilter>();
        if (meshFilter == null)
        {
            Debug.Log("Es wird ein Objekt mit Mesh benötigt.");
            return;
        }

        // Das Mesh über den MeshFilter beschaffen
        Mesh mesh = meshFilter.sharedMesh;
        Vector3 meshGroesse = mesh.bounds.size;

        // Die Skalierung beschaffen
        Vector3 skalierung = kindElement.transform.localScale;

        // Die Größe berechnen
        // Bei der Breite subtrahieren wir einen Korrekturwert für ein bisschen Überlappung
        wandbreite = meshGroesse.x * skalierung.x - 0.1f;
        wandhoehe = meshGroesse.y * skalierung.y;
        wanddicke = meshGroesse.z * skalierung.z;

        for (int y = 0; y < etagen; y++)
        {
            // Die Etagen Wurzel
            Transform etageParent = new GameObject("Floor_" + y).transform;
            etageParent.SetParent(transform);
            etageParent.transform.localPosition = new Vector3(0, y * wandhoehe, 0);

            // Boden und Decke
            Transform floorParent = new GameObject("Boden").transform;
            floorParent.SetParent(etageParent);
            floorParent.localPosition = Vector3.zero;
            for (int x = 0; x < breite; x++)
                for (int z = 0; z < laenge; z++)
                {
                    Transform floor = Instantiate(floorPrefab, floorParent, false).transform;
                    floor.transform.localPosition = new Vector3((x - breite / 2 + 0.5f) * wandbreite, 0, (z - laenge / 2 + 0.5f) * wandbreite);
                    // Nur auf der letzten Etage eine Decke ziehen
                    // Bei allen anderen ist die Decke der Boden der nächsten Etage
                    if (y == etagen - 1)
                    {
                        Transform ceiling = Instantiate(floorPrefab, floorParent, false).transform;
                        ceiling.transform.localPosition = new Vector3((x - breite / 2 + 0.5f) * wandbreite, wandhoehe, (z - laenge / 2 + 0.5f) * wandbreite);
                    }
                }

            // Außenmauer
            Transform wallsParent = new GameObject("Mauern").transform;
            wallsParent.SetParent(etageParent);
            wallsParent.localPosition = Vector3.zero;
            for (int x = 0; x < breite; x++)
            {
                float neuePosX = (x - breite / 2) * wandbreite + wanddicke;
                float neuePosZ = laenge / 2 * wandbreite;
                // Oben
                GameObject wall = Instantiate(wallPrefab, wallsParent, false);
                wall.transform.localPosition = new Vector3(neuePosX, 0, neuePosZ);
                // Unten
                neuePosZ *= -1;
                if (y == 0 && x == breite / 2) continue;// Eingang unten frei lassen
                wall = Instantiate(wallPrefab, wallsParent, false);
                wall.transform.localPosition = new Vector3(neuePosX, 0, neuePosZ);
            }
            for (int z = 1; z <= laenge; z++)
            {
                float neuePosX = -breite / 2 * wandbreite;
                float neuePosZ = (z - laenge / 2) * wandbreite - wanddicke;
                // Links
                GameObject wall = Instantiate(wallPrefab, wallsParent, false);
                wall.transform.localPosition = new Vector3(neuePosX, 0, neuePosZ);
                wall.transform.localRotation = Quaternion.Euler(0, 90, 0);
                // Rechts
                neuePosX *= -1;
                wall = Instantiate(wallPrefab, wallsParent, false);
                wall.transform.localPosition = new Vector3(neuePosX, 0, neuePosZ);
                wall.transform.localRotation = Quaternion.Euler(0, 90, 0);
            }

            // Innenwände
            for (int x = 1; x < breite-1; x++)
            {
                for (int z = 2; z < laenge; z++)
                {
                    GameObject wall = Instantiate(wallPrefab, wallsParent, false);

                    // Die neue Position zwischenspeichern
                    float neuePosX = (x - breite / 2) * wandbreite + wanddicke;
                    float neuePosZ = (z - laenge / 2) * wandbreite - wanddicke;

                    // Die neue Position setzen
                    wall.transform.localPosition = new Vector3(neuePosX, 0, neuePosZ);
                    wall.transform.localRotation = Quaternion.Euler(0, Random.Range(0, 4) * 90, 0);
                }
            }
        }
    }
}
