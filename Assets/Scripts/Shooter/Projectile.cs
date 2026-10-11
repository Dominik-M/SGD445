using UnityEngine;
using UnityEngine.InputSystem.HID;

namespace Shooter
{
    public class Projectile : MonoBehaviour
    {
        public float Speed;
        public float Damage;
        public GameObject Explosion;
        public GameObject hitEffect;
        public float ExplosionScale = 1.0f;
        public float Area = 0;
        public float Lifetime = 1;
        public bool hasHit = false;

        void Start()
        {
            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb == null) rb = gameObject.AddComponent<Rigidbody>();
            rb.linearVelocity += transform.forward * Speed;
            Destroy(gameObject, Lifetime);
        }

        private void OnDisable()
        {
            Explode();
        }

        void OnCollisionEnter(Collision collision)
        {
            if (hasHit) return;

            Debug.Log("Collide with: " + collision.gameObject.name);
            if (hitEffect != null)
            {
                Vector3 pos = collision.GetContact(0).point;
                Debug.Log("Treffer an: " + pos);
                Instantiate(hitEffect, pos, transform.rotation);
            }
            if (Area <= 0)
            {
                TryDamage(collision.gameObject);
            }
            hasHit = true;
            Destroy(gameObject);
        }

        void Explode()
        {
            if (Explosion != null)
            {
                GameObject instance = Instantiate(Explosion, transform.position, Quaternion.identity);
                instance.transform.localScale = new Vector3(ExplosionScale, ExplosionScale, ExplosionScale);
            }
            if (Area > 0)
            {
                foreach (var hit in Physics.OverlapSphere(transform.position, Area))
                    TryDamage(hit.gameObject);
            }
        }

        void TryDamage(GameObject other)
        {
            // Friendly Fire aus
            if (other.CompareTag(gameObject.tag)) return;

            DamageController dc = other.gameObject.GetComponent<DamageController>();
            if (dc == null) dc = other.gameObject.GetComponentInParent<DamageController>();
            if (dc)
                dc.Damage(Damage);
        }
    }
}