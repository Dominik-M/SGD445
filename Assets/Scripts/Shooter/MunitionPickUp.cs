using UnityEngine;

namespace Shooter
{
    public class MunitionPickUp : MonoBehaviour
    {

        public int munitionsZuwachs = 8;

        public Munition munition;

        private void Start()
        {
            foreach (var tmp in GetComponentsInChildren<TMPro.TextMeshPro>())
                tmp.text = munition.name;
        }

        private void OnTriggerEnter(Collider other)
        {

            if (other.CompareTag("Player"))
            {
                foreach (WeaponController wpn in other.GetComponentsInChildren<WeaponController>(true))
                {
                    if (wpn.MuniTyp == munition.Type && wpn.AddMunition(munitionsZuwachs) > 0)
                    {
                        PopupManager.ShowInfo($"{munitionsZuwachs} {munition.name} Munition eingesammelt");
                        Destroy(gameObject);
                        return;
                    }
                }
            }

        }

    }
}