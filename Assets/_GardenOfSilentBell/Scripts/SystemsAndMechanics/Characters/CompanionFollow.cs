using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class CompanionFollow : MonoBehaviour
{
    [Header("Follow Settings")]
    public float followDistance = 1.5f;
    public float moveSpeed = 5f;
    public Vector2 offset = new Vector2(-1.5f, 0f); // Offset behind the player

    [Header("Meet & Follow")]
    public bool autoFollowOnMeet = true;
    public bool hasMetUp = false;

    private bool isFollowing = false;

    public string followTargetId; // e.g. "hero"
    private Transform followTarget;
    private Rigidbody2D rb;
    private SpriteFlipper spriteFlipper;

    public bool IsFollowing => isFollowing;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteFlipper = GetComponent<SpriteFlipper>();
    }

    private void Start()
    {
        // On load, re-find the follow target by ID
        followTarget = CharacterManager.Instance.GetCharacterById(followTargetId)?.instance?.transform;
    }
    public void SetFollowTarget(Transform target)
    {
        followTarget = target;
    }

    public void SetHasMetUp(bool metUp)
    {
        hasMetUp = metUp;

        if (autoFollowOnMeet && hasMetUp)
        {
            StartFollowing();
        }

    
    }

    public void StartFollowing()
    {
       

        isFollowing = true;
        
    }

    public void StopFollowing()
    {
        isFollowing = false;
    }

    private void FixedUpdate()
    {
        if (!isFollowing || followTarget == null || !hasMetUp) return;

       

        

        Vector2 desiredPosition = (Vector2)followTarget.position + offset;
        Vector2 currentPosition = rb.position;
        Vector2 direction = (desiredPosition - currentPosition).normalized;

        rb.MovePosition(Vector2.MoveTowards(currentPosition, desiredPosition, moveSpeed * Time.fixedDeltaTime));

        // Face the target using your SpriteFlipper
        if (spriteFlipper != null)
        {
            spriteFlipper.Flip(direction.x);
        }

       
    }


    public void SetFollowOffset(Vector2 newOffset)
    {
        offset = newOffset;
    }
}
