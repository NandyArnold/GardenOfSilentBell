using UnityEngine;

public class InteractionHandler : MonoBehaviour
{
    public float interactionRange = 1.5f;
    public LayerMask interactableLayer;

    public void TryInteract()
    {
        Collider2D hit = Physics2D.OverlapCircle(transform.position, interactionRange, interactableLayer);
        if (hit != null)
        {
            IInteractable interactable = hit.GetComponent<IInteractable>();
            interactable?.Interact();
        }
    }
}
