using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;
using System.Linq;

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

        //foreach (var entry in characters)
        //{
        //    var input = entry.character.GetComponent<PlayerInput>();
        //    if (input != null)
        //    {
        //        input.enabled = false; // Disable until activated
        //    }
        //}

        //// Activate the first unlocked character
        //for (int i = 0; i < characters.Count; i++)
        //{
        //    if (characters[i].isUnlocked)
        //    {
        //        SetActiveCharacter(i);
        //        break;
        //    }
        //}
    }
    private void Start()
    {
        foreach (var entry in characters)
        {
            var input = entry.character.GetComponent<PlayerInput>();
            if (input != null)
            {
                input.DeactivateInput();
                input.enabled = false; // truly ensure it's off
            }
        }

        // Now initialize only the first active one
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

        // === CLEAN UP OLD CHARACTER ===
        if (activeCharacterIndex >= 0)
        {
            var oldChar = characters[activeCharacterIndex].character;
            var oldInput = oldChar.GetComponent<PlayerInput>();
            var oldHandler = oldChar.GetComponent<PlayerInputHandler>();

            if (oldHandler != null)
                oldHandler.isActivePlayer = false;

            if (oldInput != null)
            {
                if (oldInput.user.valid)
                    oldInput.user.UnpairDevices();

                oldInput.DeactivateInput();
                oldInput.enabled = false;
            }
        }

        // === ACTIVATE NEW CHARACTER ===
        activeCharacterIndex = index;
        var newChar = characters[activeCharacterIndex].character;
        var newInput = newChar.GetComponent<PlayerInput>();
        var newHandler = newChar.GetComponent<PlayerInputHandler>();

        if (newInput != null)
        {
            // IMPORTANT: Enable before doing anything
            newInput.enabled = true;
            newInput.ActivateInput();

            // Clear any previous pairing
            if (newInput.user.valid)
                newInput.user.UnpairDevices();

            // Manually create user if needed
            if (!newInput.user.valid)
            {
                var newUser = InputUser.CreateUserWithoutPairedDevices();
                newUser.AssociateActionsWithUser(newInput.actions);
                InputUser.PerformPairingWithDevice(Keyboard.current, newUser);
                InputUser.PerformPairingWithDevice(Mouse.current, newUser);
            }
            else
            {
                InputUser.PerformPairingWithDevice(Keyboard.current, newInput.user);
                InputUser.PerformPairingWithDevice(Mouse.current, newInput.user);
            }

            // Only try to switch control scheme *after* devices are paired and user is valid
            if (newInput.user.valid && !newInput.user.hasMissingRequiredDevices)
            {
                newInput.SwitchCurrentControlScheme("Keyboard&Mouse", Keyboard.current, Mouse.current);
            }
            else
            {
                Debug.LogWarning($"[CharacterManager] Could not switch control scheme for {newChar.name} — user not valid or missing devices");
            }
        }

        if (newHandler != null)
            newHandler.isActivePlayer = true;

        if (CameraFollow.Instance != null)
            CameraFollow.Instance.SetTarget(newChar.transform);

        Debug.Log($"[CharacterManager] Active character set to: {newChar.name}");
    }


}
