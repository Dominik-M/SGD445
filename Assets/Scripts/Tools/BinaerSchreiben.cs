using System.IO;
using UnityEngine;

public class BinaerSchreiben : MonoBehaviour
{
    public int[] Daten=new int[] { 5 };
    const string FILENAME = "testdaten.bin";

    void Start()
    {
        // Dateipfad erstellen
        var path = Path.Combine(Application.persistentDataPath, FILENAME);
        Write(path);
        Read(path);
    }

    void Write(string path)
    {
        Debug.Log("Schreibe Daten in: " + path);
        // FileStream und BinaryWriter erstellen
        FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write);
        BinaryWriter bw = new BinaryWriter(fs);
        // Daten schreiben
        for (int i = 0; i < Daten.Length; i++)
        {
            bw.Write(Daten[i]);
            Debug.Log($"Daten[{i}]={Daten[i]}");
        }
        bw.Close();
        fs.Close();
    }

    void Read(string path)
    {
        Debug.Log("Lese Daten aus: " + path);
        // FileStream und BinaryReader erstellen
        FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
        BinaryReader br = new BinaryReader(fs);
        // Daten lesen
        for (int i = 0; i < Daten.Length; i++)
        {
            Daten[i] = br.ReadInt32();
            Debug.Log($"Daten[{i}]={Daten[i]}");
        }
        br.Close();
        fs.Close();
    }
}
