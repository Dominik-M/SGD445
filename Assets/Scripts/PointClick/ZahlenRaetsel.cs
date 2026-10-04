using UnityEngine;

public class ZahlenRaetsel : MonoBehaviour
{
    [Header("Referenz auf das Tor")]
    [SerializeField] private DoorController door;
    [Header("Referenz auf den Anzeigetext")]
    [SerializeField] private TMPro.TextMeshPro display;
    [Header("Anzahl der Nummern im Code (1-8)")]
    [SerializeField] private int maxNumbers;
    [Header("Anzahl der maximalen Versuche")]
    [SerializeField] private int maxTries;
    [Header("Sounds")]
    [SerializeField] private AudioClip soundEnter;
    [SerializeField] private AudioClip soundWrong;
    [SerializeField] private AudioClip soundRight;
    private string enteredNumbers = "";
    private string correctCode;
    private int triesRemain;
    private AudioSource audioSource;

    void Start()
    {
        if (door == null || display == null)
        {
            Debug.LogWarning("ZahlenRaetsel: Tor oder Display Referenzen fehlen!");
            return;
        }
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();
        door.Open = false;
        display.fontSize = 2;
        display.color = Color.white;
        display.text = $"Bitte Code eingeben\r\n{maxNumbers} Ziffern\r\n{maxTries} Versuche";

        // Zuerst prüfen ob maxNumbers im gültigen Bereich ist
        if (maxNumbers <= 0 || maxNumbers > 8)
        {
            Debug.LogWarning("Ungültige Anzahl Nummern für den Code");
            return;
        }
        // Neuen Code auswürfeln
        correctCode = "";
        for (int i = 0; i < maxNumbers; i++)
        {
            int number = Random.Range(0, 10);
            correctCode += number.ToString();
        }
        Debug.Log("Der Code ist: " + correctCode);
        triesRemain = maxTries;
    }

    public void EnterNumber(int number)
    {
        // Abbrechen, wenn keine Versuche mehr
        if (triesRemain <= 0) return;
        audioSource.PlayOneShot(soundEnter);
        if (enteredNumbers.Length < maxNumbers)
            enteredNumbers += number.ToString();
        // Andere Schriftgröße für kurze Texte
        display.fontSize = 4;
        display.color = Color.white;
        display.text = enteredNumbers;
    }

    public void Check()
    {
        // Abbrechen, wenn keine Versuche mehr
        if (triesRemain <= 0) return;
        // Eingabe noch nicht fertig
        if (enteredNumbers.Length < maxNumbers) return;

        // Versuch verbrauchen
        triesRemain--;

        // Andere Schriftgröße für lange Texte
        display.fontSize = 2;

        // Ziffernfolgen in Zahlen umwandeln zum Vergleichen
        int myNumber = int.Parse(enteredNumbers);
        int correctNumber = int.Parse(correctCode);
        if (myNumber < correctNumber)
        {
            display.text = $"FALSCHER CODE\r\nZahl zu klein\r\nNoch {triesRemain} Versuche";
        }
        else if (myNumber > correctNumber)
        {
            display.text = $"FALSCHER CODE\r\nZahl zu groß\r\nNoch {triesRemain} Versuche";
        }
        else
        {
            // Richtiger Code
            audioSource.PlayOneShot(soundRight);
            door.Open = true;
            display.color = Color.green;
            display.text = enteredNumbers + "\r\nKorrekter Code\r\nSchloss entsperrt";

            // Versuche auf 0 setzen um weiere Eingabe zu verhindern
            triesRemain = 0;
            return;
        }
        audioSource.PlayOneShot(soundWrong);
        enteredNumbers = "";
        display.color = Color.red;
        if (triesRemain <= 0) display.text = "Zu viele Fehlversuche\r\nSCHLOSS GESPERRT";
    }
}
