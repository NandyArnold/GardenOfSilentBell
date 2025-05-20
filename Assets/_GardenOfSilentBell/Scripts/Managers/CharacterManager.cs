using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class CharacterManager : MonoBehaviour
{
    public static CharacterManager Instance { get; private set; }

    [System.Serializable]
    public class CharacterEntry
    {
        public GameObject character;
        public bool isUnlocked;
    }

    [SerializeField] private List<CharacterEntry> characters = new List<CharacterEntry>();

    private int activeCharacterIndex = 0;

    public GameObject ActiveCharacter => characters[activeCharacterIndex].character;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // Activate the first unlocked character
        for (int i = 0; i < characters.Count; i++)
        {
            if (characters[i].isUnlocked)
            {
                SetActiveCharacter(i);
                break;
            }
        }
    }

    public void SwitchCharacter()
    {
        if (characters.Count == 0) return;

        int startIndex = activeCharacterIndex;
        int attempts = 0;

        do
        {
            activeCharacterIndex = (activeCharacterIndex + 1) % characters.Count;
            attempts++;

            if (characters[activeCharacterIndex].isUnlocked)
            {
                SetActiveCharacter(activeCharacterIndex);
                return;
            }

        } while (attempts < characters.Count && activeCharacterIndex != startIndex);

        Debug.LogWarning("No unlocked characters to switch to.");
    }

    public void SetActiveCharacter(int index)
    {
        if (index < 0 || index >= characters.Count) return;

        // Disable old character input
        var oldChar = characters[activeCharacterIndex].character;
        var oldInput = oldChar.GetComponent<PlayerInput>();
        if (oldInput != null)
        {
            oldInput.DeactivateInput();
            oldInput.enabled = false;
        }

        var oldHandler = oldChar.GetComponent<PlayerInputHandler>();
        if (oldHandler != null)
            oldHandler.isActivePlayer = false;

        // Activate new character
        activeCharacterIndex = index;
        var newChar = characters[activeCharacterIndex].character;

        var newInput = newChar.GetComponent<PlayerInput>();
        if (newInput != null)
        {
            newInput.enabled = true;
            newInput.ActivateInput();

            // Optional: transfer device from oldInput if needed
            // newInput.SwitchCurrentControlScheme(oldInput.devices.ToArray());
        }

        var newHandler = newChar.GetComponent<PlayerInputHandler>();
        if (newHandler != null)
            newHandler.isActivePlayer = true;

        // Update camera
        if (CameraFollow.Instance != null)
        {
            CameraFollow.Instance.SetTarget(newChar.transform);
        }

        Debug.Log($"[CharacterManager] Active character set to: {newChar.name}");
    }
}
