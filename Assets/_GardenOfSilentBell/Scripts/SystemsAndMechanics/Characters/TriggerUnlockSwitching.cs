using UnityEngine;

public class TriggerUnlockSwitch : MonoBehaviour
{
    [Tooltip("Name of the character to unlock (must match GameObject name in CharacterManager list)")]
    public string characterNameToUnlock;

    [Tooltip("Automatically switch to the character after unlocking")]
    public bool switchToUnlockedCharacter = true;

    [Tooltip("Should the trigger deactivate itself after activation?")]
    public bool oneTimeUse = true;

    private bool hasActivated = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasActivated || !other.CompareTag("Player")) return;

        var manager = CharacterManager.Instance;
        if (manager == null)
        {
            Debug.LogError("[TriggerUnlockSwitch] CharacterManager not found!");
            return;
        }

        bool unlocked = false;
        for (int i = 0; i < manager.CharacterCount; i++)
        {
            var character = manager.GetCharacterEntry(i);
            //var follow = character.character.GetComponent<CompanionFollow>();
            if (character.character.name == characterNameToUnlock)
            {
                if (!character.isUnlocked)
                {
                    character.isUnlocked = true;
                    Debug.Log($"[TriggerUnlockSwitch] Character '{characterNameToUnlock}' unlocked!");
                    unlocked = true;

                    //if (follow != null)
                    //{
                    //    follow.SetHasMetUp(true); // So they are allowed to follow later
                    //}

                    if (switchToUnlockedCharacter)
                    {
                        manager.SetActiveCharacter(i);
                    }

                    // Optional: Placeholders for future features
                    // Play unlock sound here
                    // Trigger cinematic camera movement here
                }
                else
                {
                    Debug.Log($"[TriggerUnlockSwitch] Character '{characterNameToUnlock}' was already unlocked.");
                }

                break;
            }
        }

        if (!unlocked)
        {
            Debug.LogWarning($"[TriggerUnlockSwitch] Character with name '{characterNameToUnlock}' not found in CharacterManager.");
        }

        hasActivated = true;
        if (oneTimeUse) gameObject.SetActive(false);
    }
}
