using UnityEngine;

namespace Shooter
{
    public abstract class WeaponController : MonoBehaviour
    {
        // ─── Inspector Fields ─────────────────────────────────────────────────────
        [Header("References")]
        [SerializeField] private ParticleSystem schussEffekt; // Partikeleffekt für den Abschuss
        [SerializeField] private Animator animator;
        [SerializeField] protected Transform[] gunpoints;

        [Header("Munitions Typ")]
        [SerializeField] private Munition munition;
        public Munition.MunitionsTyp MuniTyp => munition.Type;

        [Header("Weapon Stats")]
        [SerializeField] private float damageScale = 1;
        public float Damage => damageScale;
        [SerializeField] private float range = 40;
        public float Range => range;
        [SerializeField] private float shootDelay = 1;
        [SerializeField] private float warmupTime = 0;
        [SerializeField] private float spreadAngle = 2;
        public float SpreadAngle => spreadAngle;

        // ─── Runtime State ────────────────────────────────────────────────────────
        private bool firing = false;
        public bool Firing
        {
            get => firing; set
            {
                if (!firing && value && reloadTimer < warmupTime)
                {
                    reloadTimer = warmupTime;
                    reloadTimerMax = reloadTimer;
                }
                firing = value;
            }
        }
        public Transform Target { get; set; }

        // ─── Private ──────────────────────────────────────────────────────────────
        private AudioSource audioSource;
        private float reloadTimer = 0;
        public float ReloadTime => reloadTimer;
        private float reloadTimerMax = 1;
        public float ReloadTimeMax => reloadTimerMax;
        private int ammo;
        public int MaxAmmo => munition.MaxAmmo;
        public int Ammo => ammo;

        // ═════════════════════════════════════════════════════════════════════════
        // Lifecycle
        // ═════════════════════════════════════════════════════════════════════════

        protected virtual void Start()
        {
            audioSource = GetComponent<AudioSource>();
        }

        protected virtual void Update()
        {
            RotateToTarget();
            HandleFiring();
        }


        // ═════════════════════════════════════════════════════════════════════════
        // Firing
        // ═════════════════════════════════════════════════════════════════════════

        private void RotateToTarget()
        {
            if (Target == null)
                // Geradeaus vom Halter
                transform.localRotation = Quaternion.identity;
            else
                // Richtung Ziel
                transform.rotation = Quaternion.LookRotation(Target.position - transform.position);
        }

        private void HandleFiring()
        {
            if (reloadTimer > 0) reloadTimer -= Time.deltaTime;
            if (!Firing) return;
            if (reloadTimer > 0) return;
            if (!CanFire()) return;

            reloadTimer = shootDelay;
            reloadTimerMax = reloadTimer;
            if (animator != null) animator.SetTrigger("Shoot");
            if (audioSource != null) audioSource.Play();
            if (schussEffekt != null) schussEffekt.Play();

            Shoot();
            ammo--;
        }

        public abstract void Shoot();

        public bool CanFire()
        {
            if (ammo <= 0) return false;
            return true;
        }

        public int AddMunition(int amount)
        {
            int prev = ammo;
            ammo = Mathf.Clamp(amount + ammo, 0, munition.MaxAmmo);
            return ammo - prev;
        }
    }
}