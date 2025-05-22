using System.Collections.Generic;
using UnityEngine;

public class FollowManager : MonoBehaviour
{
    public static FollowManager Instance { get; private set; }

    private readonly List<CompanionFollow> registeredCompanions = new List<CompanionFollow>();
    private Dictionary<CompanionFollow, GameObject> companionObjects = new Dictionary<CompanionFollow, GameObject>();

    [Header("Follow Settings")]
    public float followSpeed = 5f;
    public float spacing = 1.5f;
    public Vector2 baseOffset = new Vector2(-1.5f, 0f);
    public bool globalFollowEnabled = true;

    private Transform followTarget;

    private void Awake()
    {
        if (Instance != null) Destroy(gameObject);
        Instance = this;
    }

    private void Update()
    {
        if (!globalFollowEnabled || followTarget == null) return;

        int order = 0;
        foreach (var companion in registeredCompanions)
        {
            if (companion == null || !companion.hasMetUp) continue;

            GameObject obj = companion.gameObject;
            Vector2 offset = baseOffset + new Vector2(-order * spacing, 0f);
            Vector2 targetPos = (Vector2)followTarget.position + offset;
            obj.transform.position = Vector2.Lerp(obj.transform.position, targetPos, followSpeed * Time.deltaTime);

            // Flip to face player
            Vector3 scale = obj.transform.localScale;
            scale.x = (followTarget.position.x < obj.transform.position.x) ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x);
            obj.transform.localScale = scale;

            order++;
        }
    }

    public void SetFollowTarget(Transform target)
    {
        followTarget = target;
    }

    public void RegisterCompanion(CompanionFollow companion)
    {
        if (!registeredCompanions.Contains(companion))
            registeredCompanions.Add(companion);
    }

    public void UnregisterCompanion(CompanionFollow companion)
    {
        if (registeredCompanions.Contains(companion))
            registeredCompanions.Remove(companion);
    }

    public void ToggleGlobalFollow()
    {
        globalFollowEnabled = !globalFollowEnabled;
    }

    public void StartFollowing(CompanionFollow companion)
    {
        if (!registeredCompanions.Contains(companion))
            RegisterCompanion(companion);

        companion.hasMetUp = true;
        // Follow begins automatically in Update() if globalFollowEnabled is true
    }

    public void StopAllFollowing()
    {
        globalFollowEnabled = false;
    }

    public void ResumeAllFollowing()
    {
        globalFollowEnabled = true;
    }

    public void ToggleIndividualFollow(CompanionFollow companion)
    {
        companion.hasMetUp = !companion.hasMetUp;
    }

    public void ToggleFollowForAll()
    {
        foreach (var comp in registeredCompanions)
        {
            if (!comp.hasMetUp) continue;
            comp.hasMetUp = false;
        }

        globalFollowEnabled = !globalFollowEnabled;
    }

    public void UpdateFollowTargets(Transform newTarget)
    {
        foreach (var companion in registeredCompanions)
        {
            companion.TargetToFollow = newTarget;
        }
    }
}
