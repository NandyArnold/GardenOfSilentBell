using UnityEngine;

public class Lever : MonoBehaviour, IInteractable
{
    public void Interact()
    {
        Debug.Log($"Lever pulled by {gameObject.name} at time {Time.time}");
        // Animate or open door here
    }
}
