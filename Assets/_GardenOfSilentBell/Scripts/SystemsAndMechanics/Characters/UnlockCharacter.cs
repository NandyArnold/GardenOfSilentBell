using UnityEngine;

public class UnlockCharacter : MonoBehaviour
{
    public string characterNameToUnlock;
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
            var character = manager.GetCharacterEntry(i);
            if (character.character.name == characterNameToUnlock)
            {
                if (!character.isUnlocked)
                {
                    character.isUnlocked = true;
                    Debug.Log($"[UnlockCharacter] Character '{characterNameToUnlock}' unlocked!");

                    if (switchToUnlockedCharacter)
                    {
                        manager.SetActiveCharacter(i);
                    }

                    // **DO NOT trigger follow here**

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
