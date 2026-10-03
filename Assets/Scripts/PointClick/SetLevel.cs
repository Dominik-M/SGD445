using UnityEngine;

public class SetLevel : MonoBehaviour
{
    public int level;
    void Start()
    {
        PlayerPrefs.SetInt("Level", level);
    }
}
