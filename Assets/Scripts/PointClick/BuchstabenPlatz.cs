using PointClick;
using UnityEngine;

public class BuchstabenPlatz : MonoBehaviour, IInteractable
{
    private Gegenstand gegenstand;
    private GameObject gegenstandObjekt;
    public Gegenstand Gegenstand
    {
        get => gegenstand;
        set
        {
            gegenstand = value;
            if (gegenstandObjekt != null)
                Destroy(gegenstandObjekt);
            if (value != null)
            {
                gegenstandObjekt = Instantiate(value.prefab, transform);
                gegenstandObjekt.transform.localPosition = new Vector3(0, 1, 0);
                gegenstandObjekt.transform.localRotation = Quaternion.identity;
                var textSetter = gegenstandObjekt.GetComponent<BuchstabenTextSetter>();
                if (textSetter != null) textSetter.Text = value.bedingung;
            }
        }
    }
    private bool geloest = false;

    public void OnCursorEnter()
    {
        if (geloest) return;
        if (Gegenstand != null)
            Tooltip.Show($"Buchstaben \"{Gegenstand.bedingung}\" entnehmen");
        else
            Tooltip.Show("Platziere einen Buchstaben");
    }

    public void OnCursorExit()
    {
        Tooltip.Hide();
    }

    public void OnInteract()
    {
        if (geloest) return;

        if (Gegenstand != null)
        {
            // Buchstaben nehmen
            if (Inventar.Add(Gegenstand))
            {
                PopupManager.ShowInfo(Gegenstand.name + " genommen");
                Gegenstand = null;
            }
        }
        else
        {
            // Buchstaben platzieren
            Gegenstand gehalten = Inventar.GetGegenstandInHand();
            if (gehalten == null)
            {
                PopupManager.ShowWarning("Kein Gegenstand in der Hand");
                return;
            }
            // Passt dar Gegenstand hier?
            if (PruefeGegenstand(gehalten.bedingung))
            {
                // Verbrauche den Gegenstand
                if (Inventar.Remove(gehalten.name, 1))
                {
                    PopupManager.ShowInfo(gehalten + " wurde platziert.");
                    Gegenstand = gehalten;
                }
            }
            else
                PopupManager.ShowWarning("Sie können den Gegenstand " + gehalten + " hier nicht anwenden.");
        }
    }

    bool PruefeGegenstand(string bedingung)
    {
        // Buchstabengegenstände haben NUR den jeweiligen Buchstaben in der Bedingung, also genau ein Zeichen
        if (string.IsNullOrEmpty(bedingung)) return false;
        return bedingung.Length == 1;
    }

    public bool Vergleiche(string bedingung)
    {
        if (string.IsNullOrEmpty(bedingung)) return false;
        if (Gegenstand == null) return false;
        if (string.IsNullOrEmpty(Gegenstand.bedingung)) return false;
        return Gegenstand.bedingung.Equals(bedingung);
    }

    public void RaetselGeloest()
    {
        geloest = true;
        var textSetter = gegenstandObjekt.GetComponent<BuchstabenTextSetter>();
        if (textSetter != null) textSetter.SetColor(Color.green);
    }
}
