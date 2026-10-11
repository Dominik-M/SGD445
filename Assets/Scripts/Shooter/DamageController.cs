using System;
using UnityEngine;

namespace Shooter
{
    public class DamageController : MonoBehaviour
    {
        public event Action<float> OnDamage;
        public event Action OnDeath;
        [SerializeField] private float maxHP;

        [Header("Sound clip bei Treffer")]
        [SerializeField] private AudioClip hitSound;
        [Header("Sound clip bei Tod")]
        [SerializeField] private AudioClip deathSound;
        private float hp;
        private AudioSource _audioSource;

        public float MaxHp
        {
            get => maxHP;
            set
            {
                maxHP = value;
                Hp = hp;
            }
        }
        public float Hp
        {
            get => hp;
            private set
            {
                hp = Mathf.Clamp(value, 0, maxHP);
            }
        }

        private void Awake()
        {
            // ensure audio source
            _audioSource = GetComponent<AudioSource>();
            if (_audioSource == null) _audioSource = gameObject.AddComponent<AudioSource>();
        }

        void Start()
        {
            init();
        }

        public void init()
        {
            Hp = maxHP;
            foreach (Collider collider in GetComponentsInChildren<Collider>())
                collider.tag = tag;
        }

        public bool IsDead()
        {
            return Hp <= 0;
        }

        public virtual void Damage(float dmg)
        {
            if (IsDead()) return;

            Hp -= dmg;
            if (IsDead())
            {
                if (deathSound != null) _audioSource.PlayOneShot(deathSound);
                OnDeath?.Invoke();
            }
            else
            {
                if (hitSound != null) _audioSource.PlayOneShot(hitSound);
                OnDamage?.Invoke(dmg);
            }
        }
    }
}