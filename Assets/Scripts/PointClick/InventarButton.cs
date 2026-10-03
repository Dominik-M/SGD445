using UnityEngine;
using UnityEngine.UI;

namespace PointClick
{
    public class InventarButton : MonoBehaviour
    {
        [Header("Refrenzen & Setup")]
        [SerializeField] private TMPro.TextMeshProUGUI text;
        [SerializeField] private Button button;
        [SerializeField] private Image icon;

        private Gegenstand ggst;
        public Gegenstand Ggst
        {
            get => ggst;
            set
            {
                ggst = value;
                UpdateIconAndText();
            }
        }
        private int anzahl;
        public int Anzahl
        {
            get => anzahl; set
            {
                anzahl = value;
                UpdateIconAndText();
            }
        }

        void Start()
        {
            UpdateIconAndText();
            button.onClick.AddListener(OnClick);
        }

        void UpdateIconAndText()
        {
            if (Ggst == null)
            {
                text.text = "Leer";
                icon.sprite = null;
            }
            else
            {
                if (Anzahl > 1)
                    text.text = Ggst.ToString() + " x " + Anzahl;
                else
                    text.text = Ggst.ToString();
                icon.sprite = Ggst.icon;
            }
        }

        void OnClick()
        {
            Gegenstand gehaltenerGegenstand = Inventar.GetGegenstandInHand();
            if (gehaltenerGegenstand == null)
            {
                if (Ggst != null)
                {
                    // Gegenstand aufnehmen
                    Inventar.SetGegenstandInHand(Ggst.name);
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
                    Inventar.SetGegenstandInHand(null);
                }
                else
                {
                    if (Ggst.Equals(gehaltenerGegenstand))
                    {
                        // Gleicher Gegenstand, auf Stapel legen
                        //if (Inventar.Add(Ggst.name))
                        Inventar.SetGegenstandInHand(null);
                    }
                    else
                    {
                        // Kombinieren
                        PopupManager.ShowInfo("Kombiniere " + Ggst + " mit " + gehaltenerGegenstand);
                        // TODO
                    }
                }
            }
        }
    }
}