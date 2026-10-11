using UnityEngine;

namespace Shooter
{
    public class HealthPickUp : MonoBehaviour
    {

        public int lebensenergieBonus = 5;

        private void OnTriggerEnter(Collider other)
        {

            if (other.CompareTag("Player"))
            {
                DamageController dc = other.GetComponent<DamageController>();
                if (dc != null)
                {
                    float hpVorher = dc.Hp;
                    // Da Hp selbst schreibgeschützt ist, fügen wir negativen Schaden zu zum Heilen
                    dc.Damage(-1 * lebensenergieBonus);
                    // Nur zerstören wenn tatsächlich hp geändert wurden
                    if (dc.Hp != hpVorher)
                        Destroy(gameObject);
                }
            }

        }

    }
}