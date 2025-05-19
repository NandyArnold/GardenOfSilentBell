using UnityEngine;


//[RequireComponent(typeof(Rigidbody2D), typeof(PlayerInputHandler), typeof(MovementHandler), typeof(InteractionHandler))]
public class PlayerController : MonoBehaviour
{
    private MovementHandler movement;
    private InteractionHandler interaction;
    private PlayerInputHandler input;

    private void Awake()
    {
        movement = GetComponent<MovementHandler>();
        interaction = GetComponent<InteractionHandler>();
        input = GetComponent<PlayerInputHandler>();
    }

    private void Update()
    {
        //Debug.Log("Update is running");
        movement.ProcessMove(input.MovementInput);
        if (input.JumpPressed)
            movement.Jump();

        if (input.InteractPressed)
            interaction.TryInteract();
    }
}

