using UnityEngine;

[RequireComponent(typeof(InteractionHandler))]
public class HeavyPushHandler : MonoBehaviour
{
    public string heavyTag = "Heavy";
    private InteractionHandler interactionHandler;

    private void Awake()
    {
        interactionHandler = GetComponent<InteractionHandler>();
    }

    public void TryHeavyPush()
    {
        Collider2D hit = Physics2D.OverlapCircle(transform.position, interactionHandler.interactionRange, interactionHandler.interactableLayer);

        if (hit != null && hit.CompareTag(heavyTag))
        {
            if (hit.TryGetComponent(out PushableObject pushable))
            {
                interactionHandler.TryPushPull(pushable);
                Debug.Log($"{gameObject.name} started pushing heavy object: {hit.name}");
            }
        }
    }

    public void EnableHeavyPush() { /* allow pushing "Heavy" tagged objects */ }
    public void DisableHeavyPush() { /* revert to normal push */ }
}
