using UnityEngine;

public class DestroyByTime : MonoBehaviour
{
    [SerializeField] private float lifetime = 1;
    void Start()
    {
        Destroy(gameObject, lifetime);
    }
}
