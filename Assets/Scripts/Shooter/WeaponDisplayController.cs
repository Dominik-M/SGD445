using UnityEngine;
using UnityEngine.UI;

namespace Shooter
{
    public class WeaponDisplayController : MonoBehaviour
    {
        [Header("Meine Waffensteuerung")]
        [SerializeField] private PlayerWeaponController playerWeaponController;

        [Header("Ladezustand der Waffe")]
        [SerializeField] private Image status;

        [Header("Text für Munition")]
        [SerializeField] private TMPro.TextMeshProUGUI valueTmp;

        void Update()
        {
            if (playerWeaponController == null) return;
            var wpn = playerWeaponController.AktuelleWaffe;
            if (wpn == null) return;

            if (valueTmp != null)
            {
                // Ammo Text
                valueTmp.text = wpn.Ammo.ToString() + " / " + wpn.MaxAmmo.ToString();
                if (wpn.Ammo >= wpn.MaxAmmo)
                    valueTmp.color = Color.yellow;
                else if (wpn.Ammo <= wpn.MaxAmmo / 10)
                    valueTmp.color = Color.red;
                else
                    valueTmp.color = Color.white;
            }

            if (status != null)
            {
                // Reload state
                float remain = wpn.ReloadTime / wpn.ReloadTimeMax;
                status.fillAmount = (1 - remain);
                if (wpn.Ammo <= 0 && wpn.Firing)
                    status.color = Color.red;
                else if (remain <= 0.01)
                    status.color = Color.greenYellow;
                else status.color = Color.gray9;
            }
        }
    }
}