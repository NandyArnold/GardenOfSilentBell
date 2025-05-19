using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    public Vector2 MovementInput { get; private set; }
    public bool JumpPressed { get; private set; }
    public bool InteractPressed { get; private set; }

    private PlayerInput playerInput;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        MovementInput = context.ReadValue<Vector2>();
        //Debug.Log("Received Move Input: " + MovementInput);

    }

    public void OnJump(InputAction.CallbackContext context)
    {
        JumpPressed = context.performed;
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        InteractPressed = context.performed;
    }

    private void LateUpdate()
    {
        JumpPressed = false;
        InteractPressed = false;
    }
}

