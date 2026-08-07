using UnityEngine;

public class RocketBehavior : InteractionBehavior
{
    [Header("Schubkraft der Rakete")]
    [SerializeField] private float power = 100;
    [Header("Zeit bis zur Explosion in Sekunden")]
    [SerializeField] private float lifetime = 5;
    [Header("Prefab des Explosionseffekts")]
    [SerializeField] private GameObject explosion;
    [Header("Anzeigetext für Schubkraft")]
    [SerializeField] private TMPro.TextMeshPro powerText;

    public bool Starten
    {
        get => gestartet; set
        {
            gestartet = value;
            if (gestartet) mparticleSystem.Play();
            else mparticleSystem.Stop();
        }
    }
    private bool gestartet = false;

    private Rigidbody rb;
    private ParticleSystem mparticleSystem;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
            rb = gameObject.AddComponent<Rigidbody>();
        mparticleSystem = GetComponentInChildren<ParticleSystem>();
        powerText.text = power.ToString("F0");
    }

    void Update()
    {
        if (Starten)
        {
            rb.AddForce(transform.up * power * Time.deltaTime);
            if (lifetime <= 0)
            {
                Instantiate(explosion, transform.position, transform.rotation);
                Destroy(gameObject);
            }
            else
                lifetime -= Time.deltaTime;
        }
    }

    public override void OnInteract()
    {
        Starten = true;
    }
}
