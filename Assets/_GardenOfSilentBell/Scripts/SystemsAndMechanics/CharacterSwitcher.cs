// CharacterSwitcher.cs
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterSwitcher : MonoBehaviour
{
    void Update()
    {
        if (Keyboard.current.tabKey.wasPressedThisFrame)
        {
            Debug.Log("Trying to switch character");
            CharacterManager.Instance.SwitchCharacter();
        }
    }
}
