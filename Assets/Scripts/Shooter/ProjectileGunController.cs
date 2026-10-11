using UnityEngine;

namespace Shooter
{
    public class ProjectileGunController : WeaponController
    {
        // ─── Inspector Fields ─────────────────────────────────────────────────────
        [Header("References")]
        [SerializeField] GameObject shotPrefab;
        public override void Shoot()
        {
            foreach (Transform gunpoint in gunpoints)
            {
                GameObject instance = Instantiate(shotPrefab);
                instance.tag = tag;
                Quaternion spreadRotation = Quaternion.Euler(
                    Random.Range(-SpreadAngle, SpreadAngle),
                    Random.Range(-SpreadAngle, SpreadAngle),
                    0);
                Quaternion finalRotation = gunpoint.rotation * spreadRotation;

                instance.transform.SetPositionAndRotation(
                    gunpoint.position + gunpoint.forward * 0.25f,
                    finalRotation);
                var projectile = instance.GetComponent<Projectile>();
                projectile.Damage *= Damage;
            }
        }
    }
}