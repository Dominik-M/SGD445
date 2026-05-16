using UnityEngine;

public class FollowerCamera : MonoBehaviour
{
    public Transform Target;
    public float Speed = 5;
    public Vector3 Offset = new Vector3(0, 0, -10);
    public float MinX = -5, MinY = -5, MaxX = 20, MaxY = 5;

    void FixedUpdate()
    {
        if (Target == null) return;

        Vector3 targetPos = Target.position + Offset;

        targetPos = ClampToBounds(targetPos);

        transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * Speed);
    }

    Vector3 ClampToBounds(Vector3 pos)
    {
        pos.x = Mathf.Clamp(pos.x, MinX, MaxX);
        pos.y = Mathf.Clamp(pos.y, MinY, MaxY);
        return pos;
    }
}
