using UnityEngine;

public class Torch : MonoBehaviour, IMagicInteractable
{
    public void MagicInteract()
    {
        Debug.Log($"{name} has been extinguished.");
        gameObject.SetActive(false);
    }
}
