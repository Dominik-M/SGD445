using UnityEngine;

namespace PointClick
{
    public class GegenstandAnwenden : MonoBehaviour, IInteractable
    {
        public Gegenstand gegenstand;
        public string bedingung;
        public int anzahl;
        public int zustand;

        public void OnCursorEnter()
        {
            Tooltip.Show("Braucht " + bedingung);
        }

        public void OnCursorExit()
        {
            Tooltip.Hide();
        }

        public void OnInteract()
        {
            Gegenstand gehalten = Inventar.GehaltenerGegenstand;
            // Passt dar Gegenstand hier?
            if (Inventar.PruefeGegenstand(bedingung, 1))
            {
                // Verbrauche den Gegenstand
                if (Inventar.Remove(gehalten.name, 1))
                {
                    zustand++;
                    PopupManager.ShowInfo("Der Gegenstand " + gehalten + " wurde angewendet.");
                }
            }
            else
                PopupManager.ShowWarning("Sie können den Gegenstand " + gehalten + " hier nicht anwenden.");
        }
    }
}