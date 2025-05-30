
using UnityEngine;
using System.Linq;
using System.Collections;



public class CharacterHUDManager : MonoBehaviour
{
    [SerializeField] Transform portraitContainer;
    [SerializeField] private GameObject portraitPrefab;

    [SerializeField] Transform skillContainer;
    [SerializeField] GameObject skillPrefab;


    void Start()
    {
        Debug.Log("[HUD] portraitPrefab at Start: " + (portraitPrefab != null ? portraitPrefab.name : "NULL"));

        StartCoroutine(DelayedUpdateCharacterBar());

        //UpdateCharacterBar();
        UpdateSkillBar();
    }

    private void OnEnable()
    {
        if (CharacterManager.Instance != null)
            CharacterManager.Instance.OnCharacterSwitched += (go) => StartCoroutine(DeferredUpdateHUD(go));

        else
        {
            Debug.LogWarning("[HUD] CharacterManager.Instance is null in OnEnable. Waiting for it to initialize.");
            //StartCoroutine(WaitForCharacterManager());
        }
    }

    private void OnDisable()
    {
        if (CharacterManager.Instance != null)
            CharacterManager.Instance.OnCharacterSwitched -= UpdateHUD;
    }

    public void UpdateHUD(GameObject newCharacter)
    {
        Debug.Log("[HUD] CharacterHUDManager Start called");

        UpdateCharacterBar();
        Debug.Log($"[HUD] Updating HUD with character: {newCharacter?.name}");
        UpdateSkillBar();
    }

    void UpdateCharacterBar()
    {
        if (portraitPrefab == null)
        {
            Debug.LogError("[HUD] portraitPrefab is null. Did it get destroyed?");
            return;
        }
        foreach (Transform child in portraitContainer)
            Destroy(child.gameObject);

        var chars = CharacterManager.Instance.Characters;
        foreach (var character in chars)
        {
            if (!character.isUnlocked) continue;

            if (portraitPrefab == null)
            {
                Debug.LogError("[HUD] portraitPrefab is null. Cannot instantiate portrait.");
                return;
            }

            var go = Instantiate(portraitPrefab, portraitContainer);
            Debug.Log($"[HUD] Instantiated portrait for {character.id} - Active: {go.activeSelf}");
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

            Debug.Log($"[HUD] Setting portrait for {character.id} with sprite: {portraitData.portraitSprite.name}");

            
            portrait.Setup(portraitSprite, character.id, OnPortraitClicked);

            portrait.portraitImage.enabled = true;
            portrait.selectButton.interactable = true;

            var nameText = portrait.GetComponentInChildren<TMPro.TextMeshProUGUI>();
            if (nameText != null) nameText.enabled = true;

            var imageComponents = portrait.GetComponentsInChildren<UnityEngine.UI.Image>(true);
            foreach (var img in imageComponents) img.enabled = true;

        }
    }


    void UpdateSkillBar()
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
            CharacterManager.Instance.SetActiveCharacter(index);
    }
    IEnumerator DelayedUpdateCharacterBar()
    {
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
}


