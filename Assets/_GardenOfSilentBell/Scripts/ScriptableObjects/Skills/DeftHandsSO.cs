using UnityEngine;

[CreateAssetMenu(fileName = "DeftHandsSkill",menuName = "Skills/Deft Hands")]
public class DeftHandsSO : SkillSO
{
    public override ISkill CreateSkillInstance(GameObject character)
    {
        return new DeftHandsSkill(character, this);
    }
    public override string Description()
    {
        return "Allows the user to disarm traps and pick locks";
    }
}
