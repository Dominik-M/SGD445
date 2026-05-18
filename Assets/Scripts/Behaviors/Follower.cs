using UnityEngine;

public class Follower : MonoBehaviour
{
    // Inspector Felder
    [Header("Das zu verfolgende Objekt")]
    [SerializeField] private GameObject target;
    [Header("Abstand zum verfolgten Objekt auf X und Y Achse")]
    [SerializeField] private float distanceX;
    [SerializeField] private float distanceY;

    void Update()
    {
        Vector3 offset = new Vector3(distanceX, distanceY, 0);
        transform.position = target.transform.position + offset;
    }
}
