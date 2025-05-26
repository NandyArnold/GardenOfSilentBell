using UnityEngine;

public class UnlockCharacter : MonoBehaviour
{
    public string characterNameToUnlock;         // This should match the 'id' field of CharacterData
    public bool switchToUnlockedCharacter = true;

    public bool TryUnlockCharacter()
    {
        var manager = CharacterManager.Instance;
        if (manager == null)
        {
            Debug.LogError("[UnlockCharacter] CharacterManager not found!");
            return false;
        }

        for (int i = 0; i < manager.CharacterCount; i++)
        {
            var character = manager.GetCharacterData(i);
            if (character.id == characterNameToUnlock)
            {
                if (!character.isUnlocked)
                {
                    character.isUnlocked = true;
                    character.lastPosition = Vector2.zero; // Set a default position if needed
                    Debug.Log($"[UnlockCharacter] Character '{characterNameToUnlock}' unlocked!");
                    if (character.instance != null)
                    {
                        var controller = character.instance.GetComponent<PlayerController>();
                        if (controller != null)
                        {
                            controller.isUnlocked = true;
                        }
                    }
                    CharacterManager.Instance.EnableSwitching();
                    if (switchToUnlockedCharacter)
                    {
                        manager.SetActiveCharacter(i);
                    }

                    else
                    {
                        FollowManager.Instance?.AssignFollowTargets();
                    }
                    SaveManager.Instance?.SaveGame(); // Save immediately after unlocking
                    return true;
                }
                else
                {
                    Debug.Log($"[UnlockCharacter] Character '{characterNameToUnlock}' was already unlocked.");
                    return false;
                }
            }
        }

        Debug.LogWarning($"[UnlockCharacter] Character with name '{characterNameToUnlock}' not found in CharacterManager.");
        return false;
    }
}
