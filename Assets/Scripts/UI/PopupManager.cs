using UnityEngine;

public class PopupManager : MonoBehaviour
{
    enum MessageType { INFO, WARNING, ERROR }

    [Header("Refrenzen & Setup")]
    [SerializeField] private Popup infoPopup;
    [SerializeField] private Popup warningPopup;
    [SerializeField] private Popup errorPopup;

    private static PopupManager instance;
    private void Awake()
    {
        instance = this;
    }

    void Show(MessageType type, string text)
    {
        Popup popup;
        if (type == MessageType.INFO)
            popup = Instantiate(instance.infoPopup, instance.transform, false).GetComponent<Popup>();
        else if (type == MessageType.WARNING)
            popup = Instantiate(instance.warningPopup, instance.transform, false).GetComponent<Popup>();
        else
            popup = Instantiate(instance.errorPopup, instance.transform, false).GetComponent<Popup>();
        popup.Titletext = text;
        popup.Show();
    }

    public static void ShowInfo(string text)
    {
        if (instance == null) return;
        instance.Show(MessageType.INFO, text);
    }

    public static void ShowWarning(string text)
    {
        if (instance == null) return;
        instance.Show(MessageType.WARNING, text);
    }

    public static void ShowError(string text)
    {
        if (instance == null) return;
        instance.Show(MessageType.ERROR, text);
    }
}
