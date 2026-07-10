using System.Collections;
using TMPro;
using UnityEngine;

public class GameOverScreenController : MonoBehaviour
{
    [SerializeField] private GameObject gameUI;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI yourScoreText;
    [SerializeField] private GameObject highscores;

    private void Start()
    {
        if (titleText != null)
            titleText.gameObject.SetActive(false);
        if (yourScoreText != null)
            yourScoreText.gameObject.SetActive(false);
        if (highscores != null)
            highscores.gameObject.SetActive(false);

    }

    private void OnEnable()
    {
        GameController.I.OnGameOver += OnGameOver;
    }

    private void OnDisable()
    {
        GameController.I.OnGameOver -= OnGameOver;
    }

    void OnGameOver()
    {
        StartCoroutine(GameOverSequence());
    }

    IEnumerator GameOverSequence()
    {
        if (gameUI != null) gameUI.SetActive(false);
        ShowTitleText();
        yield return new WaitForSeconds(1);
        ShowScoreText();
        yield return new WaitForSeconds(1);
        ShowHighscores();
    }

    void ShowTitleText()
    {
        if (titleText == null) return;

        titleText.gameObject.SetActive(true);
        titleText.text = GameController.I.IsGameComplete ? "Game Complete!" : "Game Over";
    }

    void ShowScoreText()
    {
        if (yourScoreText == null) return;

        yourScoreText.gameObject.SetActive(true);
        yourScoreText.text = "Deine Punktzahl: " + GameController.I.Score;
    }

    void ShowHighscores()
    {

        if (highscores == null) return;

        // Clear list
        foreach (Transform child in highscores.transform) Destroy(child.gameObject);

        // Title row
        AddHighscoreText("TitleName", "Name", 24);
        AddHighscoreText("TitleScore", "Punkte", 24);
        for (int i = 0; i < SaveData.MAX_HIGHSCORE_ENTRIES; i++)
        {
            var item = GameController.I.GetHighscoreItem(i);
            if (item != null && !string.IsNullOrEmpty(item.name))
            {
                AddHighscoreText("Name" + i, item.name, 30);
                AddHighscoreText("Score" + i, item.score.ToString(), 30);
            }
            else
            {
                AddHighscoreText("Name" + i, "----", 30);
                AddHighscoreText("Score" + i, "----", 30);
            }
        }

        highscores.gameObject.SetActive(true);
    }

    TextMeshProUGUI AddHighscoreText(string objName, string text, int size)
    {
        GameObject obj = new GameObject(objName);
        obj.transform.SetParent(highscores.transform, false);
        var tmp = obj.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = size;
        return tmp;
    }
}
