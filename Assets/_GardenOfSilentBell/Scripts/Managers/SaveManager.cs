using UnityEngine;
using System.Collections.Generic;
using System.IO;
using static CharacterManager;

[System.Serializable]
public class GameSaveData
{
    public string currentScene;
    public List<CharacterSaveData> characterStates;
}

[System.Serializable]
public class CharacterSaveData
{
    public string id;
    public bool isUnlocked;
    public Vector2 savedPosition;
    public bool isActive;
}

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance;
    private string savePath => Path.Combine(Application.persistentDataPath, "save.json");

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

    public void SaveGame()
    {
        GameSaveData saveData = new GameSaveData
        {
            currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name,
            characterStates = new List<CharacterSaveData>()
        };

        foreach (var character in CharacterManager.Instance.Characters)
        {
            saveData.characterStates.Add(new CharacterSaveData
            {
                id = character.id,
                isUnlocked = character.isUnlocked,
                savedPosition = character.instance != null
                    ? (Vector2)character.instance.transform.position
                    : character.lastPosition,
                isActive = character.instance == CharacterManager.Instance.activeCharacter
            });
        }

        File.WriteAllText(savePath, JsonUtility.ToJson(saveData));
        Debug.Log("Game saved.");
    }

    public void LoadGame()
    {
        if (!File.Exists(savePath))
        {
            Debug.LogWarning("No save file found.");
            return;
        }

        string json = File.ReadAllText(savePath);
        GameSaveData saveData = JsonUtility.FromJson<GameSaveData>(json);

        CharacterManager.Instance.LoadCharacterStates(saveData.characterStates);
        // Load the scene by name if needed
        Debug.Log("Game loaded.");
    }

    public bool HasSave()
    {
        return File.Exists(savePath);
    }

    public string GetLastSceneName()
    {
        if (!File.Exists(savePath)) return "IntroScene";
        string json = File.ReadAllText(savePath);
        GameSaveData saveData = JsonUtility.FromJson<GameSaveData>(json);
        return saveData.currentScene;
    }
}
