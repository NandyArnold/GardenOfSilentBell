using UnityEngine;

[RequireComponent(typeof(MovementHandler))]
public class BigJumpHandler : MonoBehaviour
{
    public float extraJumpForce = 10f;
    private MovementHandler movementHandler;

    private void Awake()
    {
        movementHandler = GetComponent<MovementHandler>();
    }

    public void DoBigJump()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null && IsGrounded())
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, extraJumpForce);
            Debug.Log($"{gameObject.name} did a BIG jump!");

        }
    }

    private bool IsGrounded()
    {
        // Match MovementHandler�s ground check logic
        return Physics2D.OverlapCircle(transform.position, 0.1f, movementHandler.groundLayer);
    }

    public void ResetJump() { /* reset jump to default */ }
}
