using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class VirtualCursorManager : MonoBehaviour
{
    [Header("Refrenzen & Setup")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Image cursorUI; // Das Canvas-Image deines Cursors
    [SerializeField] private Sprite CursorNormal, CursorHovered;
    [SerializeField] private LayerMask interactableLayers;
    [SerializeField] private float gamepadSpeed = 800f;
    [SerializeField] private float interactionRange = 10f;
    [SerializeField] private bool enableMovement = true;

    [Header("Input Actions")]
    [SerializeField] private InputActionReference moveAction;     // z.B. Gamepad Stick
    [SerializeField] private InputActionReference interactAction; // z.B. Gamepad South / Cross Button

    private Vector2 cursorPosition;
    private IInteractable currentHovered;

    private void Awake()
    {
        cursorPosition = new Vector2(Screen.width / 2f, Screen.height / 2f);
    }

    private void OnEnable()
    {
        moveAction.action?.Enable();
        interactAction.action?.Enable();
    }

    private void OnDisable()
    {
        moveAction.action?.Disable();
        interactAction.action?.Disable();
    }

    private void Update()
    {
        if(enableMovement)
            UpdateCursorPosition();
        CheckRaycast();
        HandleInteraction();
    }

    private void UpdateCursorPosition()
    {
        // 1. Eingabe auslesen
        Vector2 stickInput = moveAction.action.ReadValue<Vector2>();

        // Mouse-Movement bevorzugen, wenn die Maus bewegt wird
        if (Pointer.current != null && Pointer.current.delta.ReadValue().sqrMagnitude > 0.1f)
        {
            cursorPosition = Pointer.current.position.ReadValue();
        }
        else
        {
            // Gamepad-Stick bewegt die Bildschirmposition
            cursorPosition += stickInput * gamepadSpeed * Time.deltaTime;
            cursorPosition.x = Mathf.Clamp(cursorPosition.x, 0, Screen.width);
            cursorPosition.y = Mathf.Clamp(cursorPosition.y, 0, Screen.height);
        }

        // UI-Cursor-Visual auf dem Bildschirm platzieren
        if (cursorUI != null)
        {
            cursorUI.transform.position = cursorPosition;
        }
    }

    private void CheckRaycast()
    {
        Ray ray = mainCamera.ScreenPointToRay(cursorPosition);
        IInteractable newlyHovered = null;

        if (!Inventar.ZeigeInventar
           && Physics.Raycast(ray, out RaycastHit hit, interactionRange, interactableLayers))
        {
            //Debug.Log("Cursor Raycast hit: " + hit.transform.name);

            // Versuch, das Interface direkt ODER auf Eltern-Objekten zu finden
            if (hit.collider.GetComponentInParent<IInteractable>() is IInteractable interactable)
            {
                newlyHovered = interactable;
            }
            else
            {
                //Debug.Log("Object does not implement IInteractable interface");
            }
        }

        // Hat sich das hovered Objekt geändert?
        if (newlyHovered != currentHovered)
        {
            currentHovered?.OnCursorExit();
            currentHovered = newlyHovered;
            currentHovered?.OnCursorEnter();
        }

        // UI-Cursor-Visual aktualisieren
        if (cursorUI != null)
        {
            if (currentHovered != null)
            {
                cursorUI.sprite = CursorHovered;
            }
            else
            {
                cursorUI.sprite = CursorNormal;
            }
        }
    }

    private void HandleInteraction()
    {
        if (currentHovered == null) return;

        if (interactAction.action.WasPressedThisFrame())
        {
            currentHovered.OnInteract();
        }
    }
}