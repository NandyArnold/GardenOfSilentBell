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
            CompanionFollow followComp = entry.character.GetComponent<CompanionFollow>();
            followComp.SetHasMetUp(true);

            if (!entry.isUnlocked || entry.character == CharacterManager.Instance.ActiveCharacter)
                continue;

            if (followComp != null)
            {
                followComp.SetFollowTarget(CharacterManager.Instance.ActiveCharacter.transform);
            }
        }

        FollowManager.Instance?.AssignFollowTargets();

        Destroy(gameObject);
    }
}
