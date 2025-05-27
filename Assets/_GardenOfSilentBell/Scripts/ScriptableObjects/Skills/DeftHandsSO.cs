using UnityEngine;

[CreateAssetMenu(fileName = "DeftHandsSkill",menuName = "Skills/Deft Hands")]
public class DeftHandsSO : SkillSO
{
    public override ISkill CreateSkillInstance(GameObject character)
    {
        return new DeftHandsSkill(character, this);
    }
}
