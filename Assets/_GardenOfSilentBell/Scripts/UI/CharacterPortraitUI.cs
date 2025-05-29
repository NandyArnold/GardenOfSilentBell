using UnityEngine;
using UnityEngine.UI;

public class CharacterPortraitUI : MonoBehaviour
{
    public Image portraitImage;
    public Button selectButton;
    private string characterId;

    public void Setup(Sprite portrait, string id, System.Action<string> onClickCallback)
    {
        characterId = id;
        portraitImage.sprite = portrait;
        selectButton.onClick.RemoveAllListeners(); // important if reusing
        selectButton.onClick.AddListener(() => onClickCallback?.Invoke(characterId));
    }
}

