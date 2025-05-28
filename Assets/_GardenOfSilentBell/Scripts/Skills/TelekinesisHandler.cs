using UnityEngine;
using UnityEngine.InputSystem;  
public class TelekinesisHandler : MonoBehaviour
{
    private Camera mainCamera;
    private Rigidbody2D grabbedRb;
    private Collider2D playerCollider;
    private Collider2D grabbedObjectCollider;


    private TelekinesisSO skillData;
    private bool isTelekinesisActive = false;

    private LineRenderer lineRenderer;
    [SerializeField] private Transform lineStartPoint;

    public bool IsGrabbing => grabbedRb != null;
    public void Initialize(TelekinesisSO skillData)
    {
        this.skillData = skillData;
        mainCamera = Camera.main;
        playerCollider = GetComponent<Collider2D>();

        // Try to find LineRenderer if already on object
        lineRenderer = GetComponent<LineRenderer>();
        if (lineRenderer == null)
        {
            lineRenderer = gameObject.AddComponent<LineRenderer>();
            lineRenderer.positionCount = 2;
            lineRenderer.startWidth = 0.05f;
            lineRenderer.endWidth = 0.05f;
            lineRenderer.material =Resources.Load<Material>("GlowBeam_Material");
            lineRenderer.startColor = Color.cyan;
            lineRenderer.endColor = Color.cyan;
            lineRenderer.enabled = false;
        }
    }

    public void EnableTelekinesis()
    {
        isTelekinesisActive = true;
    }

    public void DisableTelekinesis()
    {
        isTelekinesisActive = false;
        TryRelease();
    }

    public void TryGrab()
    {
        if (!isTelekinesisActive || grabbedRb != null)
        {
            Debug.Log("Telekinesis not active or already grabbing.");
            return;
        }

        Vector2 mouseWorldPos = mainCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        Vector2 playerPos = transform.position;

        Collider2D hit = Physics2D.OverlapPoint(mouseWorldPos, skillData.telekinesisLayer);
        if (hit != null && Vector2.Distance(playerPos, hit.transform.position) <= skillData.interactionRange)
        {
            grabbedRb = hit.attachedRigidbody;
            grabbedObjectCollider = hit;

            if (grabbedRb != null)
            {
                Debug.Log("Grabbing object: " + grabbedRb.name);

                // Ignore collisions with player
                if (playerCollider != null && grabbedObjectCollider != null)
                {
                    Physics2D.IgnoreCollision(grabbedObjectCollider, playerCollider, true);
                }

                grabbedRb.gravityScale = 0f;
                grabbedRb.linearVelocity = Vector2.zero;

                // Show beam
                lineRenderer.enabled = true;
            }
        }
        else
        {
            Debug.Log("No valid object to grab or out of range.");
        }
    }

    public void TryRelease()
    {
        if (grabbedRb != null)
        {
            if (playerCollider != null && grabbedObjectCollider != null)
            {
                Physics2D.IgnoreCollision(grabbedObjectCollider, playerCollider, false);
            }

            grabbedRb.gravityScale = 1f;
            grabbedRb.linearVelocity = Vector2.zero; // Reset velocity to prevent immediate fall
            grabbedRb = null;

            grabbedObjectCollider = null;

            // Hide beam
            lineRenderer.enabled = false;
        }
    }

    private void Update()
    {
        if (!isTelekinesisActive || grabbedRb == null) return;

        Vector2 mouseWorldPos = mainCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        Vector2 playerPos = transform.position;

        Vector2 dir = (mouseWorldPos - playerPos).normalized;
        float clampedDistance = Mathf.Min(Vector2.Distance(playerPos, mouseWorldPos), skillData.interactionRange);
        Vector2 targetPos = playerPos + dir * clampedDistance;

        grabbedRb.linearVelocity = (targetPos - grabbedRb.position) * 10f;

        //Vector2 velocity = (targetPos - grabbedRb.position) * 10f;
        //grabbedRb.linearVelocity = velocity;
        if (lineRenderer.enabled)
        {
            lineRenderer.SetPosition(0, lineStartPoint != null ? lineStartPoint.position : transform.position);
            lineRenderer.SetPosition(1, grabbedRb.transform.position);
        }
        if (lineRenderer.material != null)
        {
            float glow = Mathf.PingPong(Time.time * 2f, 1f) + 1f; // Range 1–2
            lineRenderer.material.SetFloat("_GlowStrength", glow);
        }
    }

}
