using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private MovementHandler movement;
    private InteractionHandler interaction;
    private PlayerInputHandler input;
    private SpriteFlipper spriteFlipper;
    private TelekinesisHandler telekinesisHandler;
    private MagicInteractHandler magicInteractHandler;


    private TelekinesisSkill telekinesisSkill;

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
        magicInteractHandler = GetComponent<MagicInteractHandler>();
    }

    private void Start()
    {
        SkillManager skillManager = GetComponent<SkillManager>();
        if (skillManager != null)
        {
            foreach (SkillSO so in skillManager.assignedSkills)
            {
                if (so is TelekinesisSO)
                {
                    telekinesisSkill = skillManager.GetSkillOfType<TelekinesisSkill>();
                    if (telekinesisSkill != null)
                    {
                        Debug.Log("[PlayerController] Got TelekinesisSkill from SkillManager.");
                    }
                    else
                    {
                        Debug.LogWarning("[PlayerController] TelekinesisSkill was not found in SkillManager.");
                    }
                    break;
                }
                else
                {
                    Debug.Log($"[PlayerController] Skill {so.name} is not TelekinesisSO, skipping.");
                }
            }
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

        magicInteractHandler.HighlightMagicTarget(); // Always show glow when near something

        if (input.InteractPressed)
        {
            interaction.TryInteract();
            magicInteractHandler.TryMagicInteract();
        }

        if(telekinesisSkill == null)
        {
            //Debug.LogWarning("[PlayerController] TelekinesisSkill is null, cannot grab.");
            return;
        }
        if (telekinesisSkill != null)
        {

            if (input.GrabPressed && telekinesisSkill != null)
            {
                Debug.Log($"[PlayerController] Grab pressed, trying to grab with TelekinesisSkill: {telekinesisSkill}");
                telekinesisSkill.ToggleGrab();
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
