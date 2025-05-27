using UnityEngine;

public class SuperStrengthSkill : ISkill
{
    private readonly GameObject character;
    private readonly HeavyPushHandler pushHandler;
    private bool isActive;

    public SuperStrengthSkill(GameObject character)
    {
        this.character = character;
        pushHandler = character.GetComponent<HeavyPushHandler>();
    }

    public void Activate()
    {
        if (!CanActivate()) return;

        isActive = true;
        pushHandler?.EnableHeavyPush();
    }

    public bool CanActivate()
    {
        return pushHandler != null && !isActive;
    }

    public void Deactivate()
    {
        isActive = false;
        pushHandler?.DisableHeavyPush();
    }


}
