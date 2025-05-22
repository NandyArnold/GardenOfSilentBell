using UnityEngine;

public class FollowTriggerBox : MonoBehaviour
{
    [Tooltip("Name of the character that should start following when this is triggered.")]
    public string characterNameToFollow;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        var manager = CharacterManager.Instance;
        if (manager == null)
        {
            Debug.LogError("[FollowTriggerBox] CharacterManager not found!");
            return;
        }

        foreach (var entry in manager.Characters)
        {
            if (entry.character.name == characterNameToFollow && entry.isUnlocked)
            {
                var follow = entry.character.GetComponent<CompanionFollow>();
                if (follow != null)
                {
                    follow.SetHasMetUp(true); // This also starts following if followOnMetUp is true
                    Debug.Log($"[FollowTriggerBox] Companion '{characterNameToFollow}' met up and is now following.");
                }
                else
                {
                    Debug.LogWarning($"[FollowTriggerBox] Companion '{characterNameToFollow}' does not have a CompanionFollow component.");
                }

                return;
            }
        }

        Debug.LogWarning($"[FollowTriggerBox] Companion '{characterNameToFollow}' not found or not unlocked.");
    }
}
