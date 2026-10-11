using UnityEngine;
namespace Shooter
{
    public class RayGunController : WeaponController
    {
        [Header("Strahl")]
        [SerializeField] private Material strahlMaterial;
        [SerializeField] private float strahlBreite = 0.05f;
        [SerializeField] private float strahlDauer = 0.05f;

        private LineRenderer[] strahlen;
        private float strahlRestzeit;
        protected override void Start()
        {
            base.Start();
            strahlen = new LineRenderer[gunpoints.Length];
            for (int i = 0; i < strahlen.Length; i++)
            {
                var go = new GameObject("Strahl " + i);
                go.transform.SetParent(transform);

                var lr = go.AddComponent<LineRenderer>();
                lr.useWorldSpace = true;
                lr.positionCount = 2;
                lr.startWidth = lr.endWidth = strahlBreite;
                lr.material = strahlMaterial;
                lr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                lr.enabled = false;
                strahlen[i] = lr;
            }
        }

        protected override void Update()
        {
            base.Update();
            if (strahlRestzeit > 0f)
            {
                strahlRestzeit -= Time.deltaTime;
                if (strahlRestzeit <= 0f)
                    StrahlenAus();
            }
        }
        void StrahlenAus()
        {
            foreach (var lr in strahlen)
                lr.enabled = false;
        }

        public override void Shoot()
        {
            for (int i = 0; i < gunpoints.Length; i++)
            {
                Transform gunpoint = gunpoints[i];

                Quaternion spreadRotation = Quaternion.Euler(
                    Random.Range(-SpreadAngle, SpreadAngle),
                    Random.Range(-SpreadAngle, SpreadAngle), 0);
                Vector3 richtung = (gunpoint.rotation * spreadRotation) * Vector3.forward;

                float strahllaenge = Range;
                if (Physics.Raycast(gunpoint.position, richtung, out RaycastHit treffer, Range))
                {
                    strahllaenge = treffer.distance;
                    if (!treffer.collider.CompareTag(tag))
                        TryDamage(treffer.collider.gameObject);
                }

                // Strahl zeichnen
                strahlen[i].SetPosition(0, gunpoint.position);
                strahlen[i].SetPosition(1, gunpoint.position + richtung * strahllaenge);
                strahlen[i].enabled = true;
            }
            strahlRestzeit = strahlDauer;
        }

        public void TryDamage(GameObject other)
        {
            DamageController dc = other.gameObject.GetComponent<DamageController>();
            if (dc == null) dc = other.gameObject.GetComponentInParent<DamageController>();
            if (dc)
                dc.Damage(Damage);
        }
    }
}