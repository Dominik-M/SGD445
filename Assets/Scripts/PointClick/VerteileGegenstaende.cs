using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VerteileGegenstaende : MonoBehaviour
{
    // Ein Array f?r die Objekte
    public GegenstandItem[] gegenstaendeScene;

    // Start is called before the first frame update
    void Start()
    {
        int anzahl = 0;

        // Den Mesh-Filter beschaffen
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        // hat das Objekt einen MeshFilter?
        if (meshFilter == null)
        {
            Debug.Log("Es wird ein Objekt mit einen Mesh benötigt");
            return;
        }

        // Das Mesh ?ber den MeshFilter beschaffen
        Mesh mesh = meshFilter.mesh;
        Vector3 size = mesh.bounds.size;
        Vector3 scale = transform.localScale;
        // ist ein Objekt ?bergeben worden?
        // Dazu pr?fen wir den ersten Eintrag
        if(gegenstaendeScene[0].gegenstand == null)
        {
            Debug.Log("Es muss ein Objket übergeben werden");
            return;
        }
   
        // Die maximalen Grenzen ermitteln
        int maxX = (int)(size.x * scale.x) / 2;
        int maxZ = (int)(size.z * scale.z) / 2;

       
        // In einer geschachtelten Schleife, die l?uft, bis
        // ein leerer Eintrag gefunden wird oder bis das Ende erreicht ist
        while((anzahl < gegenstaendeScene.Length) && (gegenstaendeScene[anzahl].gegenstand != null))
        {
            for(int schleife = 0; schleife < gegenstaendeScene[anzahl].anzahl; schleife++)
            {
                // eine zuf?llige Position ermitteln
                Vector3 position = new Vector3(Random.Range(transform.position.x - maxX, transform.position.x + maxX), gegenstaendeScene[anzahl].gegenstand.position.y, Random.Range(transform.position.z - maxZ, transform.position.z + maxZ));
                // Die Instanz erzeugen
                Instantiate(gegenstaendeScene[anzahl].gegenstand, position, gegenstaendeScene[anzahl].gegenstand.rotation);
            }
            anzahl++;
        }
    }
}
