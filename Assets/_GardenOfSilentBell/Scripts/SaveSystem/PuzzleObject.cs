using UnityEngine;

public class PuzzleObject : MonoBehaviour
{
    public string objectId; // Must be unique per object per scene

    public SceneObjectState CaptureState()
    {
        return new SceneObjectState
        {
            objectId = this.objectId,
            position = transform.position,
            rotation = transform.rotation,
            scale = transform.localScale,
            isActive = gameObject.activeSelf,
            isDestroyed = false // or whatever logic you need
        };
    }

    public void ApplyState(SceneObjectState state)
    {
        transform.position = state.position;
        transform.rotation = state.rotation;
        transform.localScale = state.scale;
        gameObject.SetActive(state.isActive);

        // Apply any other state-specific logic here
        // For example, if this is a switch, set its switched state
        // If it's a movable object, ensure it's in the right position
    }
}
