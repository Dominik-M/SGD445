using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class LevelMenuController : MainMenuController
{
    public Button[] levelSelectButtons;
    void Start()
    {
        int level = 1;
        // Gibt es ein gespeichertes Level?
        // Dann laden wir dieses Level
        // Level muss als Feld vereinbart sein
        if (PlayerPrefs.HasKey("Level"))
            level = PlayerPrefs.GetInt("Level");
        for (int i = 0; i < levelSelectButtons.Length; i++)
        {
            levelSelectButtons[i].interactable = i < level;
        }
    }

    public void DeleteAll()
    {
        PlayerPrefs.DeleteAll();
        string savefolder = Application.persistentDataPath;
        foreach (var file in Directory.GetFiles(savefolder))
            File.Delete(file);
        Debug.Log("Alle Speicherdaten gelöscht");
    }
}
