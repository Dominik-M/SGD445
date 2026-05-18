using UnityEngine;

public class ScoreText : MonoBehaviour
{
    private TMPro.TextMeshProUGUI text;
    void Start()
    {
        text = GetComponent<TMPro.TextMeshProUGUI>();
    }

    void Update()
    {
        var score = GameController.I.Score;
        text.text = score.ToString("F0");
    }
}
