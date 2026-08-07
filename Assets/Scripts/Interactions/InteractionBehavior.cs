using UnityEngine;

public abstract class InteractionBehavior : MonoBehaviour
{
    public string Text;
    public Sprite Icon;
    private bool active = true;

    public bool Active
    {
        get => active; set
        {
            active = value;
            if (!active)
                InteractionSystem.RemoveInteractable(this);
        }
    }

    public override string ToString()
    {
        return Text;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && active)
        {
            Debug.Log("Player got in contact with " + this);
            InteractionSystem.AddInteractable(this);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player no longer in contact with " + this);
            InteractionSystem.RemoveInteractable(this);
        }
    }

    public abstract void OnInteract();
}
