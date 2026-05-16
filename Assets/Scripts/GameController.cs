using UnityEngine;
using System;
using Unity.VisualScripting;

public class GameController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject PlayerPrefab;
    [SerializeField] private GameObject[] LevelPrefabs;
    [SerializeField] private AudioClip LooseSound;
    [SerializeField] private AudioClip WinSound;

    private static GameController instance;
    public static GameController I => instance;

    public event Action<bool> OnPauseResume;
    public event Action<int, int> OnScoreChanged;

    private bool running = false, gameover = false;
    private int score;
    private GameObject player;
    private GameObject currentLevel;
    private int currentLevelIdx;
    private Transform respawn;
    private Transform finish;
    private FollowerCamera followerCamera;
    private AudioSource audioSource;
    public bool Running
    {
        get => running; set
        {
            if (!gameover && running != value)
            {
                running = value;
                if (!running)
                {
                    Debug.Log("Game Paused");
                }
                else
                {
                    Debug.Log("Game Resumed");
                }
                OnPauseResume?.Invoke(running);
            }
        }
    }
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
        followerCamera = FindAnyObjectByType<FollowerCamera>();
        // ensure audio source
        if (audioSource == null) audioSource = GetComponent<AudioSource>();
        if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();
        StartLevel(0);
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
            followerCamera.MinX = respawn.position.x - 4;
            followerCamera.MaxX = finish.position.x;
            Respawn();
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
        Respawn();
    }

    public void Finish()
    {
        PlaySound(WinSound);
        Destroy(player);
        Invoke(nameof(NextLevel), 1);
    }

    void NextLevel()
    {
        StartLevel(currentLevelIdx + 1);
    }

    void TriggerEnterHook(GameObject trigger, GameObject other)
    {
        if (!other.CompareTag("Player")) return;
        if (trigger.CompareTag("Boundary"))
            Die();
        else if (trigger.CompareTag("Finish"))
            Finish();
    }
}