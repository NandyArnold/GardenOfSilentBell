using UnityEngine;

public class CompanionFollow : MonoBehaviour
{
    public bool hasMetUp = false;
    public bool followOnMetUp = true;

    private void OnEnable() => FollowManager.Instance?.RegisterCompanion(this);
    private void OnDisable() => FollowManager.Instance?.UnregisterCompanion(this);
    private Transform targetToFollow;
    public Transform TargetToFollow
    {
        get => targetToFollow;
        set => targetToFollow = value;
    }
    public void SetHasMetUp(bool metUp)
    {
        hasMetUp = metUp;
        if (hasMetUp && followOnMetUp)
        {
            FollowManager.Instance?.StartFollowing(this);
        }
    }
}
