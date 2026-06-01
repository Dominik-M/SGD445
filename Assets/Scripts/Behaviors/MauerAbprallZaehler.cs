using UnityEngine;

public class MauerAbprallZaehler : MonoBehaviour
{
    public float Speed;
    public int MaxAbpraller = 20;
    public TMPro.TextMeshProUGUI Text;

    private Vector3 direction = Vector3.right;
    private int counter = 0;
    private bool hatWandKontakt = false;
    void FixedUpdate()
    {
        if (counter < MaxAbpraller) transform.Translate(direction * Speed * Time.fixedDeltaTime);
        if (Text) Text.text = "Durchlauf " + (counter + 1).ToString();
    }

    private void OnTriggerEnter()
    {
        if (hatWandKontakt) return; // verhindere mehrfaches Auslösen
        hatWandKontakt = true;
        direction = -direction;
        counter++;
        Debug.Log("Abprall Nummer " + counter);
    }
    private void OnTriggerExit()
    {
        hatWandKontakt = false;
    }
}
