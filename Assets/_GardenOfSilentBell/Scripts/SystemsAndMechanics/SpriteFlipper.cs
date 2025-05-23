using UnityEngine;

public class SpriteFlipper : MonoBehaviour
{
    public enum Facing { Right, Left }

    [SerializeField] private Facing defaultFacing = Facing.Right;
    private int baseDirectionMultiplier; // +1 or -1
    private int currentFacing;
    

    public bool disableFlip = false;

    void Awake()
    {
        baseDirectionMultiplier = defaultFacing == Facing.Right ? 1 : -1;
        currentFacing = baseDirectionMultiplier;
    }

    public void Flip(float moveDirection)
    {
        //Debug.Log($"Flip called with moveDirection: {moveDirection}");
        if (disableFlip) return;
        if (Mathf.Abs(moveDirection) < 0.01f) return;

        int desiredFacing = moveDirection > 0 ? 1 : -1;

        if (desiredFacing != currentFacing)
        {
            currentFacing = desiredFacing;
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Abs(scale.x) * baseDirectionMultiplier * currentFacing;
            transform.localScale = scale;
        }
    }
}
