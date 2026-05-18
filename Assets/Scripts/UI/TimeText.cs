using UnityEngine;

public class TimeText : MonoBehaviour
{
    private TMPro.TextMeshProUGUI text;
    void Start()
    {
        text = GetComponent<TMPro.TextMeshProUGUI>();
    }

    void Update()
    {
        float time = GameController.I.RemainingTime;
        text.text = time.ToString("F1");
        text.color = time > 20 ? Color.white : Color.red;
    }
}
