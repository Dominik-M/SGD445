using UnityEngine;

public class DoorInteraction : InteractionBehavior
{
    public DoorController Door;

    public override void OnInteract()
    {
        if (Door != null)
        {
            Door.Open = !Door.Open;
        }
    }

    void Update()
    {
        if (Door != null) Text = Door.Open ? "Tor schließen" : "Tor öffnen";
    }
}
