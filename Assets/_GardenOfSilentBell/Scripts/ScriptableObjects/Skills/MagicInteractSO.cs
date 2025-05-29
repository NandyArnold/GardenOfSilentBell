using UnityEngine;

[CreateAssetMenu(fileName = "MagicInteractSkill", menuName = "Skills/Magic Interact")]
public class MagicInteractSO : SkillSO
{
    public float interactRange = 10f;

    public override ISkill CreateSkillInstance(GameObject character)
    {
        return new MagicInteractSkill(character, this);
    }
    public override string Description()
    {
        return "Allows the user to interact with objects of magical nature from a great distance";
    }
}
