using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private MovementHandler movement;
    private InteractionHandler interaction;
    private PlayerInputHandler input;
    private SpriteFlipper spriteFlipper;

    private TelekinesisSkill telekinesisSkill;
    [SerializeField]
    private TelekinesisSO telekinesisSO;

    [Header("Character State")]
    public bool isUnlocked = true;

    // Track last facing direction
    private int lastFacing = 1; // Default to right
    private void Awake()
    {
        movement = GetComponent<MovementHandler>();
        interaction = GetComponent<InteractionHandler>();
        input = GetComponent<PlayerInputHandler>();
        spriteFlipper = GetComponent<SpriteFlipper>();
        if (telekinesisSO != null)
        {
            telekinesisSkill = (TelekinesisSkill)telekinesisSO.CreateSkillInstance(gameObject);
            telekinesisSkill.Activate();
        }

    }

    private void Update()
    {
        // Handle input, interaction, and sprite flipping (non-physics)
        // Check for facing change and update followers
        if (spriteFlipper != null)
        {
            int currentFacing = spriteFlipper.CurrentFacing;
            if (currentFacing != lastFacing)
            {
                lastFacing = currentFacing;
                // +1 = right, -1 = left
                bool isFacingRight = currentFacing == 1;
                FollowManager.Instance?.UpdateFollowerOffsets(isFacingRight);
            }
        }

        if (!interaction.IsPushing && input.JumpPressed)
            movement.Jump();

        if (input.InteractPressed)
            interaction.TryInteract();

        if (telekinesisSkill != null)
        {
            if (input.GrabPressed && telekinesisSkill != null)
            {
                telekinesisSkill.TryGrab();
            }
        }
    }

    private void FixedUpdate()
    {
        // Handle movement and pushing (physics)
        movement.ProcessMove(input.MovementInput, interaction.IsPushing, input.SprintPressed);
       

        if (interaction.IsPushing && interaction.CurrentPushTarget != null)
        {
            interaction.CurrentPushTarget.SetBeingPushed(true);
            interaction.CurrentPushTarget.Push(input.MovementInput, movement.pushMoveSpeed);
        }


    }

    
}
