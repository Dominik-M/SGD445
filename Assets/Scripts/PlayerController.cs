using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Input Axis (Sticks)")]
    [SerializeField] private InputActionReference MoveInput;
    [Header("Main Buttons")]
    [SerializeField] private InputActionReference JumpInput;
    [SerializeField] private InputActionReference CancelInput;
    [Header("Parameter")]
    [SerializeField] private float Speed = 2;
    [SerializeField] private float JumpPower = 10;
    [Header("References")]
    [SerializeField] private AudioClip JumpSound;

    private Rigidbody rb;
    private AudioSource _audioSource;

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        MoveInput.action.Enable();
        JumpInput.action.Enable();
        CancelInput.action.Enable();
    }
    private void OnDisable()
    {
        MoveInput.action.Disable();
        JumpInput.action.Disable();
        CancelInput.action.Disable();
    }

    void Update()
    {
        Vector2 move = MoveInput.action.ReadValue<Vector2>();
        bool jump = JumpInput.action.WasPressedThisFrame();
        rb.AddForce(new Vector3(move.x, move.y / 4, 0) * Speed * Time.deltaTime);
        if (jump && CanJump())
        {
            rb.AddForce(new Vector3(0, JumpPower, 0));
            if (_audioSource != null && JumpSound != null)
                _audioSource.PlayOneShot(JumpSound);
        }
    }

    bool CanJump()
    {
        // One is always the player so must be greater
        return Physics.OverlapSphere(transform.position + new Vector3(0, -0.1f, 0), 0.49f).Length > 1;
    }
}
