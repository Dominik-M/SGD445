using UnityEngine;

public class DeathsText : MonoBehaviour
{
    private TMPro.TextMeshProUGUI text;
    void Start()
    {
        text = GetComponent<TMPro.TextMeshProUGUI>();
    }

    void Update()
    {
        var value = GameController.I.DeathCounter;
        text.text = value.ToString("F0");
    }
}
