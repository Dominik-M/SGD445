using UnityEngine;
using UnityEngine.UI;

public class SelectOnEnable : MonoBehaviour
{
    void OnEnable()
    {
        GetComponent<Selectable>().Select();
    }
}
