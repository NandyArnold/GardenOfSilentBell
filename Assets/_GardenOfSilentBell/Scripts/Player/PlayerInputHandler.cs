using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class PlayerInputHandler : MonoBehaviour
{
    public Vector2 MovementInput { get; private set; }
    public bool JumpPressed { get; private set; }
    public bool InteractPressed { get; private set; }

    private PlayerInput playerInput;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();

        // Hook into events from InputAction
        playerInput.actions["Move"].performed += OnMove;
        playerInput.actions["Move"].canceled += OnMove;

        playerInput.actions["Jump"].performed += OnJump;
        playerInput.actions["Interact"].performed += OnInteract;
    }

    private void OnDestroy()
    {
        // Always clean up subscriptions
        playerInput.actions["Move"].performed -= OnMove;
        playerInput.actions["Move"].canceled -= OnMove;
        playerInput.actions["Jump"].performed -= OnJump;
        playerInput.actions["Interact"].performed -= OnInteract;
    }

    private void Update()
    {
        JumpPressed = false;
        InteractPressed = false;
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        MovementInput = context.ReadValue<Vector2>();
    }

    private void OnJump(InputAction.CallbackContext context)
    {
        JumpPressed = true;
    }

    private void OnInteract(InputAction.CallbackContext context)
    {
        InteractPressed = true;
    }
}
