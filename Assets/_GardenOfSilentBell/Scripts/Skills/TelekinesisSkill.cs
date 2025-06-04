using UnityEngine;

public class TelekinesisSkill : ISkill
{
    private readonly GameObject character;
    private readonly TelekinesisHandler handler;
    private readonly TelekinesisSO skillData;
    private readonly PlayerInputHandler inputHandler;
    private bool isActive;

    public TelekinesisSkill(GameObject character, TelekinesisSO skillData)
    {
        this.character = character;
        this.skillData = skillData;
        this.inputHandler = character.GetComponent<PlayerInputHandler>();
        this.handler = character.GetComponent<TelekinesisHandler>();

        //Debug.Log($"[TelekinesisSkill] Constructor called for character: {character.name}");
        //Debug.Log("[TelekinesisSkill] Handler is " + (handler == null ? "NULL" : "FOUND"));

        if (handler != null)
        {
            handler.Initialize(skillData);
            //Debug.Log($"[TelekinesisSkill] TelekinesisHandler initialized for {character.name}");
        }
        else
        {
            Debug.LogError($"[TelekinesisSkill] TelekinesisHandler component not found on {character.name}");
        }
    }
    public void Activate()
    {
        if (!CanActivate()) return;
        isActive = true;
        handler?.EnableTelekinesis();
    }

    public bool CanActivate() => handler != null && !isActive;

    public void Deactivate()
    {
        isActive = false;
        handler?.DisableTelekinesis();
    }

    public void TryGrab()
    {
        if (!isActive) return;
        handler?.TryGrab();
    }

    public void TryRelease()
    {
        if (!isActive) return;
        handler?.TryRelease();
    }

    public void ToggleGrab()
    {
        if (!isActive) return;
        if (handler.IsGrabbing)
        {
            TryRelease();
        }
        else
        {
            TryGrab();
        }
    }
}