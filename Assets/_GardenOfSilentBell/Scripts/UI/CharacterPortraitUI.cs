//using static System.Net.Mime.MediaTypeNames   
using UnityEngine;
using TMPro;
using UnityEngine.UI;


public class CharacterPortraitUI : MonoBehaviour
{
    public Image iconImage;
    public GameObject glowBorder;
    public TooltipTrigger tooltipTrigger; // Handles hover text
    public string characterId;

    public void Setup(Sprite portrait, string name, string id, bool isActive)
    {
        iconImage.sprite = portrait;
        tooltipTrigger.SetText(name);
        characterId = id;
        glowBorder.SetActive(isActive);
    }

    public void SetActiveVisual(bool active)
    {
        glowBorder.SetActive(active);
    }
}

