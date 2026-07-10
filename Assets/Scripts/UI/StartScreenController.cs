using UnityEngine;

public class StartScreenController : MonoBehaviour
{
    [SerializeField] private TMPro.TMP_InputField nameInput;
    [SerializeField] private GameObject hint;
    [SerializeField] private GameObject gameUI;
    void Start()
    {
        GameController.I.PlayerName = PlayerPrefs.GetString("Name");
        nameInput.text = GameController.I.PlayerName;
        hint.SetActive(false);
        gameUI.SetActive(false);
        Time.timeScale = 0f;
    }
    public void OnStartButtonPressed()
    {
        string name = nameInput.text;
        if (string.IsNullOrEmpty(name))
        {
            hint.SetActive(true);
            return;
        }
        GameController.I.PlayerName = name;
        PlayerPrefs.SetString("Name", name);
        PlayerPrefs.Save();
        gameObject.SetActive(false);
        gameUI.SetActive(true);
        Time.timeScale = 1f;
    }
}
