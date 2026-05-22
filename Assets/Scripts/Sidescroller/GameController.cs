using UnityEngine;
using System;
using Unity.VisualScripting;

public class GameController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject PlayerPrefab;
    [SerializeField] private GameObject PlayerDestroyEffectPrefab;
    [SerializeField] private GameObject[] LevelPrefabs;

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

    private int score;
    private float remainingTime;
    private bool gameover, gamecomplete;
    private int lifesRemaining, deathCounter;
    private GameObject player;
    private GameObject currentLevel;
    private int currentLevelIdx;
    private Transform respawn;
    private Transform finish;
    private FollowerCamera followerCamera;
    private AudioSource audioSource;

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

    void Start()
    {
        if (instance != null)
        {
            Debug.LogWarning("There is already an active GameController instance");
        }
        instance = this;
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

    void Update()
    {
        if (gameover) return;
        remainingTime -= Time.deltaTime;
        if (remainingTime <= 0)
        {
            remainingTime = 0;
            GameOver();
        }
    }

    public void PlaySound(AudioClip sound)
    {
        if (audioSource != null && sound != null)
            audioSource.PlayOneShot(sound);
    }

    public void StartLevel(int level)
    {
        Debug.Log($"StartLevel({level})");
        if (level >= 0 && level < LevelPrefabs.Length)
        {
            currentLevelIdx = level;
            if (currentLevel != null) Destroy(currentLevel);
            currentLevel = Instantiate(LevelPrefabs[level], Vector3.zero, Quaternion.identity);
            respawn = currentLevel.transform.Find("Respawn");
            finish = currentLevel.transform.Find("Finish");
            foreach (TriggerHandler trigger in currentLevel.GetComponentsInChildren<TriggerHandler>())
                trigger.OnEnter += TriggerEnterHook;
            if (respawn != null && finish != null)
            {
                followerCamera.MinX = respawn.position.x - 4;
                followerCamera.MaxX = finish.position.x;
                remainingTime = TimePerLevel;
                Respawn();
            }
            else
            {
                Debug.LogWarning($"StartLevel({level}): Loaded level invalid - missing respawn or finish");
            }
        }
        else
        {
            Debug.LogWarning($"StartLevel({level}): Invalid index");
        }
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
        if (player != null)
        {
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
                if (lifesRemaining > 0) Invoke(nameof(Respawn), 2);
                else Invoke(nameof(GameOver), 2);
            }
        }
    }

    public void Finish()
    {
        Debug.Log($"Level {currentLevelIdx} Finished");
        PlaySound(WinSound);
        Destroy(player);
        Invoke(nameof(NextLevel), 1);
    }

    void NextLevel()
    {
        if (currentLevelIdx + 1 < LevelPrefabs.Length)
            StartLevel(currentLevelIdx + 1);
        else
            GameComplete();
    }

    void GameOver()
    {
        if (!gameover)
        {
            gameover = true;
            Debug.Log("Game Over");
            Die();
            PlaySound(GameOverSound);
        }
    }
    void GameComplete()
    {
        if (!gameover)
        {
            gameover = true;
            gamecomplete = true;
            Debug.Log("Game Complete");
            PlaySound(GameCompleteSound);
        }
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
            Score++;
        }
    }
}