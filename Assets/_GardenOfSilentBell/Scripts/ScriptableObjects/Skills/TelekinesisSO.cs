using UnityEngine;

[CreateAssetMenu(fileName = "TelekinesisSkill", menuName = "Skills/Telekinesis")]
public class TelekinesisSO : SkillSO
{
    public float interactionRange = 5f;
    public LayerMask telekinesisLayer;

    public override ISkill CreateSkillInstance(GameObject character)
    {
        return new TelekinesisSkill(character, this);
    }
}
