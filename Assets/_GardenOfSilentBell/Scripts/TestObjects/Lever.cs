using UnityEngine;

public class Lever : MonoBehaviour, IInteractable
{
    public void Interact()
    {
        Debug.Log("Lever pulled!");
        // Animate or open door here
    }
}
