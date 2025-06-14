using UnityEngine;

public class PuzzleObject : MonoBehaviour
{
    public string objectId; // Must be unique per object per scene

    public SceneObjectState CaptureState()
    {
        return new SceneObjectState
        {
            objectId = objectId,
            position = transform.position,
            rotation = transform.rotation,
            scale = transform.localScale,
            isActive = gameObject.activeSelf,
            isDestroyed = this == null // or handled externally
        };
    }

    public void ApplyState(SceneObjectState state)
    {
        transform.position = state.position;
        transform.rotation = state.rotation;
        transform.localScale = state.scale;
        gameObject.SetActive(state.isActive);
    }
}
