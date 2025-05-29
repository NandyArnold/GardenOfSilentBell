
using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.TextCore.Text;



public class CharacterHUDManager : MonoBehaviour
{
    [SerializeField] Transform portraitContainer;
    [SerializeField] GameObject portraitPrefab;

    [SerializeField] Transform skillContainer;
    [SerializeField] GameObject skillPrefab;

    [SerializeField] CharacterPortraitData[] portraitDataArray;
    private Dictionary<string, Sprite> portraitDataLookup = new();

    //private Dictionary<string, CharacterPortraitUI> portraitLookup = new();

    void Start()
    {
        foreach (var data in portraitDataArray)
        {
            if (!portraitDataLookup.ContainsKey(data.characterId))
                portraitDataLookup[data.characterId] = data.portraitSprite;
        }

        UpdateCharacterBar();
        UpdateSkillBar();
    }

    private void OnEnable()
    {
        if (CharacterManager.Instance != null)
            CharacterManager.Instance.OnCharacterSwitched += UpdateHUD;
    }

    private void OnDisable()
    {
        if (CharacterManager.Instance != null)
            CharacterManager.Instance.OnCharacterSwitched -= UpdateHUD;
    }

    public void UpdateHUD(GameObject newCharacter)
    {
        UpdateCharacterBar();
        UpdateSkillBar();
    }

    void UpdateCharacterBar()
    {

        foreach (Transform child in portraitContainer)
            Destroy(child.gameObject);

        var chars = CharacterManager.Instance.Characters;
        foreach (var character in chars)
        {
            if (!character.isUnlocked) continue;

            var go = Instantiate(portraitPrefab, portraitContainer);
            var portrait = go.GetComponent<CharacterPortraitUI>();
            var portraitSprite = portraitDataLookup.TryGetValue(character.id, out var sprite)
                     ? sprite
                     : sprite; // fallback if needed

            portrait.Setup(portraitSprite, character.id, OnPortraitClicked);

        }
        //foreach (Transform child in portraitContainer)
        //    Destroy(child.gameObject);

        //var chars = CharacterManager.Instance.Characters;
        //foreach (var character in chars)
        //{
        //    if (!character.isUnlocked) continue;

        //    var go = Instantiate(portraitPrefab, portraitContainer);
        //    var portrait = go.GetComponent<CharacterPortraitUI>();
        //    portrait.Setup(character.portraitSprite,character.id, OnPortraitClicked);

        //    portraitLookup[character.id] = portrait;
        //}
    }

    void UpdateSkillBar()
    {
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
}


