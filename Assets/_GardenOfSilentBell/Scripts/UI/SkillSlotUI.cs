using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkillSlotUI : MonoBehaviour
{
    public Image iconImage;
    public TMP_Text cooldownText;
    public TMP_Text chargesText;
    public TMP_Text skillNameText;

    private SkillSO skillSO;
    private ISkill runtimeSkill;

    public void Initialize(SkillSO skill, ISkill runtime)
    {
        skillSO = skill;
        runtimeSkill = runtime;

        iconImage.sprite = skillSO.icon;
        skillNameText.text = skillSO.skillName;

        // If you want to show static values like cooldown/charges:
        cooldownText.text = skillSO.cooldown > 0 ? $"{skillSO.cooldown}s" : "";
        chargesText.text = skillSO.charges > 0 ? $"x{skillSO.charges}" : "";
    }
}
