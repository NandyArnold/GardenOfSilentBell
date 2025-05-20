using UnityEngine;
using UnityEngine.InputSystem;

//[RequireComponent(typeof(PlayerInput))]
public class PlayerInputHandler : MonoBehaviour
{
    public Vector2 MovementInput { get; private set; }
    public bool JumpPressed { get; private set; }
    public bool InteractPressed { get; private set; }
    public bool SprintPressed { get; private set; }

    private PlayerInput playerInput;

    public bool isActivePlayer = true;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();

        if (playerInput == null)
        {
            Debug.LogError("PlayerInput is missing from " + gameObject.name);
            return;
        }

        if (playerInput.actions == null)
        {
            Debug.LogError("PlayerInput.actions is null on " + gameObject.name);
            return;
        }

        // Hook into events from InputAction
        playerInput.actions["Move"].performed += OnMove;
        playerInput.actions["Move"].canceled += OnMove;

        playerInput.actions["Jump"].performed += OnJump;
        playerInput.actions["Interact"].performed += OnInteract;

        playerInput.actions["Sprint"].performed += OnSprint;
        playerInput.actions["Sprint"].canceled += OnSprint;

        playerInput.actions["Switch"].performed += OnSwitch;

    }

    private void OnDestroy()
    {
        // Always clean up subscriptions
        playerInput.actions["Move"].performed -= OnMove;
        playerInput.actions["Move"].canceled -= OnMove;
        playerInput.actions["Jump"].performed -= OnJump;
        playerInput.actions["Interact"].performed -= OnInteract;
    }

    private void LateUpdate()
    {
        JumpPressed = false;
        InteractPressed = false;
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        MovementInput = context.ReadValue<Vector2>();
        if (!isActivePlayer || !context.performed) return;
    }

    private void OnJump(InputAction.CallbackContext context)
    {
        JumpPressed = true;
        if (!isActivePlayer || !context.performed) return;
    }

    private void OnInteract(InputAction.CallbackContext context)
    {
        InteractPressed = true;
        if (!isActivePlayer || !context.performed) return;
    }

    private void OnSprint(InputAction.CallbackContext context)
    {
        SprintPressed = true;
        if (!isActivePlayer || !context.performed) return;
    }

    public void OnSwitch(InputAction.CallbackContext context)
    {
        Debug.Log($"[PlayerInputHandler] OnSwitch called on {gameObject.name}, isActivePlayer: {isActivePlayer}");

        if (!isActivePlayer || !context.performed) return;

        Debug.Log("Trying to switch character");
        CharacterManager.Instance.SwitchCharacter();
    }
}
