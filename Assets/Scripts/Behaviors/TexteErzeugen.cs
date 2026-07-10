using UnityEngine;
using UnityEngine.UI;

public class TexteErzeugen : MonoBehaviour
{
    [SerializeField] private int AnzahlTexte = 3;

    void Start()
    {
        Canvas canvas = FindAnyObjectByType<Canvas>();
        if (canvas == null)
        {
            canvas = new GameObject("Canvas").AddComponent<Canvas>();
        }
        GameObject parentObject = new GameObject("Texte");
        parentObject.transform.SetParent(canvas.transform, false);
        parentObject.AddComponent<HorizontalLayoutGroup>();
        for (int i = 0; i < AnzahlTexte; i++)
        {
            var obj = new GameObject("Text_" + i);
            obj.transform.SetParent(parentObject.transform, false);
            var txt = obj.AddComponent<TMPro.TextMeshProUGUI>();
            txt.text = i.ToString();
        }
    }
}
