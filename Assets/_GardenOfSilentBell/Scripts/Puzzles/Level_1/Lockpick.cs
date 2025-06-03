using UnityEngine;

public class Lockpick : MonoBehaviour, IInteractable

{
    public void Interact()
    {
        var interactors = FindObjectsByType<InteractionHandler>(FindObjectsSortMode.None);
        GameObject interactor = null;
        foreach (InteractionHandler handler in interactors)
        {
            if(handler.gameObject.name == "StartingCharacter")
            {
                interactor = handler.gameObject;
                break;
            }
        }

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
