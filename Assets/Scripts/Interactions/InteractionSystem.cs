
using System.Collections.Generic;
using System.Diagnostics;

public static class InteractionSystem
{
    private static int selectedInteraction;
    private static List<InteractionBehavior> interactablesInRange = new List<InteractionBehavior>();

    public static List<InteractionBehavior> GetInteractablesInRange()
    {
        return interactablesInRange;
    }
    public static void SelectNextInteraction()
    {
        selectedInteraction++;
        if (selectedInteraction >= interactablesInRange.Count || selectedInteraction < 0)
        {
            selectedInteraction = 0;
        }
    }
    public static void SelectPrevInteraction()
    {
        selectedInteraction--;
        if (selectedInteraction >= interactablesInRange.Count || selectedInteraction < 0)
        {
            selectedInteraction = interactablesInRange.Count - 1;
        }
    }

    public static void AddInteractable(InteractionBehavior ic)
    {
        interactablesInRange.Add(ic);
    }

    public static void RemoveInteractable(InteractionBehavior ic)
    {
        interactablesInRange.Remove(ic);
        selectedInteraction = 0;// reset the selection to avoid invalid pointer
    }

    public static InteractionBehavior GetSelectedInteraction()
    {
        if (interactablesInRange != null && selectedInteraction >= 0 && selectedInteraction < interactablesInRange.Count)
            return interactablesInRange[selectedInteraction];
        return null;
    }

    public static void OnInteract()
    {
        var ic = GetSelectedInteraction();
        if (ic != null) ic.OnInteract();
    }
}
