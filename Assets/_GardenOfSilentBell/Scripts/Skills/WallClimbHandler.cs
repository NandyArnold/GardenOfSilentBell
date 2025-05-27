using UnityEngine;

public class WallClimbHandler : MonoBehaviour
{
    public float climbSpeed = 3f;
    public LayerMask climbableLayer;

    private Rigidbody2D rb;
    private bool isClimbing;

    private void Awake() => rb = GetComponent<Rigidbody2D>();

    private void Update()
    {
        if (isClimbing)
        {
            float inputY = Input.GetAxisRaw("Vertical"); // Replace with Input System later
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, inputY * climbSpeed);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (((1 << collision.gameObject.layer) & climbableLayer) != 0)
        {
            isClimbing = true;
            rb.gravityScale = 0;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (((1 << collision.gameObject.layer) & climbableLayer) != 0)
        {
            isClimbing = false;
            rb.gravityScale = 1;
        }
    }
}
