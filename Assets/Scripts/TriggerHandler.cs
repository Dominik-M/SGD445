using System;
using UnityEngine;

public class TriggerHandler : MonoBehaviour
{
    public event Action<GameObject,GameObject> OnEnter;

    private void OnTriggerEnter(Collider other)
    {
        OnEnter?.Invoke(gameObject, other.gameObject);
    }
}
