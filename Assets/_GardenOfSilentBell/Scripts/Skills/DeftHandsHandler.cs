using UnityEngine;

public class DeftHandsHandler : MonoBehaviour
{
    public float disarmRange = 2f;
    public LayerMask trapLayer;

    public void TryDisarm()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.right * transform.localScale.x, disarmRange, trapLayer);
        if (hit.collider != null && hit.collider.TryGetComponent<IDisarmable>(out var trap))
        {
            trap.Disarm();
            Debug.Log($"{name} disarmed: {trap}");
        }
    }
}
