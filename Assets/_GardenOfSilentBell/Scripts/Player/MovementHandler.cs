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
    public float pushMoveSpeed = 2.5f;

    private Rigidbody2D rb;
    private bool isGrounded;
    private PlayerInputHandler inputHandler;
    private SpriteFlipper spriteFlipper;
    private InteractionHandler interactionHandler;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteFlipper = GetComponent<SpriteFlipper>();
        inputHandler = GetComponent<PlayerInputHandler>();
        interactionHandler = GetComponent<InteractionHandler>();
        if (spriteFlipper == null)
        {
            Debug.LogError("spriteFlipper is null!");
        }
    }

    //private void Update()
    //{
    //    if (inputHandler.InteractPressed)
    //    {
    //        interactionHandler.TryInteract();
    //    }
    //}
    void FixedUpdate()
    {
        float move = inputHandler.MovementInput.x;
        rb.linearVelocity = new Vector2(move * moveSpeed, rb.linearVelocity.y);

        spriteFlipper?.Flip(move);
        //Debug.Log($"FixedUpdate move input: {move}");
    }


    public void ProcessMove(Vector2 input, bool isPushing = false)
    {
        Debug.Log($"[MovementHandler] moveSpeed: {moveSpeed}, pushMoveSpeed: {pushMoveSpeed}");
        float speed = isPushing ? pushMoveSpeed : moveSpeed;
        Debug.Log($"[MovementHandler] isPushing: {isPushing}, Using speed: {speed}");

        float targetSpeed = input.x * speed;
        rb.linearVelocity = new Vector2(targetSpeed, rb.linearVelocity.y);
    }

    public void Jump()
    {
        if (isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            isGrounded = false; // Prevent double jump
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
