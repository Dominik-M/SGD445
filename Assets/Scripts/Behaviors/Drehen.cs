using UnityEngine;
using UnityEngine.InputSystem;

public class Drehen : MonoBehaviour
{
    //für die Geschwindigkeit
    public enum Geschwindigkeit { aus, langsam, mittel, schnell };
    //ein Feld für die Geschwindigkeit
    public Geschwindigkeit objektGeschwindigkeit = Geschwindigkeit.langsam;
    //ein privates Feld für die Länge der Enumeration
    readonly int anzahlGeschwindigkeiten = System.Enum.GetValues(typeof(Geschwindigkeit)).Length;

    void Update()
    {
        //wurde die linke Maustaste gedrückt? - Angepasst an das neue InputSystem
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            //ist der neue Wert noch kleiner als die Anzahl der Geschwindigkeiten minus 1
            if ((int)objektGeschwindigkeit < anzahlGeschwindigkeiten - 1)
                objektGeschwindigkeit += 1;
            // Zusatz: Beim Überlauf auf den ersten Wert setzen
            else objektGeschwindigkeit = (Geschwindigkeit)0;
        }
        else
        {
            if (Mouse.current.rightButton.wasPressedThisFrame)
            {
                //ist der neue Wert noch im gültigen Bereich
                if ((int)objektGeschwindigkeit > 0)
                    objektGeschwindigkeit -= 1;
                // Zusatz: Beim Unterlauf auf den letzten Wert setzen
                else objektGeschwindigkeit = (Geschwindigkeit)(anzahlGeschwindigkeiten - 1);
            }
        }
        transform.Rotate((int)objektGeschwindigkeit * 100 * Time.deltaTime, 0, 0);
        Debug.Log(objektGeschwindigkeit);
    }
}