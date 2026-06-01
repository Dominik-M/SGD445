using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class LaufKyle : MonoBehaviour
{

    [Header("Input Axis (Sticks)")]
    [SerializeField] private InputActionReference MoveInput;
    [SerializeField] private InputActionReference LookInput;

    public float geschwindigkeit = 6;
    public float geschwindigkeitDrehung = 2;

    private Animator Animator;

    void Start()
    {
        Animator = GetComponent<Animator>();
    }
    private void OnEnable()
    {
        MoveInput.action.Enable();
        LookInput.action.Enable();
    }
    private void OnDisable()
    {
        MoveInput.action.Disable();
        LookInput.action.Disable();
    }


    void Update()
    {
        Vector2 move = MoveInput.action.ReadValue<Vector2>();
        Vector2 look = LookInput.action.ReadValue<Vector2>();
        float horizontal = move.x;
        float vertikal = move.y;

        transform.Translate(new Vector3(horizontal, 0, vertikal) * geschwindigkeit * Time.deltaTime);

        float turn = look.x;

        transform.rotation *= Quaternion.Slerp(Quaternion.identity, Quaternion.LookRotation(turn < 0 ? Vector3.left : Vector3.right), Mathf.Abs(turn) * geschwindigkeitDrehung * Time.deltaTime);

        Animator.SetFloat("vorwärts", vertikal);
        Animator.SetFloat("seitwärts", horizontal);
    }
}
