using UnityEngine;

public class CheckPush : MonoBehaviour
{
    public DoorController door;

    private float startY;

    private void Start()
    {
        startY = transform.position.y;
    }

    void Update()
    {
        if (door != null && !door.Open && Mathf.Abs(startY - transform.position.y) > 0.4f)
            door.Open = true;
    }
}
