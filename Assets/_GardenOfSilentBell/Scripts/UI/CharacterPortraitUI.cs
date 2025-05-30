using UnityEngine;
using UnityEngine.UI;

public class CharacterPortraitUI : MonoBehaviour
{
    public Image portraitImage;
    public Button selectButton;
    private string characterId;

    public void Setup(Sprite portrait, string id, System.Action<string> onClickCallback)
    {
        if (portraitImage == null || selectButton == null)
        {
            Debug.LogError("[CharacterPortraitUI] Missing references in inspector!");
            return;
        }
        characterId = id;
        portraitImage.sprite = portrait;
        selectButton.onClick.RemoveAllListeners(); // important if reusing
        selectButton.onClick.AddListener(() => onClickCallback?.Invoke(characterId));
    }
}

