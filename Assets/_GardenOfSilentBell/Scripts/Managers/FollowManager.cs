using System.Collections.Generic;
using UnityEngine;

public class FollowManager : MonoBehaviour
{
    public static FollowManager Instance { get; private set; }

    private List<CompanionFollow> companions = new List<CompanionFollow>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    // Register companion so it's part of follow logic
    public void RegisterCompanion(CompanionFollow comp)
    {
        if (!companions.Contains(comp))
            companions.Add(comp);
    }

    public void UnregisterCompanion(CompanionFollow comp)
    {
        if (companions.Contains(comp))
            companions.Remove(comp);
    }

    public void ToggleFollowAll()
    {
        foreach (var comp in companions)
        {
            if (!comp.hasMetUp) continue;

            if (comp.IsFollowing)
            {
                comp.StopFollowing();
            }
            else
            {
                comp.StartFollowing();
            }
        }

        Debug.Log("[FollowManager] Toggled follow state for all companions.");
    }

    public void AssignFollowTargets()
    {
        var activeChar = CharacterManager.Instance?.ActiveCharacter?.transform;
        if (activeChar == null) return;

        int order = 1;
        foreach (var entry in CharacterManager.Instance.Characters)
        {
            if (!entry.isUnlocked || entry.character == CharacterManager.Instance.ActiveCharacter) continue;

            var follower = entry.character.GetComponent<CompanionFollow>();
            if (follower != null)
            {
                RegisterCompanion(follower);
                follower.SetFollowTarget(activeChar);

                // Set follow offset (future: use UI portrait order instead of list order)
                float offsetX = -order * 0.75f; // space them out behind
                follower.SetFollowOffset(new Vector2(offsetX, 0f));

                if (follower.hasMetUp && !follower.IsFollowing)
                    follower.StartFollowing();

                order++;
            }
        }
    }

    /// <summary>
    /// Called when a character is unlocked to refresh their follow state.
    /// </summary>
    public void RefreshFollowStates()
    {
        AssignFollowTargets(); // Ensure targets are up-to-date
        foreach (var comp in companions)
        {
            if (comp.hasMetUp && !comp.IsFollowing)
            {
                comp.StartFollowing(); // If already met, restart following if allowed
            }
        }
    }
}
