using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PushableObject : MonoBehaviour, IInteractable
{
    private Rigidbody2D rb;
    public bool IsBeingPushed { get; private set; }
    public bool IsHeavy => gameObject.CompareTag("Heavy");

    //public float pushSpeed = 2f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Interact()
    {
        Debug.Log("Pushable marked to be pushed. Actual logic handled by player.");
        // Nothing to do here, just a marker
    }

    public void SetBeingPushed(bool isPushed)
    {
        IsBeingPushed = isPushed;
        if (isPushed)
        {
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        }
        else
        {
            rb.bodyType = RigidbodyType2D.Kinematic;
            // Optionally, reset velocity to zero
            rb.linearVelocity = Vector2.zero;
        }
    }

    //public void SetBeingPushed(bool isPushed)
    //{
    //    IsBeingPushed = isPushed;

    //    if (!isPushed)
    //    {
    //        rb.linearVelocity = Vector2.zero;
    //    }
    //}

    public void Push(Vector2 direction, float speed)
    {
        rb.linearVelocity = new Vector2(direction.x * speed, rb.linearVelocity.y);
    }
    //public void Push(Vector2 direction, float force)
    //{
    //    rb.AddForce(direction * force, ForceMode2D.Force);
    //}
}
