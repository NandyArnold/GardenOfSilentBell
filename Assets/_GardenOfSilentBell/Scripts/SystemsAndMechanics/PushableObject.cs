using UnityEngine;

public class PushableObject : MonoBehaviour
{
    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    public void SetBeingPushed(bool isPushed)
    {
        if (isPushed)
        {
            rb.linearVelocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Kinematic;
        }
        else
        {
            transform.SetParent(null);
            rb.bodyType = RigidbodyType2D.Dynamic;
        }
    }



    //void Update()
    //{
    //    if (transform.parent != null)
    //    {
    //        // When attached, override physics
    //        rb.linearVelocity = Vector2.zero;
    //        rb.bodyType = RigidbodyType2D.Kinematic;
    //    }
    //    else
    //    {
    //        // Restore physics
    //        rb.bodyType = RigidbodyType2D.Dynamic;
    //    }
    //}
}