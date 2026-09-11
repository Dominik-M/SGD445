using UnityEngine;

namespace PointClick
{
    public class GegenstandAufheben : MonoBehaviour, IInteractable
    {
        [Header("Refrenzen & Setup")]
        public Gegenstand gegenstand;
        public void OnCursorEnter()
        {
            Tooltip.Show(gegenstand.name + " nehmen");
        }

        public void OnCursorExit()
        {
            Tooltip.Hide();
        }

        public void OnInteract()
        {
            if (Inventar.Add(gegenstand))
            {
                PopupManager.ShowInfo(gegenstand.name + " genommen");
                Destroy(gameObject);
            }
        }
    }
}