using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterPortraitUI : MonoBehaviour
{
    public Image portraitImage;
    public Button selectButton;
    private string characterId;
    public TMP_Text characterNameText;
    public GameObject activeCharacterPanel;

    public void Setup(Sprite portrait, string id, System.Action<string> onClickCallback, bool isActive)
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
        characterNameText.text = id; // Assuming characterId is the name, adjust as needed

        activeCharacterPanel.SetActive(isActive);
        Debug.Log($"[CharacterPortraitUI] Setting activeCharacterPanel to {isActive} for character {id}");
    }
}

