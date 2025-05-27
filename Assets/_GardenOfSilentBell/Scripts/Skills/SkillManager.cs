using UnityEngine;

public class SkillManager : MonoBehaviour
{
    public SkillSO[] assignedSkills;
    private ISkill[] runtimeSkills;

    void Awake()
    {
        runtimeSkills = new ISkill[assignedSkills.Length];
        for (int i = 0; i < assignedSkills.Length; i++)
        {
            runtimeSkills[i] = assignedSkills[i].CreateSkillInstance(gameObject);
        }
    }

    public void UseSkill(int index)
    {
        if (index < 0 || index >= runtimeSkills.Length) return;
        if (runtimeSkills[index].CanActivate())
            runtimeSkills[index].Activate();
    }
}
