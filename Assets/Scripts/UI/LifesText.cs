using UnityEngine;

public class LifesText : MonoBehaviour
{
    private TMPro.TextMeshProUGUI text;
    void Start()
    {
        text = GetComponent<TMPro.TextMeshProUGUI>();
    }

    void Update()
    {
        var value = GameController.I.LifesRemaining;
        text.text = value.ToString("F0");
    }
}
