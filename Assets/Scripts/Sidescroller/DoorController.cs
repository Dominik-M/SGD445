using UnityEngine;

public class DoorController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform door;
    [Header("Parameters")]
    [SerializeField] private Vector3 doorOpenRotation = new Vector3(0, 90, 0);
    [SerializeField] private Vector3 doorClosedRotation = Vector3.zero;
    [SerializeField] private Vector3 doorOpenPosition = Vector3.zero;
    [SerializeField] private Vector3 doorClosedPosition = Vector3.zero;
    [SerializeField] private float slerpSpeed = 5f;

    private bool open = false;

    public bool Open
    {
        get => open; set
        {
            if (open != value)
            {
                open = value;
                HandleDoorOpenChanged();
            }
        }
    }

    public void Toggle() => Open = !Open;

    private void HandleDoorOpenChanged()
    {
        if (Open)
        {
            Debug.Log("Door Opened");
        }
        else
        {
            Debug.Log("Door Closed");
        }
    }

    void Update()
    {
        if (door == null) return;

        if (Open)
        {
            SlerpPositionAndRotation(door, doorOpenPosition, Quaternion.Euler(doorOpenRotation), Time.deltaTime * slerpSpeed);
        }
        else
        {
            SlerpPositionAndRotation(door, doorClosedPosition, Quaternion.Euler(doorClosedRotation), Time.deltaTime * slerpSpeed);
        }
    }

    public static void SlerpPositionAndRotation(Transform target, Vector3 targetPos, Quaternion targetRot, float dt)
    {
        Vector3 currentPos = target.localPosition;
        Quaternion currentRot = target.localRotation;
        target.localPosition = Vector3.Lerp(currentPos, targetPos, dt);
        target.localRotation = Quaternion.Slerp(currentRot, targetRot, dt);
    }
}
