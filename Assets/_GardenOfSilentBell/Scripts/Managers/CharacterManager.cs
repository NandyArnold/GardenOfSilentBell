using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using System.Linq;

public class CharacterManager : MonoBehaviour
{
    public static CharacterManager Instance { get; private set; }

    [System.Serializable]
    public class CharacterData
    {
        public string id;                     // Unique ID (e.g. "mage", "knight")
        public GameObject characterPrefab;             // Reference to the .characterPrefab (not in scene!)
        [HideInInspector] public GameObject instance; // Instantiated runtime object
        public bool isUnlocked;
        public Vector2 lastPosition;
        public bool isActive;
    }

    [SerializeField] private List<CharacterData> characters = new List<CharacterData>();

    private int activeCharacterIndex = -1;
    public GameObject activeCharacter { get; private set; }

    public IReadOnlyList<CharacterData> Characters => characters;
    public int CharacterCount => characters.Count;
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
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        UnlockCharacter("StartingCharacter"); // or whatever ID you use
       
        var spawnPos = SpawnManager.Instance.GetStartSpawnPoint() ?? Vector2.zero;
        var instance = SpawnManager.Instance.SpawnCharacterById("StartingCharacter", spawnPos);
        var data = GetCharacterById("StartingCharacter");
        if (data != null)
        {
            data.instance = instance;
            data.lastPosition = spawnPos;
            SetActiveCharacter(characters.IndexOf(data));
        }
        //foreach (var data in characters)
        //{
        //    //if (data.isUnlocked)
        //    //{
        //    //    Vector2 spawnPos = data.lastPosition != Vector2.zero ? data.lastPosition : Vector2.zero;
        //    //    data.instance = Instantiate(data.characterPrefab, spawnPos, Quaternion.identity);
        //    //    data.instance.name = data.id;
        //    //    data.instance.SetActive(true);

        //    //    var input = data.instance.GetComponent<PlayerInput>();
        //    //    if (input != null)
        //    //    {
        //    //        input.DeactivateInput();
        //    //        input.enabled = false;
        //    //    }
        //    //}
        //}

        // Set active character if one is unlocked
        for (int i = 0; i < characters.Count; i++)
        {
            if (characters[i].isUnlocked)
            {
                SetActiveCharacter(i);
                break;
            }
        }
    }

    public void UnlockCharacter(string id)
    {
        var data = characters.FirstOrDefault(c => c.id == id);
        if (data != null && !data.isUnlocked)
        {
            data.isUnlocked = true;
            data.lastPosition = Vector2.zero;
            SaveManager.Instance?.SaveGame();
        }
    }

    public void SwitchCharacter()
    {
        if (characters.Count <= 1 || !CanSwitch)
        {
            Debug.LogWarning("[CharacterManager] Not enough characters or switching disabled.");
            return;
        }

        int originalIndex = activeCharacterIndex;
        int nextIndex = activeCharacterIndex;

        do
        {
            nextIndex = (nextIndex + 1) % characters.Count;
            if (characters[nextIndex].isUnlocked)
            {
                SetActiveCharacter(nextIndex);
                return;
            }
        } while (nextIndex != originalIndex);
    }

    public void SetActiveCharacter(int index)
    {
        if (index < 0 || index >= characters.Count)
        {
            Debug.LogWarning("[CharacterManager] Invalid character index");
            return;
        }

        foreach (var entry in characters)
        {
            if (entry?.instance == null) continue;

            var obj = entry.instance;
            var handler = obj.GetComponent<PlayerInputHandler>();
            var input = obj.GetComponent<PlayerInput>();
            var sprite = obj.GetComponent<SpriteRenderer>();

            if (handler != null)
            {
                handler.UnbindAllInputActions();
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
            if (sprite != null) sprite.sortingOrder = 9;
            if (input != null) input.enabled = false;
        }

        var selected = characters[index];
        if (selected?.instance == null) return;

        var selInput = selected.instance.GetComponent<PlayerInput>();
        var selHandler = selected.instance.GetComponent<PlayerInputHandler>();
        var selSprite = selected.instance.GetComponent<SpriteRenderer>();

        if (selInput != null) selInput.enabled = true;

        if (selInput != null && selInput.user.valid)
        {
            try
            {
                selInput.user.AssociateActionsWithUser(selInput.actions);
                selInput.user.ActivateControlScheme(selInput.defaultControlScheme);
                selInput.ActivateInput();
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"[CharacterManager] Could not activate input: {e.Message}");
            }
        }

        if (selSprite != null) selSprite.sortingOrder = 10;
        selected.instance.layer = LayerMask.NameToLayer("PlayerActive");

        if (selHandler != null)
        {
            selHandler.enabled = true;
            selHandler.isActivePlayer = true;
        }

        activeCharacterIndex = index;
        activeCharacter = selected.instance;

        var newCompanion = selected.instance.GetComponent<CompanionFollow>();
        if (newCompanion != null)
        {
            FollowManager.Instance?.UnregisterCompanion(newCompanion);
        }

        CameraFollow.Instance?.SetTarget(selected.instance.transform);
        FollowManager.Instance?.AssignFollowTargets();
    }

    public GameObject SpawnCharacter(string id, Vector2 position)
    {
        var data = characters.FirstOrDefault(c => c.id == id);
        if (data != null && data.characterPrefab != null)
        {
            GameObject instance = Instantiate(data.characterPrefab, position, Quaternion.identity);
            data.instance = instance;
            return instance;
        }
        return null;
    }

    public CharacterData GetCharacterData(int index)
    {
        if (index >= 0 && index < characters.Count)
            return characters[index];
        return null;
    }

    public CharacterData GetCharacterById(string id)
    {
        return characters.FirstOrDefault(c => c.id == id);
    }

    public void EnableSwitching() => CanSwitch = true;
    public void SetMetUp() => HasMetUp = true;

    public void SaveCharacterState()
    {
        foreach (var data in characters)
        {
            PlayerPrefs.SetInt($"CharacterUnlocked_{data.id}", data.isUnlocked ? 1 : 0);
            PlayerPrefs.SetFloat($"CharacterPosX_{data.id}", data.instance != null ? data.instance.transform.position.x : 0);
            PlayerPrefs.SetFloat($"CharacterPosY_{data.id}", data.instance != null ? data.instance.transform.position.y : 0);
        }
    }

    public void LoadCharacterState()
    {
        foreach (var data in characters)
        {
            data.isUnlocked = PlayerPrefs.GetInt($"CharacterUnlocked_{data.id}", 0) == 1;
            data.lastPosition = new Vector2(
                PlayerPrefs.GetFloat($"CharacterPosX_{data.id}", 0),
                PlayerPrefs.GetFloat($"CharacterPosY_{data.id}", 0)
            );
        }
    }

    public void LoadCharacterStates(List<CharacterSaveData> saveData)
    {
        foreach (var saved in saveData)
        {
            var character = characters.FirstOrDefault(c => c.id == saved.id);
            if (character != null)
            {
                character.isUnlocked = saved.isUnlocked;
                character.lastPosition = saved.savedPosition;
                character.isActive = saved.isActive;

                if (character.isUnlocked && character.characterPrefab != null)
                {
                    character.instance = Instantiate(character.characterPrefab, saved.savedPosition, Quaternion.identity);
                    character.instance.name = character.id;
                }
            }
        }

        // Set active character from loaded data
        for (int i = 0; i < characters.Count; i++)
        {
            if (characters[i].isActive)
            {
                SetActiveCharacter(i);
                break;
            }
        }
    }

    public void InitializeUnlockedCharacters()
    {
        foreach (var data in characters)
        {
            if (data.isUnlocked)
            {
                Vector2 spawnPos = data.lastPosition != Vector2.zero ? data.lastPosition : Vector2.zero;
                GameObject instance = SpawnManager.Instance.SpawnCharacterById(data.id, spawnPos);
                data.instance = instance;
              
            }
        }

        // Set the active character (you can skip if already restored from save)
        for (int i = 0; i < characters.Count; i++)
        {
            if (characters[i].isUnlocked && characters[i].isActive)
            {
                SetActiveCharacter(i);
                break;
            }
        }
    }

    public string GetActiveCharacterId()
    {
        if (activeCharacter == null) return null;

        var data = characters.Find(c => c.instance == activeCharacter);
        return data != null ? data.id : null;
    }
}
