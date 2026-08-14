using UnityEngine;

public class Flackern : MonoBehaviour
{
    public float minIntensity = 0.5f, maxIntensity = 2f;
    public float interval = 0.2f;
    public float changeSpeed = 2f;

    private Light mLight;
    private float timeToChange;
    private float targetIntensity;

    void Start()
    {
        mLight = GetComponent<Light>();
        timeToChange = interval;
        targetIntensity = maxIntensity;
    }

    void Update()
    {
        if (mLight == null) return;

        mLight.intensity = Mathf.Lerp(mLight.intensity, targetIntensity, Time.deltaTime * changeSpeed);

        timeToChange -= Time.deltaTime;
        if (timeToChange < 0)
        {
            timeToChange = interval;
            targetIntensity = Random.Range(minIntensity, maxIntensity);
        }
    }
}
