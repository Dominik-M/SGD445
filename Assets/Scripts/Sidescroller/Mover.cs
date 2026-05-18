using UnityEngine;

public class Mover : MonoBehaviour
{
    [Header("Bewegungsgeschwindigkeit")]
    [SerializeField] private float speed = 5;
    [SerializeField] private Vector3 direction = new Vector3(1, 0, 0);
    [Header("Distanz nach der die Richtung wechselt")]
    [SerializeField] private float toggleDistance = 10;

    private float movedDistance = 0;

    void Update()
    {
        Vector3 delta = speed * Time.deltaTime * direction;
        transform.Translate(delta);
        movedDistance += delta.magnitude;
        if (movedDistance > toggleDistance)
        {
            direction *= -1;
            movedDistance = 0;
        }
    }
}
