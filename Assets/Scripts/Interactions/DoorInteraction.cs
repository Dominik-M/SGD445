using PointClick;
using UnityEngine;

public class DoorInteraction : InteractionBehavior, IInteractable
{
    public DoorController Door;

    public void OnCursorEnter()
    {
        Tooltip.Show(Text);
    }

    public void OnCursorExit()
    {
        Tooltip.Hide();
    }

    public override void OnInteract()
    {
        if (Door != null)
        {
            Door.Open = !Door.Open;
        }
    }
}
