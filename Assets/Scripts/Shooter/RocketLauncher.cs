using UnityEngine;

namespace Shooter
{
    public class RocketLauncher : WeaponController
    {
        // ─── Inspector Fields ─────────────────────────────────────────────────────
        [Header("References")]
        [SerializeField] GameObject shotPrefab;
        public override void Shoot()
        {

            foreach (Transform gunpoint in gunpoints)
            {
                GameObject instance = Instantiate(shotPrefab, gunpoint.position, gunpoint.rotation);
                instance.tag = tag;
                var projectile = instance.GetComponent<Projectile>();
                if (projectile != null)
                    projectile.Damage *= Damage;
                var missile = instance.GetComponent<MissileController>();
                if (missile != null)
                {
                    // Ziel setzen
                    RaycastHit treffer;
                    if (Physics.Raycast(transform.position, transform.forward, out treffer))
                    {
                        if (treffer.transform.CompareTag("enemy"))
                            missile.Target = treffer.transform;
                    }
                }
            }
        }
    }
}