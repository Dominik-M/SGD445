using UnityEngine;

public class DoorTrigger : MonoBehaviour
{
    [SerializeField] private DoorController door;
    [SerializeField] private bool closeOnExit;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            door.Open = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && closeOnExit)
            door.Open = false;
    }
}
