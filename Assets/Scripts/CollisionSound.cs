using UnityEngine;

public class CollisionSound : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private AudioClip Soundeffect;

    private AudioSource _audioSource;

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (_audioSource != null && Soundeffect != null)
            _audioSource.PlayOneShot(Soundeffect);
    }
}
