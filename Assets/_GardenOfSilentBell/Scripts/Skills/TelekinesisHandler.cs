using UnityEngine;
using UnityEngine.InputSystem;  
public class TelekinesisHandler : MonoBehaviour
{
    private Camera mainCamera;
    private Rigidbody2D grabbedRb;
    private TelekinesisSO skillData;
    private bool isTelekinesisActive = false;

    public void Initialize(TelekinesisSO skillData)
    {
        this.skillData = skillData;
        mainCamera = Camera.main;
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
        if (!isTelekinesisActive || grabbedRb != null) return;

        Vector2 mouseWorldPos = mainCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        Vector2 playerPos = transform.position;

        Collider2D hit = Physics2D.OverlapPoint(mouseWorldPos, skillData.telekinesisLayer);
        if (hit != null && Vector2.Distance(playerPos, hit.transform.position) <= skillData.interactionRange)
        {
            grabbedRb = hit.attachedRigidbody;
            if (grabbedRb != null)
            {
                grabbedRb.gravityScale = 0f;
                grabbedRb.linearVelocity = Vector2.zero;
            }
        }
    }

    public void TryRelease()
    {
        if (grabbedRb != null)
        {
            grabbedRb.gravityScale = 1f;
            grabbedRb.linearVelocity = Vector2.zero; // Reset velocity to prevent immediate fall
            grabbedRb = null;
        }
    }

    private void Update()
    {
        if (!isTelekinesisActive || grabbedRb == null) return;

        Vector2 mouseWorldPos = mainCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        Vector2 playerPos = transform.position;

        if (Vector2.Distance(playerPos, mouseWorldPos) <= skillData.interactionRange)
        {
            Vector2 direction = mouseWorldPos - grabbedRb.position;
            grabbedRb.linearVelocity = direction * 10f;
        }
    }
}
