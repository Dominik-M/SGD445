using UnityEngine;

public class ToggleOnOff : MonoBehaviour
{
    [Header("Zeit zwischen Ein- und Ausschalten in Sekunden")]
    [SerializeField] private float cycleTime = 2;

    private float t = 0;
    private Collider _collider;
    private Renderer _renderer;

    private void Start()
    {
        _collider = GetComponent<Collider>();
        _renderer = GetComponent<Renderer>();
    }

    void Update()
    {
        t += Time.deltaTime;
        if (t >= cycleTime)
        {
            Toggle();
            t = 0;
        }
    }

    void Toggle()
    {
        _collider.enabled = !_collider.enabled;
        _renderer.enabled = !_renderer.enabled;
    }
}
