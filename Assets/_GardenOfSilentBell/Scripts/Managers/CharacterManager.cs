using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class CharacterManager : MonoBehaviour
{
    public static CharacterManager Instance { get; private set; }

    [SerializeField] private List<PlayerController> allCharacters;
    private int currentIndex = 0;

    public PlayerController ActiveCharacter => allCharacters[currentIndex];

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        SetActiveCharacter(0); // Start with first unlocked
    }

    public void SwitchCharacter()
    {
        int startIndex = currentIndex;
        int nextIndex = (currentIndex + 1) % allCharacters.Count;

        while (nextIndex != startIndex)
        {
            if (allCharacters[nextIndex].isUnlocked)
            {
                SetActiveCharacter(nextIndex);
                return;
            }
            nextIndex = (nextIndex + 1) % allCharacters.Count;
        }

        // Fallback to original
        if (allCharacters[startIndex].isUnlocked)
            SetActiveCharacter(startIndex);
    }

    public void SetActiveCharacter(int index)
    {
        if (index < 0 || index >= allCharacters.Count) return;

        // Deactivate old character
        var oldChar = allCharacters[currentIndex];
        var oldInput = oldChar.GetComponent<PlayerInput>();
        if (oldInput != null)
        {
            oldInput.DeactivateInput();
            oldInput.enabled = false;
        }

        // Activate new character
        currentIndex = index;
        var newChar = allCharacters[currentIndex];
        var newInput = newChar.GetComponent<PlayerInput>();
        if (newInput != null)
        {
            newInput.enabled = true;

            var newInputHandler = newChar.GetComponent<PlayerInputHandler>();
            if (newInputHandler != null)
                newInputHandler.isActivePlayer = true;

            var oldInputHandler = oldChar.GetComponent<PlayerInputHandler>();
            if (oldInputHandler != null)
                oldInputHandler.isActivePlayer = false;

            // Transfer control scheme + devices
            var oldDevices = PlayerInput.all[0].devices; // assume only 1 player total
            newInput.ActivateInput();
            newInput.SwitchCurrentControlScheme(oldDevices.ToArray());
        }

        // Update camera follow
        if (CameraFollow.Instance != null)
        {
            CameraFollow.Instance.SetTarget(newChar.transform);
        }

        Debug.Log($"[CharacterManager] Active character set to: {newChar.name}");
    }

}
