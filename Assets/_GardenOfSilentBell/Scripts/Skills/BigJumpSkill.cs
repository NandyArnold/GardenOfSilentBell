using UnityEngine;

public class BigJumpSkill : ISkill
{
    private readonly GameObject character;
    private readonly BigJumpHandler jumpHandler;
    private bool isActive;

    public BigJumpSkill(GameObject character)
    {
        this.character = character;
        jumpHandler = character.GetComponent<BigJumpHandler>();
    }

    public void Activate()
    {
        if (!CanActivate()) return;

        isActive = true;
        jumpHandler?.DoBigJump();
    }

    public bool CanActivate()
    {
        return jumpHandler != null && !isActive;
    }

    public void Deactivate()
    {
        isActive = false;
        jumpHandler?.ResetJump();
    }
}
