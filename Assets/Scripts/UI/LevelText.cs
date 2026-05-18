using UnityEngine;

public class LevelText : MonoBehaviour
{
    private TMPro.TextMeshProUGUI text;
    void Start()
    {
        text = GetComponent<TMPro.TextMeshProUGUI>();
    }

    void Update()
    {
        var level = GameController.I.CurrentLevelIndex + 1;
        text.text = "Level " + level.ToString("F0");
    }
}
