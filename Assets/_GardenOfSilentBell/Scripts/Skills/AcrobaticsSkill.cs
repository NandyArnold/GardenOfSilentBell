using UnityEngine;

public class AcrobaticsSkill : ISkill
{
    private GameObject character;

    public AcrobaticsSkill(GameObject character, AcrobaticsSO data)
    {
        this.character = character;
        var climber = character.GetComponent<WallClimbHandler>();
        if (climber != null) climber.enabled = true;
    }

    public void Activate() { }
    public bool CanActivate() => false;
    public void Deactivate() { }
}
