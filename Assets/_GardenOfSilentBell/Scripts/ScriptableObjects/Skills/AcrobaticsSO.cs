using UnityEngine;

[CreateAssetMenu(fileName = "AcrobaticsSkill", menuName = "Skills/Acrobatics")]
public class AcrobaticsSO : SkillSO
{
    public override ISkill CreateSkillInstance(GameObject character)
    {
        return new AcrobaticsSkill(character, this);
    }
    public override string Description()
    {
        return "Allows the user to climb on walls";
    }
}

