using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    public Transform followTarget;
    public float followSpeed = 3f;
    public float repositionCooldown = 0.5f;

    private float lastTurnTime = 0f;

    private void Update()
    {
        if (!CharacterManager.Instance.HasMetUp) return;

        // Simple follow behind logic
        float dist = Vector2.Distance(transform.position, followTarget.position);
        if (dist > 1.5f)
        {
            transform.position = Vector2.MoveTowards(transform.position, followTarget.position, followSpeed * Time.deltaTime);
        }

        // Optional: Direction flipping with cooldown
        if (Time.time - lastTurnTime > repositionCooldown)
        {
            if (ShouldReposition())
            {
                lastTurnTime = Time.time;
                // Flip or adjust offset behind the player
            }
        }
    }

    private bool ShouldReposition()
    {
        // Add logic to determine if player changed direction
        return true;
    }
}
