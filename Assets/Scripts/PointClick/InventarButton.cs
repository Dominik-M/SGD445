using UnityEngine;
using UnityEngine.UI;

public class InventarButton : MonoBehaviour
{
    [Header("Refrenzen & Setup")]
    [SerializeField] private TMPro.TextMeshProUGUI text;
    [SerializeField] private Button button;

    private Gegenstand ggst;

    public Gegenstand Ggst
    {
        get => ggst;
        set
        {
            ggst = value;
            UpdateText();
        }
    }

    void Start()
    {
        UpdateText();
        button.onClick.AddListener(OnClick);
    }

    void UpdateText()
    {
        if (Ggst == null)
        {
            text.text = "Leer";
        }
        else
        {
            text.text = Ggst.ToString();
        }
    }

    void OnClick()
    {
        if (string.IsNullOrEmpty(Inventar.GehaltenerGegenstand))
        {
            if (Ggst != null)
            {
                // Gegenstand aufnehmen
                Inventar.GehaltenerGegenstand = Ggst.name;
                // Falls gehaltener Gegenstand wie ein eigener Inventar Slot ist, entferne ihn beim Aufnehmen
                //Inventar.Remove(Ggst.name);
            }
        }
        else
        {
            if (Ggst == null)
            {
                // Leerer Slot, lege Gegenstand ab
                //if (Inventar.Add(Inventar.GehaltenerGegenstand))
                    Inventar.GehaltenerGegenstand = null;
            }
            else
            {
                if (Ggst.name.Equals(Inventar.GehaltenerGegenstand))
                {
                    // Gleicher Gegenstand, auf Stapel legen
                    //if (Inventar.Add(Ggst.name))
                        Inventar.GehaltenerGegenstand = null;
                }
                else
                {
                    // Kombinieren
                    PopupManager.ShowInfo("Kombiniere " + Ggst + " mit " + Inventar.GehaltenerGegenstand);
                    // TODO
                }
            }
        }
    }
}
