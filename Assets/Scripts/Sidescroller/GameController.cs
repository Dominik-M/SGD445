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

    private static GameController instance;
    public static GameController I => instance;

    public event Action<int, int> OnScoreChanged;

    private int score;
    private float remainingTime;
    private bool gameover, gamecomplete;
    private GameObject player;
    private GameObject currentLevel;
    private int currentLevelIdx;
    private Transform respawn;
    private Transform finish;
    private FollowerCamera followerCamera;
    private AudioSource audioSource;

    public int CurrentLevelIndex => currentLevelIdx;
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
        instance = this;
        gameover = false;
        gamecomplete = false;
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
        PlaySound(LooseSound);
        if (player != null)
        {
            if (PlayerDestroyEffectPrefab) Instantiate(PlayerDestroyEffectPrefab, player.transform.position, Quaternion.identity);
            Destroy(player);
        }
    }

    public void Finish()
    {
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
            if (remainingTime > 2)
                Invoke(nameof(Respawn), 2);
        }
        else if (trigger.CompareTag("Finish"))
            Finish();
        else if (trigger.CompareTag("Pickup"))
        {
            PlaySound(PickupSound);
            Destroy(trigger.gameObject);
            Score++;
        }
    }
}