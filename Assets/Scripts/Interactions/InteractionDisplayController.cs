using UnityEngine;

public class InteractionDisplayController : MonoBehaviour
{
    [Header("Background (Parent) of the display")]
    [SerializeField] private GameObject background;
    [Header("Display Text")]
    [SerializeField] private TMPro.TextMeshProUGUI text;


    void Update()
    {
        var current = InteractionSystem.GetSelectedInteraction();
        if(current != null)
        {
            text.text = current.ToString();
            background.SetActive(true);
        }
        else
        {
            background.SetActive(false);
        }
    }
}
