using UnityEngine;

namespace PointClick
{
    public class GegenstandAnwenden : MonoBehaviour, IInteractable
    {
        public event System.Action<int> ZustandGeandert;
        public string bedingung;
        public int anzahl;
        private int zustand;
        public int Zustand
        {
            get => zustand; set
            {
                zustand = value;
                ZustandGeandert?.Invoke(zustand);
            }
        }
        public int Endzustand = 1;
        public string SaveId => name;

        public void OnCursorEnter()
        {
            if (zustand < Endzustand)
                Tooltip.Show("Braucht " + bedingung);
        }

        public void OnCursorExit()
        {
            Tooltip.Hide();
        }

        public void OnInteract()
        {
            if (zustand >= Endzustand)
                return;

            if (string.IsNullOrEmpty(bedingung))
            {
                // Keine Bedingung, direkt Zustand ändern
                Zustand++;
                return;
            }

            Gegenstand gehalten = Inventar.GetGegenstandInHand();
            if (gehalten == null)
            {
                PopupManager.ShowWarning("Kein Gegenstand in der Hand");
                return;
            }
            // Passt dar Gegenstand hier?
            if (Inventar.PruefeGegenstand(bedingung, anzahl))
            {
                // Verbrauche den Gegenstand
                if (Inventar.Remove(gehalten.name, anzahl))
                {
                    Zustand++;
                    PopupManager.ShowInfo("Der Gegenstand " + gehalten + " wurde angewendet.");
                }
            }
            else
                PopupManager.ShowWarning("Sie können den Gegenstand " + gehalten + " hier nicht anwenden.");
        }
    }
}