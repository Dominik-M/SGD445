using UnityEngine;

namespace Shooter
{
    public class HPText : MonoBehaviour
    {
        [Header("Mein Schadensobjekt")]
        [SerializeField] private DamageController dc;
        [Header("GUI Text")]
        [SerializeField] private TMPro.TextMeshProUGUI tmp;

        void Update()
        {
            if (dc == null || tmp == null) return;
            tmp.text = dc.Hp.ToString("F0");
        }
    }
}