using UnityEngine;

public class Rotator : MonoBehaviour
{
    [SerializeField] private Vector3 Rotation = new Vector3(0, 180, 0);

    private void Update()
    {
        transform.rotation *= Quaternion.Euler(Rotation * Time.deltaTime);
    }
}
