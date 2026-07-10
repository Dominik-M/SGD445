using System;

[Serializable]
public class SaveData
{
    // Savefile Metadata used in SaveDataHandler
    public bool isEmpty = true;
    public string timestamp;
    public int slot;

    // Game data
    [Serializable]
    public class HighscoreItem : System.IComparable
    {
        public string name; public int score;
        public HighscoreItem(string name, int score)
        {
            this.name = name;
            this.score = score;
        }

        public override string ToString()
        {
            return $"Name: {name} Score: {score}";
        }

        public int CompareTo(object obj)
        {
            if (obj == null) return -1;
            HighscoreItem other = obj as HighscoreItem;
            if (this.score < other.score) return 1;
            if (this.score > other.score) return -1;
            return 0;
        }
    }
    public const int MAX_HIGHSCORE_ENTRIES = 3;
    public HighscoreItem[] highscoreList;

    public SaveData(int slot)
    {
        Init();
        this.slot = slot;
    }
    // Set initial values here
    public void Init()
    {
        isEmpty = true;
        highscoreList = new HighscoreItem[MAX_HIGHSCORE_ENTRIES];
    }
}