using UnityEngine;

public class MagicInteractHandler : MonoBehaviour
{
    public float interactRange = 5f;
    public LayerMask magicInteractLayer;

    public void TryMagicInteract()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.right * transform.localScale.x, interactRange, magicInteractLayer);
        if (hit.collider != null && hit.collider.TryGetComponent<IMagicInteractable>(out var target))
        {
            target.MagicInteract();
            Debug.Log($"{name} magically interacted with {target}");
        }
    }
}

public interface IMagicInteractable
{
    void MagicInteract();
}
