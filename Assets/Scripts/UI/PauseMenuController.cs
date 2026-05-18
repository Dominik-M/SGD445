using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenuController : MonoBehaviour
{
    [Header("Pause Button")]
    [SerializeField] private InputActionReference StartButton;

    private Transform content;
    private bool paused;
    public bool Paused
    {
        get => paused;
        set
        {
            paused = value;
            Time.timeScale = paused ? 0 : 1;
            content.gameObject.SetActive(paused);
        }
    }

    private void Start()
    {
        content = transform.GetChild(0);
        Paused = false;
    }

    private void Update()
    {
        if (StartButton.action.WasPressedThisFrame()) Toggle();
    }

    public void Toggle()
    {
        Paused = !Paused;
    }

    public void LoadMainMenu()
    {
        Paused = false;// To reset time scale
        SceneManager.LoadScene(0);
    }
}
