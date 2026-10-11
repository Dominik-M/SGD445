using UnityEngine;

namespace Shooter
{
    public class Spieler : MonoBehaviour
    {
        public static Spieler Instance { get; private set; }

        public DamageController Leben { get; private set; }

        void Awake()
        {
            Instance = this;
            Leben = GetComponent<DamageController>();
            if (Leben == null)
            {
                Debug.LogError("Kein DamageController am Spieler Objekt");
                return;
            }
            Leben.OnDeath += SpielerTod;
        }

        void OnDestroy()
        {
            if (Instance == this) Instance = null;
        }

        void SpielerTod()
        {
            // Steuerung abschalten - Nicht zerstören, da Referenzen hierauf vorhanden
            // Kamera vorher loslösen
            Camera.main.transform.SetParent(transform.root.parent);
            gameObject.SetActive(false);
        }
    }
}