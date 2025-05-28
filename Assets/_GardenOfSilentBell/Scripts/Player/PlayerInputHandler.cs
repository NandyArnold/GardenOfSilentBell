using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

//[RequireComponent(typeof(PlayerInput))]
public class PlayerInputHandler : MonoBehaviour
{
    public Vector2 MovementInput { get; private set; }
    public bool JumpPressed { get; private set; }
    public bool InteractPressed { get; private set; }
    public bool SprintPressed { get; private set; }
    public bool GrabPressed { get; private set; }   


    private PlayerInput playerInput;

    public bool isActivePlayer = true;

    private bool actionsBound = false;  



    private void Awake()
    {
        if (playerInput == null) playerInput = GetComponent<PlayerInput>();

    }

    private void OnEnable()
    {
        //Debug.Log($"[OnEnable] {gameObject.name}");
        if (playerInput == null) playerInput = GetComponent<PlayerInput>();
        BindAllInputActions();
    }


    private void OnDisable()
    {
        //Debug.Log($"[OnDisable] {gameObject.name}");
        UnbindAllInputActions();
    }
    private void OnDestroy()
    {
        //Debug.Log($"[OnDestroy] {gameObject.name}");
        UnbindAllInputActions();
        playerInput = null;
    }

    private void LateUpdate()
    {
        JumpPressed = false;
        InteractPressed = false;
        //SprintPressed = false;
        GrabPressed = false;    
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        //Debug.Log($"[OnMove] {gameObject.name} enabled={enabled} isActivePlayer={isActivePlayer}");
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
        //Debug.Log($"[OnJump] {gameObject.name} enabled={enabled} isActivePlayer={isActivePlayer}");
        if (!isActivePlayer || !context.performed || !enabled) return;
        JumpPressed = true;
    }

    private void OnInteract(InputAction.CallbackContext context)
    {
        Debug.Log($"[OnInteract] {gameObject.name} enabled={enabled} isActivePlayer={isActivePlayer}");
        if (!isActivePlayer || !context.performed || !enabled) return;
        InteractPressed = true;
    }

    private void OnGrab(InputAction.CallbackContext context)
    {
        Debug.Log($"[OnGrab] {gameObject.name} enabled={enabled} isActivePlayer={isActivePlayer}");
        if (!isActivePlayer || !context.performed || !enabled) return;
        GrabPressed = true;
    }

    private void OnSprint(InputAction.CallbackContext context)
    {
        //Debug.Log($"[OnSprint] {gameObject.name} enabled={enabled} isActivePlayer={isActivePlayer}");


        if (!enabled || !isActivePlayer) return;

        if (context.performed)
        {
            SprintPressed = true;
            Debug.Log($"[OnSprint] Sprint started");
        }
        else if (context.canceled)
        {
            SprintPressed = false;
            Debug.Log($"[OnSprint] Sprint stopped");
        }
    }

    public void OnToggleFollow(InputAction.CallbackContext context)
    {
        Debug.Log($"[OnToggleFollow] {gameObject.name} enabled={enabled} isActivePlayer={isActivePlayer}");
        if (!isActivePlayer || !context.performed || !enabled) return;

        if (FollowManager.Instance != null)
        {
            Debug.Log("ToggleFollow pressed by " + gameObject.name);
            FollowManager.Instance.ToggleFollowAll();
        }
        else
        {
            Debug.LogWarning("FollowManager.Instance is null.");
        }
    }

    public void OnSwitch(InputAction.CallbackContext context)
    {
        Debug.Log($"[PlayerInputHandler] OnSwitch called on {gameObject.name}, isActivePlayer: {isActivePlayer}");
        //Debug.Log($"[OnSwitch] {gameObject.name} enabled={enabled} isActivePlayer={isActivePlayer}");

        if (!playerInput.inputIsActive || !isActivePlayer || !context.performed)
            return;
        MovementInput = Vector2.zero; // Reset movement input to prevent unwanted movement during switch
        StartCoroutine(DeferredSwitch());

        //if (CharacterManager.Instance == null)
        //{
        //    Debug.Break(); // Pause the game in Editor
        //    Debug.LogError("[PlayerInputHandler] CharacterManager.Instance is null just before switching!");
        //    return;
        //}

        //try
        //{
        //    Debug.Log("[PlayerInputHandler] Trying to switch character...");
        //    CharacterManager.Instance.SwitchCharacter();
        //}
        //catch (System.Exception ex)
        //{
        //    Debug.LogError($"[PlayerInputHandler] Exception during character switch: {ex.Message}\n{ex.StackTrace}");
        //}
    }

    public void ResetInput()
    {
        MovementInput = Vector2.zero;
        JumpPressed = false;
        InteractPressed = false;
        SprintPressed = false;
        if (playerInput != null && playerInput.actions != null)
        {
            playerInput.DeactivateInput();
        }
    }

    public void BindAllInputActions()
    {
        if (actionsBound || playerInput == null || playerInput.actions == null)
            return;

        playerInput.actions["Move"].performed += OnMove;
        playerInput.actions["Move"].canceled += OnMove;
        playerInput.actions["Jump"].performed += OnJump;
        playerInput.actions["Interact"].performed += OnInteract;
        playerInput.actions["Sprint"].performed += OnSprint;
        playerInput.actions["Sprint"].canceled += OnSprint;
        playerInput.actions["Switch"].performed += OnSwitch;
        playerInput.actions["ToggleFollow"].performed += OnToggleFollow;
        playerInput.actions["Grab"].performed += OnGrab;

        actionsBound = true;
    }

    public void UnbindAllInputActions()
    {
        if (!actionsBound || playerInput == null || playerInput.actions == null)
            return;

        playerInput.actions["Move"].performed -= OnMove;
        playerInput.actions["Move"].canceled -= OnMove;
        playerInput.actions["Jump"].performed -= OnJump;
        playerInput.actions["Interact"].performed -= OnInteract;
        playerInput.actions["Sprint"].performed -= OnSprint;
        playerInput.actions["Sprint"].canceled -= OnSprint;
        playerInput.actions["Switch"].performed -= OnSwitch;
        playerInput.actions["ToggleFollow"].performed -= OnToggleFollow;
        playerInput.actions["Grab"].performed -= OnGrab;

        actionsBound = false;
    }

    private IEnumerator DeferredSwitch()
    {
        yield return null; // Wait one frame
        CharacterManager.Instance.SwitchCharacter();
    }

}
