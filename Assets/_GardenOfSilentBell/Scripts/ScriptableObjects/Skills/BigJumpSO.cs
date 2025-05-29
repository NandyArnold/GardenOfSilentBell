using UnityEngine;

[CreateAssetMenu(fileName = "BigJumpSkill", menuName = "Skills/BigJump")]
public class BigJumpSkillSO : SkillSO
{
    public override ISkill CreateSkillInstance(GameObject character)
    {
        return new BigJumpSkill(character);
    }
    public override string Description()
    {
        return "Allows the user to performe a big jump to overcome high gaps";
    }
}
