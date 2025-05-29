using UnityEngine;

[CreateAssetMenu(fileName = "SuperStrengthSkill", menuName = "Skills/SuperStrength")]
public class SuperStrengthSkillSO : SkillSO
{
    public override ISkill CreateSkillInstance(GameObject character)
    {
        return new SuperStrengthSkill(character);
    }
    public override string Description()
    {
        return "Allows the user to move around very heavy objects";
    }
}
