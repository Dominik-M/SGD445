using UnityEngine;

public class Tooltip : MonoBehaviour
{
    private static Tooltip instance;

    [Header("Refrenzen & Setup")]
    [SerializeField] private TMPro.TextMeshProUGUI text;

    private void Awake()
    {
        instance = this;
        Hide();
    }

    public static void Hide()
    {
        if (instance != null)
        {
            instance.gameObject.SetActive(false);
        }
    }

    public static void Show(string text)
    {
        if (instance != null)
        {
            instance.text.text = text;
            instance.gameObject.SetActive(true);
        }
    }
}
