using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private MovementHandler movement;
    private InteractionHandler interaction;
    private PlayerInputHandler input;
    private SpriteFlipper spriteFlipper;
    private void Awake()
    {
        movement = GetComponent<MovementHandler>();
        interaction = GetComponent<InteractionHandler>();
        input = GetComponent<PlayerInputHandler>();
        spriteFlipper = GetComponent<SpriteFlipper>();
    }

    private void Update()
    {
        movement.ProcessMove(input.MovementInput);

        if (spriteFlipper != null && interaction != null)
        {
            // Disable flipping if pushing
            spriteFlipper.disableFlip = interaction.IsPushing;
        }

        if (input.JumpPressed)
            movement.Jump();

        if (input.InteractPressed)
            interaction.TryInteract();
    }
}
