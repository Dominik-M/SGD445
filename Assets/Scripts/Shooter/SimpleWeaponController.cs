using UnityEngine;

namespace Shooter
{
    public class SimpleWeaponController : WeaponController
    {
        // ─── Inspector Fields ─────────────────────────────────────────────────────
        [Header("References")]
        [SerializeField] GameObject shotPrefab;
        public override void Shoot()
        {
            // Hier wird nur das Objekt erzeugt und ggf. der Schaden berechnet
            // alles andere soll das neue Objekt selbst machen
            GameObject instance = Instantiate(shotPrefab, transform.position, transform.rotation);
            instance.tag = tag;
            var projectile = instance.GetComponent<Projectile>();
            if (projectile != null)
                projectile.Damage *= Damage;
        }
    }
}