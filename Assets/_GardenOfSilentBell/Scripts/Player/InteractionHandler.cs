using UnityEngine;

public class InteractionHandler : MonoBehaviour
{
    public float interactionRange = 1.5f;
    public LayerMask interactableLayer;

    public bool IsPushing { get; private set; }
    private Transform heldObject;
    public Transform pushAnchor;

    public void TryInteract()
    {
        Collider2D hit = Physics2D.OverlapCircle(transform.position, interactionRange, interactableLayer);
        if (hit != null)
        {
            Debug.Log($"Interactable found: {hit.name}, by {gameObject.name}");
            if (hit.TryGetComponent(out IInteractable interactable))
            {
                interactable.Interact();
            }
            else if (hit.TryGetComponent(out PushableObject pushable))
            {
                TryPushPull(pushable.transform); // Pass transform directly
            }
        }
        else
        {
            Debug.Log("No interactable in range.");
        }
    }

    public void HandleInputInteraction()
    {
        TryInteract();
    }


    public void TryPushPull(Transform objectToPush = null)
    {
        if (IsPushing)
        {
            if (heldObject != null)
                heldObject.SetParent(null);

            heldObject = null;
            IsPushing = false;
        }
        else if (objectToPush != null)
        {
            heldObject = objectToPush;
            heldObject.SetParent(pushAnchor);
            heldObject.localPosition = Vector3.zero;
            IsPushing = true;
        }
    }

}
