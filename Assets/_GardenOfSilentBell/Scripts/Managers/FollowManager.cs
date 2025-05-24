using System.Collections.Generic;
using UnityEngine;

public class FollowManager : MonoBehaviour
{
    public static FollowManager Instance { get; private set; }

    public List<CompanionFollow> companions = new List<CompanionFollow>();

    public bool isFollowEnabled = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        DontDestroyOnLoad(gameObject);
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

        comp.StopFollowing(); // Ensure they stop following if unregistered
        comp.SetFollowTarget(null); // Clear the follow target
    }

    public void ToggleFollowAll()
    {
        isFollowEnabled = !isFollowEnabled;

        foreach (var comp in companions)
        {
            if (!comp.hasMetUp) continue;

            comp.SetFollowTarget(CharacterManager.Instance?.activeCharacter?.transform);

            if (isFollowEnabled)
                comp.StartFollowing();
            else
                comp.StopFollowing();
        }

        Debug.Log($"[FollowManager] Follow toggled. Now set to: {isFollowEnabled}");
    }

    public void AssignFollowTargets()
    {
        var activeChar = CharacterManager.Instance?.activeCharacter?.transform;
        if (activeChar == null) return;

        companions.Clear();
        int order = 1;

        foreach (var entry in CharacterManager.Instance.Characters)
        {
            if (!entry.isUnlocked || entry.instance == CharacterManager.Instance.activeCharacter) 
                continue;

            var follower = entry.instance.GetComponent<CompanionFollow>();
            if (follower != null)
            {
                RegisterCompanion(follower);
                follower.SetFollowTarget(activeChar);
                follower.followDistance = 1f + order;
                follower.SetFollowOffset(new Vector2(-follower.followDistance, 0f)); // <-- Add this line
                order++;

                if (isFollowEnabled && follower.hasMetUp)
                    follower.StartFollowing();  // Only follow if toggle is ON
                else
                    follower.StopFollowing(); // Or explicitly stop if OFF

            }
        }

        //Debug.Log("[FollowManager] Assigned follow targets. Follow state: " + isFollowEnabled);
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

    public void UpdateFollowerOffsets(bool isFacingRight)
    {
        int order = 1;
        foreach (var comp in companions)
        {
            float offsetX = isFacingRight ? -comp.followDistance : comp.followDistance;
            comp.SetFollowOffset(new Vector2(offsetX, 0f));
            order++;
        }
    }
}
