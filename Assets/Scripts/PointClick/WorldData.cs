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
        public float playerX, playerY, playerZ;
        GegenstandData[] gegenstaendeAufheben;
        GegenstandData[] gegenstaendeAnwenden;

        public WorldData(string savefile)
        {
            this.savefile = savefile;
        }

        public void Save()
        {
            Debug.Log("Welt wird gespeichert...");
            // Szenen Objekte beschaffen
            var player = Inventar.Instance.Player;
            GegenstandAufheben[] gegenstandAufhebenScene = GameObject.FindObjectsByType<GegenstandAufheben>(FindObjectsSortMode.None);
            GegenstandAnwenden[] gegenstandAnwendenScene = GameObject.FindObjectsByType<GegenstandAnwenden>(FindObjectsSortMode.None);

            // Daten extrahieren
            if (player != null)
            {
                playerX = player.transform.position.x;
                playerY = player.transform.position.y;
                playerZ = player.transform.position.z;
                Debug.Log($"Spielerposition = {playerX.ToString("F1")} {playerY.ToString("F1")} {playerZ.ToString("F1")}");
            }
            else
            {
                Debug.LogWarning("Spielerposition kann nicht gespeichert werden: Spieler nicht gefunden");
            }
            gegenstaendeAufheben = new GegenstandData[gegenstandAufhebenScene.Length];
            for (int i = 0; i < gegenstaendeAufheben.Length; i++)
            {
                gegenstaendeAufheben[i] = new GegenstandData
                {
                    x = gegenstandAufhebenScene[i].transform.position.x,
                    y = gegenstandAufhebenScene[i].transform.position.y,
                    z = gegenstandAufhebenScene[i].transform.position.z,
                    gegenstandsName = gegenstandAufhebenScene[i].gegenstand.name
                };
            }
            gegenstaendeAnwenden = new GegenstandData[gegenstandAnwendenScene.Length];
            for (int i = 0; i < gegenstaendeAnwenden.Length; i++)
            {
                gegenstaendeAnwenden[i] = new GegenstandData
                {
                    x = gegenstandAnwendenScene[i].transform.position.x,
                    y = gegenstandAnwendenScene[i].transform.position.y,
                    z = gegenstandAnwendenScene[i].transform.position.z,
                    bedingung = gegenstandAnwendenScene[i].bedingung,
                    anzahl = gegenstandAnwendenScene[i].anzahl,
                    zustand = gegenstandAnwendenScene[i].Zustand,
                    objectId = gegenstandAnwendenScene[i].SaveId
                };
                // Debug.Log($"WorldData.gegenstaendeAnwenden[{i}]: ID={gegenstaendeAnwenden[i].objectId}");
            }

            // Daten in Datei schreiben
            FileStream meinFileStream = new FileStream(savefile, FileMode.Create);
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            binaryFormatter.Serialize(meinFileStream, this);
            meinFileStream.Close();

            // Buchstabendaten speichern
            GameObject.FindAnyObjectByType<BuchstabenRaetsel>()?.Speichern();

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

            if (!ok)
            {
                Debug.Log("Keine Weltdaten geladen. Szenenobjekte bleiben unverändert");
                return;
            }
            // Aufhebbare Objekte
            // Alte Objekte löschen
            GegenstandAufheben[] gegenstandAufhebenScene = GameObject.FindObjectsByType<GegenstandAufheben>(FindObjectsSortMode.None);
            foreach (GegenstandAufheben gegenstand in gegenstandAufhebenScene)
                GameObject.Destroy(gegenstand.gameObject);

            // Neue Objekte erstellen
            foreach (GegenstandData data in gegenstaendeAufheben)
            {
                Gegenstand gegenstand = Inventar.FindeGegenstand(data.gegenstandsName);
                var obj = GameObject.Instantiate(gegenstand.prefab);
                obj.transform.position = new Vector3(data.x, data.y, data.z);
                var script = obj.AddComponent<GegenstandAufheben>();
                script.gegenstand = gegenstand;
            }

            // Interaktionsobjekte
            // Finde gespeicherte Objekte in der Szene und aktualisiere den Zustand
            GegenstandAnwenden[] gegenstandAnwendenScene = GameObject.FindObjectsByType<GegenstandAnwenden>(FindObjectsSortMode.None);
            foreach (GegenstandData data in gegenstaendeAnwenden)
            {
                var script = FindGegenstandSzene(gegenstandAnwendenScene, data.objectId);
                if (script == null) continue;
                script.transform.position = new Vector3(data.x, data.y, data.z);
                script.anzahl = data.anzahl;
                script.bedingung = data.bedingung;
                script.Zustand = data.zustand;
            }
        }

        GegenstandAnwenden FindGegenstandSzene(GegenstandAnwenden[] gegenstandAnwendenScene, string id)
        {
            foreach (GegenstandAnwenden item in gegenstandAnwendenScene)
                if (item.SaveId.Equals(id))
                    return item;
            Debug.LogWarning("GegenstandAnwenden nicht gefunden mit ID=" + id);
            return null;
        }

        public void Delete()
        {
            GameObject.FindAnyObjectByType<BuchstabenRaetsel>()?.DeleteData();
            File.Delete(savefile);
            Debug.Log("Weltdaten gelöscht");
        }
    }
}