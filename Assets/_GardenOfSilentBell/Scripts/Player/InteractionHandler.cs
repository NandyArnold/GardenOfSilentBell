using UnityEngine;

public class InteractionHandler : MonoBehaviour
{
    public float interactionRange = 1.5f;
    public LayerMask interactableLayer;


    public bool IsPushing { get; private set; }
    
    private PushableObject currentPushTarget;
   
    public PushableObject CurrentPushTarget => currentPushTarget;

    public void TryInteract()
    {
        Collider2D hit = Physics2D.OverlapCircle(transform.position, interactionRange, interactableLayer);
        if (hit == null)
        {
            Debug.Log("No interactable in range.");
            return;
        }

        Debug.Log($"Interactable found: {hit.name}, by {gameObject.name}");

        // Prioritize pushable objects
        if (hit.TryGetComponent(out PushableObject pushable))
        {
            TryPushPull(pushable);
        }
        else if (hit.TryGetComponent(out IInteractable interactable))
        {
            interactable.Interact();
        }
       
    }


    public void TryPushPull(PushableObject target)
    {
        if (IsPushing)
        {
            if (currentPushTarget != null)
            {
                currentPushTarget.SetBeingPushed(false); //  Revert to Kinematic
            }

            currentPushTarget = null;
            IsPushing = false;
            Debug.Log("Stopped pushing");
        }
        else
        {
            currentPushTarget = target;
            IsPushing = true;
            currentPushTarget.SetBeingPushed(true); //  Activate pushing mode
            Debug.Log("Started pushing: " + target.name);
        }
    }
}