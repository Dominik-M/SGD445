using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace PointClick
{
    [System.Serializable]
    public class WorldData
    {
        readonly string savefile;
        float playerX, playerY, playerZ;
        GegenstandData[] gegenstaendeAufheben;
        GegenstandData[] gegenstaendeAnwenden;

        public WorldData(string savefile)
        {
            this.savefile = savefile;
        }

        public void Save()
        {
            // Szenen Objekte beschaffen
            Transform player = GameObject.FindWithTag("Player").transform;
            GegenstandAufheben[] gegenstandAufhebenScene = GameObject.FindObjectsByType<GegenstandAufheben>(FindObjectsSortMode.None);
            GegenstandAnwenden[] gegenstandAnwendenScene = GameObject.FindObjectsByType<GegenstandAnwenden>(FindObjectsSortMode.None);

            // Daten extrahieren
            playerX = player.position.x;
            playerY = player.position.y;
            playerZ = player.position.z;
            gegenstaendeAufheben = new GegenstandData[gegenstandAufhebenScene.Length];
            for (int i = 0; i < gegenstaendeAufheben.Length; i++)
            {
                gegenstaendeAufheben[i] = new GegenstandData();
                gegenstaendeAufheben[i].x = gegenstandAufhebenScene[i].transform.position.x;
                gegenstaendeAufheben[i].y = gegenstandAufhebenScene[i].transform.position.y;
                gegenstaendeAufheben[i].z = gegenstandAufhebenScene[i].transform.position.z;
                gegenstaendeAufheben[i].gegenstandsName = gegenstandAufhebenScene[i].gegenstand.name;
            }
            gegenstaendeAnwenden = new GegenstandData[gegenstandAnwendenScene.Length];
            for (int i = 0; i < gegenstaendeAnwenden.Length; i++)
            {
                gegenstaendeAnwenden[i] = new GegenstandData();
                gegenstaendeAnwenden[i].x = gegenstandAnwendenScene[i].transform.position.x;
                gegenstaendeAnwenden[i].y = gegenstandAnwendenScene[i].transform.position.y;
                gegenstaendeAnwenden[i].z = gegenstandAnwendenScene[i].transform.position.z;
                gegenstaendeAnwenden[i].bedingung = gegenstandAnwendenScene[i].bedingung;
                gegenstaendeAnwenden[i].anzahl = gegenstandAnwendenScene[i].anzahl;
                gegenstaendeAnwenden[i].zustand = gegenstandAnwendenScene[i].zustand;
            }

            // Daten in Datei schreiben
            FileStream meinFileStream = new FileStream(savefile, FileMode.Create);
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            binaryFormatter.Serialize(meinFileStream, this);
            meinFileStream.Close();
            Debug.Log("Weltdaten gespeichert unter " + savefile);
        }

        public void Load()
        {
            bool ok = false;
            if (File.Exists(savefile))
            {
                // Eine neue instanz von FileStream erzeugen
                // Die Datei wird zum lesen geöffnet
                FileStream meinFileStream = new FileStream(savefile, FileMode.Open, FileAccess.Read);
                // Eine instanz von BinaryFormatter erzeugen
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                try
                {
                    // Die Daten deserialisieren und ablegen
                    WorldData geladeneWelt = binaryFormatter.Deserialize(meinFileStream) as WorldData;
                    playerX = geladeneWelt.playerX;
                    playerY = geladeneWelt.playerY;
                    playerZ = geladeneWelt.playerZ;
                    gegenstaendeAufheben = geladeneWelt.gegenstaendeAufheben;
                    gegenstaendeAnwenden = geladeneWelt.gegenstaendeAnwenden;
                    Debug.Log("Welt Daten geladen aus " + savefile);
                    ok = true;
                }
                catch (Exception ex) { Debug.LogWarning("Fehler beim Laden: " + ex); }

                meinFileStream.Close();
            }

            if (ok)
            {
                // Alte Objekte löschen
                GegenstandAufheben[] gegenstandAufhebenScene = GameObject.FindObjectsByType<GegenstandAufheben>(FindObjectsSortMode.None);
                GegenstandAnwenden[] gegenstandAnwendenScene = GameObject.FindObjectsByType<GegenstandAnwenden>(FindObjectsSortMode.None);
                foreach (GegenstandAufheben gegenstand in gegenstandAufhebenScene)
                    GameObject.Destroy(gegenstand.gameObject);
                foreach (GegenstandAnwenden gegenstand in gegenstandAnwendenScene)
                    GameObject.Destroy(gegenstand.gameObject);

                // Neue Objekte erstellen
                foreach (GegenstandData data in gegenstaendeAufheben)
                {
                    Gegenstand gegenstand = Inventar.Get(data.gegenstandsName);
                    var obj = GameObject.Instantiate(gegenstand.prefab);
                    obj.transform.position = new Vector3(data.x, data.y, data.z);
                    var script = obj.AddComponent<GegenstandAufheben>();
                    script.gegenstand = gegenstand;
                }
                foreach (GegenstandData data in gegenstaendeAnwenden)
                {
                    Gegenstand gegenstand = Inventar.Get(data.gegenstandsName);
                    var obj = GameObject.Instantiate(gegenstand.prefab);
                    obj.transform.position = new Vector3(data.x, data.y, data.z);
                    var script = obj.AddComponent<GegenstandAnwenden>();
                    script.gegenstand = gegenstand;
                    script.anzahl = data.anzahl;
                    script.bedingung = data.bedingung;
                    script.zustand = data.zustand;
                }
            }
            else
            {
                Debug.Log("Keine Weltdaten geladen. Szenenobjekte bleiben unverändert");
            }
        }

        public void Delete()
        {
            File.Delete(savefile);
            Debug.Log("Weltdaten gelöscht");
        }
    }
}