using UnityEngine;

public class Lockpick : MonoBehaviour, IInteractable

{
    public void Interact()
    {
        GameObject interactor = Object.FindFirstObjectByType<InteractionHandler>()?.gameObject;

        if (interactor != null && interactor.GetComponent<DeftHandsHandler>() != null)
        {
            Debug.Log($"{name} disarmed by {interactor.name}");
            gameObject.SetActive(false);
        }
        else
        {
            Debug.Log($"{name} could not be disarmed — missing DeftHandsHandler");
        }
    }
}
