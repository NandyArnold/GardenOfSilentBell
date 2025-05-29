using System.Net;
using UnityEngine;


[RequireComponent(typeof(Rigidbody2D))]
public class MovementHandler : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;

    [Header("Jumping")]
    public float jumpForce = 7f;

    [Header("Ground Detection")]
    public LayerMask groundLayer;

    [Header("Pushing")]
    public float normalPushSpeed;
    public float heavyPushSpeed;
    //private float pushForce = 5f;

    [Header("Sprinting")]
    public float sprintSpeed = 8f;

    private Rigidbody2D rb;
    private bool isGrounded;
    private PlayerInputHandler inputHandler;
    private SpriteFlipper spriteFlipper;
    private InteractionHandler interactionHandler;
    private BigJumpHandler bigJumpHandler;
    private HeavyPushHandler heavyPushHandler;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteFlipper = GetComponent<SpriteFlipper>();
        inputHandler = GetComponent<PlayerInputHandler>();
        interactionHandler = GetComponent<InteractionHandler>();
        bigJumpHandler = GetComponent<BigJumpHandler>();
        heavyPushHandler = GetComponent<HeavyPushHandler>();
        if (spriteFlipper == null)
        {
            Debug.LogError("spriteFlipper is null!");
        }
    }



    public void ProcessMove(Vector2 input, bool isPushing = false, bool isSprinting = false)
    {
        if (interactionHandler.CurrentPushTarget != null &&
      interactionHandler.CurrentPushTarget.CompareTag("Heavy") &&
      heavyPushHandler != null)
        {
            normalPushSpeed = heavyPushSpeed;
        }
        else
        {
           
            //Debug.Log("[MovementHandler] Using normal push speed");
        }

        //Debug.Log($"[MovementHandler] moveSpeed: {moveSpeed}, pushMoveSpeed: {pushMoveSpeed}");
        float speed = isPushing ? normalPushSpeed : (isSprinting ? sprintSpeed : moveSpeed);
        //Debug.Log($"[MovementHandler] isPushing: {isPushing}, Using speed: {speed}");

        float targetSpeed = input.x * speed;
        rb.linearVelocity = new Vector2(targetSpeed, rb.linearVelocity.y);
        spriteFlipper?.Flip(input.x);

        if (isPushing && interactionHandler.CurrentPushTarget != null)
        {

            // Only push if there is horizontal input
            if (Mathf.Abs(input.x) > 0.01f)
            {
                Vector2 pushDirection = new Vector2(input.x, 0f).normalized;
                // Use the same speed as the player
                interactionHandler.CurrentPushTarget.Push(new Vector2(input.x, 0f), speed);

                //interactionHandler.CurrentPushTarget.Push(pushDirection, Mathf.Abs(speed));
            }
            else
            {
                // Stop the pushable object if no input
                interactionHandler.CurrentPushTarget.Push(Vector2.zero, 0f);
            }
        }
    }


    public void Jump()
    {
        if (isGrounded) { 
            if (bigJumpHandler != null)
                {
                    bigJumpHandler.DoBigJump();
                }
            else
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
                isGrounded = false; // Prevent double jump
            }
        }
    }



    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (IsGroundLayer(collision.gameObject))
        {
            isGrounded = true;
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (IsGroundLayer(collision.gameObject))
        {
            isGrounded = true;
            //Debug.Log("Touched ground");
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (IsGroundLayer(collision.gameObject))
        {
            isGrounded = false;
        }
    }

    private bool IsGroundLayer(GameObject obj)
    {
        return groundLayer == (groundLayer | (1 << obj.layer));
    }
}
