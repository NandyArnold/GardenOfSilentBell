
using UnityEngine;
using System.Linq;
using System.Collections;
using UnityEngine.TextCore.Text;
using UnityEngine.SceneManagement;



public class CharacterHUDManager : MonoBehaviour
{
    [SerializeField] Transform portraitContainer;
    [SerializeField] private GameObject portraitPrefab;

    [SerializeField] Transform skillContainer;
    [SerializeField] GameObject skillPrefab;

    public static CharacterHUDManager Instance;



private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        //Debug.Log("[HUD] CharacterHUDManager Instance created.");
        if (CharacterManager.Instance != null)
        {
            CharacterManager.Instance.OnCharacterSwitched += HandleCharacterSwitched;
        }
        else
        {
            Debug.LogWarning("[HUD] CharacterManager.Instance is null in Awake. Will retry in Start.");
        }
    }

    private void OnDestroy()
    {
        if (CharacterManager.Instance != null)
        {
            CharacterManager.Instance.OnCharacterSwitched -= HandleCharacterSwitched;
        }
    }
    void Start()
    {
        if (CharacterManager.Instance != null)
        {
            CharacterManager.Instance.OnCharacterSwitched -= HandleCharacterSwitched; // Avoid double hookup
            CharacterManager.Instance.OnCharacterSwitched += HandleCharacterSwitched;
        }
        else
        {
            Debug.LogError("[HUD] CharacterManager.Instance is STILL null in Start.");
        }

        //Debug.Log("[HUD] portraitPrefab at Start: " + (portraitPrefab != null ? portraitPrefab.name : "NULL"));

        StartCoroutine(DelayedUpdateCharacterBar());
        UpdateSkillBar();
    }

    public void UpdateHUD(GameObject newCharacter)
    {
        //Debug.Log("[HUD] CharacterHUDManager Start called");

        UpdateCharacterBar();

        if (newCharacter == null)
        {
            //Debug.LogWarning("[HUD] newCharacter is null or destroyed in UpdateHUD.");
            return;
        }

        if (!newCharacter) // This catches destroyed objects in Unity
        {
            //Debug.LogWarning("[HUD] newCharacter has been destroyed.");
            return;
        }

        //Debug.Log($"[HUD] Updating HUD with character: {newCharacter.name}");
        UpdateSkillBar();
    }

    public void UpdateCharacterBar()
    {
        if (GameBootstrapper.IsBootstrapping)
        {
            Debug.Log("[CharacterHUDManager] Skipping UpdateCharacterBar during bootstrap.");
            return;
        }
        var cm = CharacterManager.Instance;
        if (cm == null || cm.Characters == null || cm.Characters.Count == 0)
        {
            //Debug.LogWarning("[HUD] CharacterManager or Characters not initialized yet.");
            return;
        }
        if (portraitPrefab == null)
        {
            //Debug.LogError("[HUD] portraitPrefab is null. Did it get destroyed?");
            return;
        }
        foreach (Transform child in portraitContainer)

            Destroy(child.gameObject);

        var chars = CharacterManager.Instance.Characters;
        string activeId = CharacterManager.Instance.GetCharacterById(CharacterManager.Instance.activeCharacter.name)?.id;
        foreach (var character in chars)
        {
           
            if (cm == null || cm.Characters == null || cm.Characters.Count == 0)
            {
                //Debug.LogWarning("[HUD] CharacterManager not ready.");
                return;
            }
            if (!character.isUnlocked) continue;

            if (portraitPrefab == null)
            {
                //Debug.LogError("[HUD] portraitPrefab is null. Cannot instantiate portrait.");
                return;
            }

            var go = Instantiate(portraitPrefab, portraitContainer);
            //Debug.Log($"[HUD] Instantiated portrait for {character.id} - Active: {go.activeSelf}");
            var portrait = go.GetComponent<CharacterPortraitUI>();

            //  Dynamically extract from prefab's child script
            var portraitData = character.characterPrefab.GetComponentInChildren<CharacterPortraitData>();
            Sprite portraitSprite = portraitData != null ? portraitData.portraitSprite : null;

            if (portraitSprite == null)
            {
                Debug.LogWarning($"[HUD] No portrait sprite found for character ID: {character.id}");
                continue;
            }
            if (portraitData.portraitSprite == null)
            {
                Debug.LogWarning($"[HUD] CharacterPortraitData exists but sprite is null for ID: {character.id}");
                continue;
            }

            //Debug.Log($"[HUD] Setting portrait for {character.id} with sprite: {portraitData.portraitSprite.name}");


            //bool isActive = CharacterManager.Instance.activeCharacter != null &&
            //    CharacterManager.Instance.GetCharacterById(CharacterManager.Instance.activeCharacter.name)?.id == character.id;
            bool isActive = character.id == activeId;
            portrait.Setup(portraitSprite, character.id, OnPortraitClicked,isActive);

            portrait.portraitImage.enabled = true;
            portrait.selectButton.interactable = true;

            var nameText = portrait.GetComponentInChildren<TMPro.TextMeshProUGUI>();
            if (nameText != null) nameText.enabled = true;

            var imageComponents = portrait.GetComponentsInChildren<UnityEngine.UI.Image>(true);
            foreach (var img in imageComponents) img.enabled = true;

          

        }
    }


    public void UpdateSkillBar()
    {
        if (skillPrefab == null)
        {
            Debug.LogError("[HUD] skillPrefab is null. Cannot update skill bar.");
            return;
        }
        foreach (Transform child in skillContainer)
            Destroy(child.gameObject);

        var active = CharacterManager.Instance.activeCharacter;
        if (active == null) return;

        var skillManager = active.GetComponent<SkillManager>();
        if (skillManager == null) return;

        foreach (var skill in skillManager.assignedSkills)
        {
            var go = Instantiate(skillPrefab, skillContainer);
            var icon = go.GetComponent<SkillIconUI>();
            icon.Setup(skill);
        }
    }


    private void OnPortraitClicked(string id)
    {
        var data = CharacterManager.Instance.GetCharacterById(id);
        if (data == null) return;

        var index = CharacterManager.Instance.Characters.ToList().FindIndex(c => c.id == id);
        if (index >= 0)
        Debug.Log($"[HUD] Portrait clicked for character: {id}");
        CharacterManager.Instance.SetActiveCharacter(index);
    }


    IEnumerator DelayedUpdateCharacterBar()
    {
        while (GameBootstrapper.IsBootstrapping)
            yield return null;
        if (portraitPrefab == null)
        {
            Debug.LogError("[HUD] portraitPrefab is null in coroutine. It was likely destroyed or unassigned.");
        }
        yield return null; // or WaitForSeconds(0.1f);
        if (portraitPrefab == null)
        {
            Debug.LogError("[HUD] portraitPrefab is null in coroutine. It was likely destroyed or unassigned.");
        }
        UpdateCharacterBar();
    }

    private IEnumerator DeferredUpdateHUD(GameObject go)
    {
        yield return null; // wait one frame
        UpdateHUD(go);
    }

    public void InitHUD()
    {
        //Debug.Log("[HUD] Initializing CharacterHUDManager");
        if (portraitPrefab == null)
        {
            Debug.LogError("[HUD] portraitPrefab is not assigned in InitHUD.");
            return;
        }
        UpdateCharacterBar();
        UpdateSkillBar();
    }

    private void HandleCharacterSwitched(GameObject newCharacter)
    {
        //Debug.Log("[HUD] Handling character switch event.");
        UpdateHUD(newCharacter);
    }
}


