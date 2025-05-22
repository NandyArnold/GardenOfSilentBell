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
        if (playerInput == null) playerInput = GetComponent<PlayerInput>();

    }

    void OnEnable()
    {
        

       
       
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

    void OnDisable()
    {
        if (playerInput == null || playerInput.actions == null) return;

        playerInput.actions["Move"].performed -= OnMove;
        playerInput.actions["Move"].canceled -= OnMove;
        playerInput.actions["Jump"].performed -= OnJump;
        playerInput.actions["Interact"].performed -= OnInteract;
        playerInput.actions["Sprint"].performed -= OnSprint;
        playerInput.actions["Sprint"].canceled -= OnSprint;
        playerInput.actions["Switch"].performed -= OnSwitch;
    }
    private void OnDestroy()
    {
        
    }

    private void LateUpdate()
    {
        JumpPressed = false;
        InteractPressed = false;
        SprintPressed = false;
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        if (!isActivePlayer || !enabled) return;

        if (context.canceled)
        {
            MovementInput = Vector2.zero;
        }
        else
        {
            MovementInput = context.ReadValue<Vector2>();
        }
    }

    private void OnJump(InputAction.CallbackContext context)
    {
        if (!isActivePlayer || !context.performed || !enabled) return;
        JumpPressed = true;
    }

    private void OnInteract(InputAction.CallbackContext context)
    {
        if (!isActivePlayer || !context.performed || !enabled) return;
        InteractPressed = true;
    }

    private void OnSprint(InputAction.CallbackContext context)
    {
        if (!isActivePlayer || !context.performed || !enabled) return;
        SprintPressed = true;
    }

    public void OnToggleFollow(InputAction.CallbackContext context)
    {
        if (!isActivePlayer || !context.performed || !enabled) return;
        //FollowManager.Instance?.ToggleFollow();
    }

    public void OnSwitch(InputAction.CallbackContext context)
    {
        Debug.Log($"[PlayerInputHandler] OnSwitch called on {gameObject.name}, isActivePlayer: {isActivePlayer}");

        if (!playerInput.inputIsActive ||!isActivePlayer || !context.performed) return;

        Debug.Log("Trying to switch character");
        CharacterManager.Instance.SwitchCharacter();
    }

    public void ResetInput()
    {
        MovementInput = Vector2.zero;
        JumpPressed = false;
        InteractPressed = false;
        SprintPressed = false;
    }



}
