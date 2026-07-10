using System;
using System.IO;
using UnityEngine;

public class SaveDataHandler
{
    public static void Write(SaveData data)
    {
        string path = GetSavefilePath(data.slot);
        string json = Serialize(data);
        File.WriteAllText(path, json);
        Debug.Log("Savedata written to: " + path);
    }

    public static SaveData Read(int slot = 0)
    {
        string path = GetSavefilePath(slot);
        SaveData data = null;
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            Debug.Log("Savedata read from: " + path);
            data = Deserialize(json);
        }
        if (data == null)
        {
            Debug.Log("New SaveData created for slot: " + slot);
            data = new SaveData(slot);
        }
        return data;
    }

    public static void Delete(int slot = 0)
    {
        string path = GetSavefilePath(slot);
        if (File.Exists(path))
        {
            File.Delete(path);
            Debug.Log("Deleted savefile: " + path);
        }
    }

    private static string GetSavefilePath(int slot = 0)
    {
        return Path.Combine(Application.persistentDataPath, "savegame" + slot + ".json");
    }

    private static string Serialize(SaveData data)
    {
        data.timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        data.isEmpty = false;
        return JsonUtility.ToJson(data, true);
    }

    private static SaveData Deserialize(string json)
    {
        if (string.IsNullOrEmpty(json))
        {
            Debug.LogWarning("Deserialize: JSON data empty or invalid.");
            return null;
        }
        return JsonUtility.FromJson<SaveData>(json);
    }
}