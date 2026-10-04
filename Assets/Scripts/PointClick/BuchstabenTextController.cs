using UnityEngine;
namespace PointClick
{
    public class BuchstabenTextController : MonoBehaviour
    {
        public string Text;
        public void SetColor(Color color)
        {
            GetComponent<TMPro.TextMeshPro>().color = color;
        }

        void Start()
        {
            // Falls kein Script vorhanden, nimm vordefinierten Text
            var script = GetComponent<GegenstandAufheben>();
            if (script != null)
                GetComponent<TMPro.TextMeshPro>().text = script.gegenstand.bedingung;
            else
                GetComponent<TMPro.TextMeshPro>().text = Text;
        }
    }
}