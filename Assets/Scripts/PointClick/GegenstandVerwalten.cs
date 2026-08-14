using UnityEngine;

public class GegenstandVerwalten : MonoBehaviour, IInteractable
{
    [Header("Refrenzen & Setup")]
    [SerializeField] private Gegenstand gegenstand;
    public void OnCursorEnter()
    {
        Tooltip.Show(gegenstand.ToString());
    }

    public void OnCursorExit()
    {
        Tooltip.Hide();
    }

    public void OnInteract()
    {
        if (Inventar.Add(gegenstand))
        {
            PopupManager.ShowInfo(gegenstand + " genommen");
            Destroy(gameObject);
        }
    }
}
