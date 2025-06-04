using UnityEngine;

[RequireComponent(typeof(MovementHandler))]
public class BigJumpHandler : MonoBehaviour
{
    public float extraJumpForce = 10f;
    private MovementHandler movementHandler;
    private Rigidbody2D rb;

    private void Awake()
    {
        movementHandler = GetComponent<MovementHandler>();
    }

    private void Start()
    {

        rb = GetComponent<Rigidbody2D>();
    }

    public void DoBigJump()
    {
        if (rb != null && movementHandler.isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, extraJumpForce);
            Debug.Log($"{gameObject.name} did a BIG jump!");

        }
    }

    public void ResetJump() { /* reset jump to default */ }
}
