using UnityEngine;

[CreateAssetMenu(menuName = "Skills/Magic Interact")]
public class MagicInteractSO : SkillSO
{
    public float interactRange = 10f;

    public override ISkill CreateSkillInstance(GameObject character)
    {
        return new MagicInteractSkill(character, this);
    }
}
