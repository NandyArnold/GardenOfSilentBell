using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using System.Linq;

public class CharacterManager : MonoBehaviour
{
    public static CharacterManager Instance { get; private set; }

    [System.Serializable]
    public class CharacterEntry
    {
        public string characterName;
        public GameObject character;
        public bool isUnlocked;
    }

    public GameObject activeCharacter { get; private set; }

    public IReadOnlyList<CharacterEntry> Characters => characters;
    public int CharacterCount => characters.Count;

    [SerializeField] private List<CharacterEntry> characters = new List<CharacterEntry>();

    private int activeCharacterIndex = -1;

    public GameObject ActiveCharacter => activeCharacterIndex >= 0 ? characters[activeCharacterIndex].character : null;

    public bool CanSwitch { get; set; } = false;
    public bool HasMetUp { get; set; } = false;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        Debug.Log("[CharacterManager] Awake called");
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        foreach (var entry in characters)
        {
            var input = entry.character.GetComponent<PlayerInput>();
            if (input != null)
            {
                input.DeactivateInput();
                input.enabled = false;
            }
        }

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
        if (characters == null || characters.Count == 0)
        {
            Debug.LogError("[CharacterManager] Cannot switch character — character list is null or empty.");
            return;
        }

        if (activeCharacterIndex < 0 || activeCharacterIndex >= characters.Count)
        {
            Debug.LogWarning("[CharacterManager] Active character index is invalid, resetting to 0.");
            activeCharacterIndex = 0;
        }

        int originalIndex = activeCharacterIndex;
        int nextIndex = activeCharacterIndex;

        do
        {
            nextIndex = (nextIndex + 1) % characters.Count;

            if (characters[nextIndex] == null)
            {
                Debug.LogWarning($"[CharacterManager] Character at index {nextIndex} is null. Skipping.");
                continue;
            }

            if (characters[nextIndex].isUnlocked)
            {
                SetActiveCharacter(nextIndex);
                return;
            }

        } while (nextIndex != originalIndex);

        Debug.LogWarning("[CharacterManager] No unlocked characters to switch to.");
    }


    public void SetActiveCharacter(int index)
    {
        if (index < 0 || index >= characters.Count)
        {
            Debug.LogWarning("[CharacterManager] Invalid character index");
            return;
        }

        Debug.Log($"[CharacterManager] Attempting to set active character at index {index}");

        // Deactivate all characters
        foreach (var entry in characters)
        {
            if (entry?.character == null)
            {
                Debug.LogWarning("[CharacterManager] Skipping null character entry during deactivation");
                continue;
            }

            var obj = entry.character;
            var handler = obj.GetComponent<PlayerInputHandler>();
            var input = obj.GetComponent<PlayerInput>();
            var sprite = obj.GetComponent<SpriteRenderer>();

            if (handler != null)
            {
                handler.UnbindInputActions();
                handler.isActivePlayer = false;
                handler.enabled = false;
            }

            if (input != null && input.user.valid)
            {
                input.user.UnpairDevicesAndRemoveUser();
            }

            if (obj.TryGetComponent(out Collider2D collider))
            {
                collider.enabled = true;
            }

            obj.layer = LayerMask.NameToLayer("PlayerInactive");

            if (sprite != null)
            {
                sprite.sortingOrder = 9;
            }

            if (input != null)
            {
                input.enabled = false;
            }
        }

        var selectedEntry = characters[index];

        if (selectedEntry?.character == null)
        {
            Debug.LogError("[CharacterManager] Selected character is null!");
            return;
        }

        var selectedInput = selectedEntry.character.GetComponent<PlayerInput>();
        var selectedHandler = selectedEntry.character.GetComponent<PlayerInputHandler>();
        var selectedSprite = selectedEntry.character.GetComponent<SpriteRenderer>();

        if (selectedInput == null || selectedHandler == null)
        {
            Debug.LogError("[CharacterManager] Selected character missing PlayerInput or PlayerInputHandler");
            return;
        }

        selectedInput.enabled = true;

        if (!selectedInput.user.valid)
        {
            Debug.LogWarning($"[CharacterManager] Could not switch control scheme for {selectedEntry.character.name} — user not valid or missing devices");
        }
        else
        {
            try
            {
                selectedInput.user.AssociateActionsWithUser(selectedInput.actions);
                selectedInput.user.ActivateControlScheme(selectedInput.defaultControlScheme);
                selectedInput.ActivateInput();
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"[CharacterManager] Failed to activate control scheme for {selectedEntry.character.name}: {e.Message}");
            }
        }

        if (selectedSprite != null)
            selectedSprite.sortingOrder = 10;

        selectedEntry.character.layer = LayerMask.NameToLayer("PlayerActive");
        selectedHandler.enabled = true;
        selectedHandler.isActivePlayer = true;
        selectedHandler.BindInputActions();
        activeCharacterIndex = index;
        activeCharacter = selectedEntry.character;

        CameraFollow.Instance?.SetTarget(selectedEntry.character.transform);
        FollowManager.Instance?.AssignFollowTargets();

        Debug.Log($"[CharacterManager] Active character set to: {selectedEntry.character.name}");
    }


    public void EnableSwitching()
    {
        CanSwitch = true;
        Debug.Log("Switching now enabled.");
    }

    public void SetMetUp()
    {
        HasMetUp = true;
        Debug.Log("Characters have met up!");
    }
       

    public CharacterEntry GetCharacterEntry(int index)
    {
        if (index >= 0 && index < characters.Count)
        {
            return characters[index];
        }
        return null;
    }
   



}
