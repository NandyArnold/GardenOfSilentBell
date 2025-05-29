
using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;


public class CharacterHUDManager : MonoBehaviour
{
    [SerializeField] Transform portraitContainer;
    [SerializeField] GameObject portraitPrefab;

    [SerializeField] Transform skillContainer;
    [SerializeField] GameObject skillPrefab;

    private Dictionary<string, CharacterPortraitUI> portraitLookup = new();

    void Start()
    {
        UpdateCharacterBar();
        UpdateSkillBar();
    }

    void OnEnable()
    {
        // Subscribe to switch character event
        CharacterManager.Instance.OnCharacterSwitched += UpdateHUD;
    }

    void OnDisable()
    {
        CharacterManager.Instance.OnCharacterSwitched -= UpdateHUD;
    }

    public void UpdateHUD()
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
            portrait.Setup(character.characterPrefab.GetComponent<SpriteRenderer>().sprite,
                           character.id, character.id,
                           character.instance == CharacterManager.Instance.activeCharacter);

            portraitLookup[character.id] = portrait;
        }
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
}

