using UnityEngine;

public class DeftHandsSkill : ISkill
{
    private GameObject character;

    public DeftHandsSkill(GameObject character, DeftHandsSO data)
    {
        this.character = character;
        var disarmer = character.GetComponent<TrapDisarmHandler>();
        if (disarmer != null) disarmer.enabled = true;
    }

    public void Activate() { } // Passive
    public bool CanActivate() => false;
    public void Deactivate() { }
}
