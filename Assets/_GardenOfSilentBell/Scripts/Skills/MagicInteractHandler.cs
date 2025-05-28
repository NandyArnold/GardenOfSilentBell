using UnityEngine;

public class MagicInteractHandler : MonoBehaviour
{
    public float interactRange = 5f;
    public LayerMask magicInteractLayer;

    private GameObject lastHighlighted;
    public Material glowMaterial; // Assign in Inspector
    private Material originalMaterial;
    private GameObject glowObject;
    private Collider2D currentHighlight;
    public void TryMagicInteract()
    {
        Debug.Log($"{name} trying to magic interact with OverlapCircle...");

        // Restore material on previous
        if (lastHighlighted != null)
        {
            var sr = lastHighlighted.GetComponent<SpriteRenderer>();
            if (sr != null && originalMaterial != null)
            {
                sr.material = originalMaterial;
            }
            lastHighlighted = null;
        }

        Collider2D hit = Physics2D.OverlapCircle(transform.position, interactRange, magicInteractLayer);

        if (hit != null)
        {
            Debug.Log($"Overlap hit: {hit.name}");

            var sr = hit.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                originalMaterial = sr.material;
                ShowGlow(hit.gameObject);
                lastHighlighted = hit.gameObject;
            }
            else
            {
                Debug.LogWarning($"No SpriteRenderer on {hit.name}");
            }

            if (hit.TryGetComponent<IMagicInteractable>(out var target))
            {
                Debug.Log($"Found IMagicInteractable: {target}");
                target.MagicInteract();
                Debug.Log($"{name} magically interacted with {target}");
            }
            else
            {
                Debug.Log($"{hit.name} does not implement IMagicInteractable.");
            }
        }
        else
        {
            Debug.Log("No magic interactable found in OverlapCircle.");
        }
        HideGlow();
    }


    public void HighlightMagicTarget()
    {
        Collider2D found = Physics2D.OverlapCircle(
            transform.position + Vector3.right * Mathf.Sign(transform.localScale.x),
            interactRange,
            magicInteractLayer
        );

        if (found != currentHighlight)
        {
            HideGlow(); // remove old glow

            if (found != null)
            {
                ShowGlow(found.gameObject);
                currentHighlight = found;
            }
            else
            {
                currentHighlight = null;
            }
        }
    }
    private void OnDrawGizmosSelected()
    {
        // Optional: visualize the interaction range in Scene view
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, interactRange);
    }


    private void ShowGlow(GameObject target)
    {
        if (glowObject == null)
        {
            glowObject = new GameObject("Glow");
            glowObject.AddComponent<SpriteRenderer>();
        }

        glowObject.transform.SetParent(target.transform);
        glowObject.transform.localPosition = Vector3.zero;

        var targetSR = target.GetComponent<SpriteRenderer>();
        var glowSR = glowObject.GetComponent<SpriteRenderer>();
        glowSR.material = glowMaterial;
        glowSR.sprite = targetSR.sprite;
        glowSR.color = new Color(1f, 1f, 1f, 0.6f);
        glowSR.sortingLayerID = targetSR.sortingLayerID;
        glowSR.sortingOrder = targetSR.sortingOrder - 1;

        // Slightly enlarge the glow
        glowObject.transform.localScale = Vector3.one * 1.2f;

        glowObject.SetActive(true);
    }


    private void HideGlow()
    {
        if (glowObject != null)
        {
            glowObject.transform.SetParent(null);
            glowObject.SetActive(false);
        }
    }
}
