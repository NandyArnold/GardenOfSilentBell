using UnityEngine;

public enum SkillType { Instant, Passive, Aimed }

public abstract class SkillSO : ScriptableObject
{
    public string skillName;
    public Sprite icon;
    public SkillType skillType;
    public int charges = 0;
    public float cooldown = 0f;
    public GameObject aimPrefab; // Optional, used for aimed skills

    public abstract ISkill CreateSkillInstance(GameObject character);
}

