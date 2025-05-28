using UnityEngine;

public class MagicFloatUp : MonoBehaviour, IMagicInteractable
{
  
    public void MagicInteract()
    {
        transform.position += Vector3.up * 2f;
        Debug.Log($"{name} floated up!");
    }
}

