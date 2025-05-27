using UnityEngine;

[CreateAssetMenu(fileName = "BigJumpSkill", menuName = "Skills/BigJump")]
public class BigJumpSkillSO : SkillSO
{
    public override ISkill CreateSkillInstance(GameObject character)
    {
        return new BigJumpSkill(character);
    }
}
