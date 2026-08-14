using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GegenstandAnwenden : MonoBehaviour, IInteractable
{
    public string bedingung;
    public int anzahl;

    public void OnCursorEnter()
    {
        Tooltip.Show("Braucht " + bedingung + (anzahl > 1 ? "x" + anzahl : ""));
    }

    public void OnCursorExit()
    {
        Tooltip.Hide();
    }

    public void OnInteract()
    {
        string gegenstand = Inventar.GehaltenerGegenstand;
        // Passt dar Gegenstand hier?
        if (Inventar.PruefeGegenstand(bedingung, anzahl))
        {
            // Verbrauche den Gegenstand
            if (Inventar.Remove(gegenstand, anzahl))
            {
                PopupManager.ShowInfo("Der Gegenstand " + gegenstand + " wurde angewendet.");
            }
        }
        else
            PopupManager.ShowWarning("Sie können den Gegenstand " + gegenstand + " hier nicht anwenden.");
    }
}
