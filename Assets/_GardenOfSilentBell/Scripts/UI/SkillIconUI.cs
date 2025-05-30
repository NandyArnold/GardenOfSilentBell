using UnityEngine;
using TMPro;
using UnityEngine.UI;

//using static System.Net.Mime.MediaTypeNames;

public class SkillIconUI : MonoBehaviour
{
    public Image iconImage;
    public TMP_Text cooldownText;
    public TMP_Text chargeText;
    public TooltipTrigger tooltipTrigger;


    void Awake()
    {
        if (iconImage == null) Debug.LogError("[SkillIconUI] iconImage not assigned.");
        if (cooldownText == null) Debug.LogError("[SkillIconUI] cooldownText not assigned.");
        if (chargeText == null) Debug.LogError("[SkillIconUI] chargeText not assigned.");
    }

    public void Setup(SkillSO skill)
    {
        iconImage.sprite = skill.icon;
        cooldownText.text = skill.cooldown > 0 ? $"{skill.cooldown:F1}s" : "";
        chargeText.text = skill.charges > 0 ? $"{skill.charges}" : "";
        tooltipTrigger.SetText($"{skill.skillName}\n{skill.skillType}\n{skill.Description()}");


    }
}

