using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static SaveData;

public class GameController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject PlayerPrefab;
    [SerializeField] private GameObject PlayerDestroyEffectPrefab;
    [SerializeField] private GameObject[] LevelPrefabs;
    [SerializeField] private GameObject TextPrefab;

    [Header("Sounds")]
    [SerializeField] private AudioClip LooseSound;
    [SerializeField] private AudioClip WinSound;
    [SerializeField] private AudioClip GameOverSound;
    [SerializeField] private AudioClip GameCompleteSound;
    [SerializeField] private AudioClip PickupSound;

    [Header("Settings")]
    [SerializeField] private float TimePerLevel;
    [SerializeField] private int StartLifes;

    private static GameController instance;
    public static GameController I => instance;

    public event Action<int, int> OnScoreChanged;
    public event Action OnGameOver;

    private int score;
    private float remainingTime;
    private bool gameover, gamecomplete, paused;
    private int lifesRemaining, deathCounter;
    private GameObject player;
    private GameObject currentLevel;
    private int currentLevelIdx;
    private Transform respawn;
    private Transform finish;
    private FollowerCamera followerCamera;
    private AudioSource audioSource;
    private SaveData saveData;
    private List<HighscoreItem> highscoreList = new();

    public int CurrentLevelIndex => currentLevelIdx;
    public int LifesRemaining => lifesRemaining;
    public int DeathCounter => deathCounter;
    public float RemainingTime => remainingTime;
    public bool IsGameOver => gameover;
    public bool IsGameComplete => gamecomplete;
    public int Score
    {
        get => score; set
        {
            int prev = score;
            score = value;
            OnScoreChanged?.Invoke(prev, score);
        }
    }
    public string PlayerName { get; set; }
    public HighscoreItem GetHighscoreItem(int index)
    {
        if (highscoreList == null || index < 0 || index >= highscoreList.Count) return null;
        return highscoreList[index];
    }

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("There is already an active GameController instance");
        }
        instance = this;
    }

    void Start()
    {
        gameover = false;
        gamecomplete = false;
        lifesRemaining = StartLifes;
        deathCounter = 0;
        followerCamera = FindAnyObjectByType<FollowerCamera>();
        // ensure audio source
        if (audioSource == null) audioSource = GetComponent<AudioSource>();
        if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();
        StartLevel(0);
    }

    private void OnEnable()
    {
        Load();
    }

    private void OnDisable()
    {
        Save();
    }

    void Update()
    {
        if (gameover || paused) return;

        remainingTime -= Time.deltaTime;
        if (remainingTime <= 0)
        {
            remainingTime = 0;
            GameOver();
        }
    }

    public void Load()
    {
        Debug.Log("Loading Highscores");
        saveData = SaveDataHandler.Read();
        highscoreList.Clear();
        for (int i = 0; i < saveData.highscoreList.Length; i++)
        {
            var item = saveData.highscoreList[i];
            if (item != null) highscoreList.Add(item);
            Debug.Log($"saveData.highscoreList[{i}] = {saveData.highscoreList[i]}");
        }
    }

    public void Save()
    {
        Debug.Log("Saving Highscores");
        for (int i = 0; i < saveData.highscoreList.Length; i++)
        {
            saveData.highscoreList[i] = GetHighscoreItem(i);
            Debug.Log($"saveData.highscoreList[{i}] = {saveData.highscoreList[i]}");
        }
        SaveDataHandler.Write(saveData);
    }

    public void PlaySound(AudioClip sound)
    {
        if (audioSource != null && sound != null)
            audioSource.PlayOneShot(sound);
    }

    public void StartLevel(int level)
    {
        Debug.Log($"StartLevel({level})");
        if (level < 0 || level >= LevelPrefabs.Length)
        {
            Debug.LogWarning($"StartLevel({level}): Invalid index");
            return;
        }
        currentLevelIdx = level;
        if (currentLevel != null) Destroy(currentLevel);
        currentLevel = Instantiate(LevelPrefabs[level], Vector3.zero, Quaternion.identity);
        respawn = currentLevel.transform.Find("Respawn");
        finish = currentLevel.transform.Find("Finish");
        foreach (TriggerHandler trigger in currentLevel.GetComponentsInChildren<TriggerHandler>())
            trigger.OnEnter += TriggerEnterHook;
        if (respawn == null)
        {
            Debug.LogWarning($"StartLevel({level}): Loaded level invalid - missing respawn");
            return;
        }
        if (finish == null)
        {
            Debug.LogWarning($"StartLevel({level}): Loaded level invalid - missing finish");
            return;
        }
        followerCamera.MinX = respawn.position.x - 4;
        followerCamera.MaxX = finish.position.x;
        remainingTime = TimePerLevel;
        paused = false;
        Respawn();
    }

    public void Respawn()
    {
        if (player == null)
        {
            player = Instantiate(PlayerPrefab);
        }
        player.transform.position = respawn.position;
        player.transform.rotation = Quaternion.identity;
        followerCamera.Target = player.transform;
    }

    public void Die()
    {
        if (player == null)
        {
            Debug.LogWarning("Die(): Player object already destroyed");
            return;
        }
        Debug.Log("Player died");
        PlaySound(LooseSound);
        deathCounter++;
        lifesRemaining--;
        if (PlayerDestroyEffectPrefab) Instantiate(PlayerDestroyEffectPrefab, player.transform.position, Quaternion.identity);
        Destroy(player);

        // Delay the respawn or gameover, but only if enough time is left
        // Otherwise Gameover will be called by timeout before
        if (remainingTime > 2)
        {
            if (lifesRemaining > 0) Invoke(nameof(Respawn), 1f);
            else Invoke(nameof(GameOver), 1f);
        }
    }

    public void Finish()
    {
        Debug.Log($"Level {currentLevelIdx} Finished");
        paused = true;
        PlaySound(WinSound);
        AddTimeBonus();
        Destroy(player);
        Invoke(nameof(NextLevel), 1);
    }

    void NextLevel()
    {
        currentLevelIdx++;
        if (currentLevelIdx < LevelPrefabs.Length)
            StartLevel(currentLevelIdx);
        else
            GameComplete();
    }

    void AddTimeBonus()
    {
        int bonus = (int)(remainingTime * 10);
        Debug.Log($"AddTimeBonus(): {bonus}");
        Score += bonus;
        GameObject obj = Instantiate(TextPrefab, player.transform.position, Quaternion.identity);
        obj.GetComponent<TextMeshPro>().text = "Zeitbonus: " + bonus;
        Destroy(obj, 1);
    }

    void GameOver()
    {
        if (gameover) return;

        gameover = true;
        Debug.Log("Game Over");
        Die();
        PlaySound(GameOverSound);
        AddToHighscoreList(new HighscoreItem(PlayerName, Score));
        OnGameOver?.Invoke();
    }
    void GameComplete()
    {
        if (gameover) return;

        gameover = true;
        gamecomplete = true;
        Debug.Log("Game Complete");
        PlaySound(GameCompleteSound);
        AddToHighscoreList(new HighscoreItem(PlayerName, Score));
        OnGameOver?.Invoke();
    }

    void AddToHighscoreList(HighscoreItem item)
    {
        highscoreList.Add(item);
        highscoreList.Sort();
    }

    void TriggerEnterHook(GameObject trigger, GameObject other)
    {
        if (!other.CompareTag("Player")) return;

        if (trigger.CompareTag("Boundary"))
        {
            Die();
        }
        else if (trigger.CompareTag("Finish"))
        {
            Finish();
        }
        else if (trigger.CompareTag("Pickup"))
        {
            Debug.Log("Pickup collected");
            PlaySound(PickupSound);
            Destroy(trigger.gameObject);
            Score += 100;
        }
    }
}