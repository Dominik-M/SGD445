using UnityEngine;

namespace Shooter
{
    public class MissileController : MonoBehaviour
    {
        [Header("References")]
        public Transform Target;

        [Header("Flight Settings")]
        public float speed = 20f;
        public float turnSpeed = 90f;      // Grad pro Sekunde, wie schnell sie einlenken kann
        public float acceleration = 10f;   // wie schnell sie auf Zielgeschwindigkeit beschleunigt

        private Rigidbody rb;
        private float currentSpeed;

        void Start()
        {
            rb = GetComponent<Rigidbody>();
            rb.useGravity = false; // typisch für lenkbare Raketen, sonst kämpft Gravity gegen die Steuerung
        }

        void FixedUpdate()
        {
            if (Target != null)
            {
                SteerTowardsTarget();
            }

            FlyForward();
        }

        void SteerTowardsTarget()
        {
            Vector3 direction = Target.position - transform.position;
            if (direction.sqrMagnitude < 0.0001f) return; // Ziel exakt erreicht, keine Rotation berechnen

            Quaternion targetRotation = Quaternion.LookRotation(direction.normalized);
            Quaternion newRotation = Quaternion.RotateTowards(
                transform.rotation,
                targetRotation,
                turnSpeed * Time.fixedDeltaTime);

            rb.MoveRotation(newRotation);
        }

        void FlyForward()
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, speed, acceleration * Time.fixedDeltaTime);
            rb.linearVelocity = transform.forward * currentSpeed;
        }
    }
}