using UnityEngine;

public class TriggerFollowZone : MonoBehaviour
{
    [Tooltip("Enable follow automatically on all unlocked but inactive characters upon entering.")]
    public bool triggerFollowOnEnter = true;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (!triggerFollowOnEnter) return;

        Debug.Log("[TriggerFollowZone] Player entered, activating nearby companions.");

        foreach (var entry in CharacterManager.Instance.Characters)
        {
            if (!entry.isUnlocked || entry.instance == null)
                continue;

            // Don't set follow target for the active character
            if (entry.instance == CharacterManager.Instance.activeCharacter)
                continue;

            var followComp = entry.instance.GetComponent<CompanionFollow>();
            if (followComp != null)
            {
                followComp.SetHasMetUp(true);
                followComp.SetFollowTarget(CharacterManager.Instance.activeCharacter.transform);
            }
        }
        SaveManager.Instance?.SaveGame(); // Save the game state after triggering follow

        FollowManager.Instance?.AssignFollowTargets();

        gameObject.SetActive(false); // Disable this trigger zone after use
    }
}
