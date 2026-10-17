using UnityEngine;
using UnityEngine.AI;

namespace Shooter
{
    public class GegnerKI : MonoBehaviour
    {
        [System.Serializable]
        public enum State { IDLE, AGGRO, DEAD }

        [Header("Meine Waffe")]
        [SerializeField] private WeaponController wpn;

        [Header("Abstand zum Spieler ab dem angegriffen wird")]
        [SerializeField] private float aggroRange = 20;
        [Header("Mein Animations Controller")]
        [SerializeField] private Animator animator;
        [Header("Parameter für das Herumlaufen")]
        [SerializeField] private float wanderRadius = 8f;
        [SerializeField] private Vector2 wanderPause = new Vector2(2f, 5f); // min/max Wartezeit

        private Vector3 startPos;
        private float naechsterWanderStart;
        private bool wartet;

        private State state;
        private Spieler spieler;
        private DamageController spielerLeben;
        private NavMeshAgent navMeshAgent;
        private float abstandZumSpieler;

        void Start()
        {
            navMeshAgent = GetComponent<NavMeshAgent>();
            spieler = Spieler.Instance;
            if (spieler != null)
                spielerLeben = spieler.GetComponent<DamageController>();
            DamageController mDC = GetComponent<DamageController>();
            if (mDC != null)
            {
                mDC.OnDamage += GetHit;
                mDC.OnDeath += Die;
            }
            state = State.IDLE;
            startPos = transform.position;
        }


        void Update()
        {
            if (state == State.DEAD) return;
            HandleMovement();
            HandleAttack();
        }
        bool IsPlayerAlive()
        {
            if (spieler == null || spielerLeben == null || spielerLeben.IsDead())
                return false;
            return true;
        }
        void HandleMovement()
        {
            abstandZumSpieler = Vector3.Distance(spieler.transform.position, transform.position);
            if (state == State.AGGRO)
            {
                if (IsPlayerAlive())
                {
                    // Zum Spieler laufen
                    navMeshAgent.SetDestination(spieler.transform.position);
                }
                else
                {
                    // Spieler tot, ich beruhige mich wieder
                    state = State.IDLE;
                }
            }
            else if (state == State.IDLE)
            {
                // Random herumlaufen
                Wander();

                // Lebender Spieler in Reichweite macht mich aggressiv
                if (IsPlayerAlive() && abstandZumSpieler < aggroRange)
                    state = State.AGGRO;
            }
        }

        void Wander()
        {
            bool angekommen = !navMeshAgent.pathPending
                              && navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance;

            if (angekommen)
            {
                if (!wartet)
                {
                    // Gerade angekommen: Pause auslosen
                    wartet = true;
                    naechsterWanderStart = Time.time + Random.Range(wanderPause.x, wanderPause.y);
                }
                else if (Time.time >= naechsterWanderStart)
                {
                    Vector3 zufall = startPos + Random.insideUnitSphere * wanderRadius;
                    if (NavMesh.SamplePosition(zufall, out NavMeshHit hit, 2f, NavMesh.AllAreas))
                    {
                        navMeshAgent.SetDestination(hit.position);
                        wartet = false;
                    }
                    // Kein gültiger Punkt: nächster Frame versucht es erneut
                }
            }
        }

        void HandleAttack()
        {
            if (wpn == null) return;
            float reichweite = wpn.Range;
            if (wpn.Ammo <= 0)
            {
                // Nachladen
                wpn.AddMunition(100);
            }
            if (state == State.AGGRO && abstandZumSpieler <= reichweite)
            {
                // Wir können ballern, ziele auf Spieler
                wpn.Target = spieler.transform;
                wpn.Firing = true;
            }
            else
            {
                // Zielen und schießen abbrechen
                wpn.Target = null;
                wpn.Firing = false;
            }
        }

        void GetHit(float dmg)
        {
            if (state == State.DEAD) return;
            // Ich wurde getroffen, das war vermutlich der Spieler, jetzt bin ich sauer
            state = State.AGGRO;
            // Starte die Animation neu
            if (animator != null) animator.SetTrigger("Hit");
        }

        void Die()
        {
            // Merken, dass ich tot bin und nichts mehr machen kann
            state = State.DEAD;

            // Agent stoppen
            if (navMeshAgent != null)
            {
                navMeshAgent.isStopped = true;
                navMeshAgent.enabled = false;
            }

            // Waffe stilllegen
            if (wpn != null)
            {
                wpn.Firing = false;
                wpn.Target = null;
            }

            // Collider ausschalten
            foreach (Collider c in GetComponentsInChildren<Collider>())
                c.enabled = false;

            // Starte die Animation neu
            if (animator != null) animator.SetTrigger("Die");
            // Nach der Animation entfernen
            Destroy(gameObject, 1f);
        }

        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, aggroRange);
        }
    }
}