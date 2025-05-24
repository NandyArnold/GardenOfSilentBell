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
public class ReturnSpawnPointData
{
    public string sceneName;
    public Vector2 returnPosition;
}



[System.Serializable]
public class CharacterSaveData
{
    public string id;
    public bool isUnlocked;
    public List<string> visitedScenes = new List<string>();
    public Vector2 savedPosition;
    public bool isActive;
    public bool reachedExit;
    public List<ReturnSpawnPointData> returnSpawnPoints = new List<ReturnSpawnPointData>();
}

public class SaveManager : MonoBehaviour
{

    private Dictionary<string, int> sceneOrder = new Dictionary<string, int>()
{
    {"Level_1", 1},
    {"Level_2", 2},
    {"Level_3", 3},
    {"Level_4", 4}
};
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

        if (saveData.characterStates == null)       //nullchecks for characterStates, for safety
            saveData.characterStates = new List<CharacterSaveData>();

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

    public List<CharacterSaveData> GetCharactersThatReachedExit()
    {
        if (!File.Exists(savePath)) return new List<CharacterSaveData>();
        string json = File.ReadAllText(savePath);
        GameSaveData data = JsonUtility.FromJson<GameSaveData>(json);
        return data.characterStates.FindAll(c => c.reachedExit);
    }

    public CharacterSaveData GetCharacterSaveDataById(string characterId)
    {
        if (!File.Exists(savePath)) return null;

        string json = File.ReadAllText(savePath);
        GameSaveData data = JsonUtility.FromJson<GameSaveData>(json);

        return data.characterStates.Find(c => c.id == characterId);
    }

    public bool HasCharacterVisitedScene(string characterId, string sceneName)
    {
        var characterData = GetCharacterSaveDataById(characterId);
        return characterData != null && characterData.visitedScenes.Contains(sceneName);
    }

    public void MarkSceneVisited(string characterId, string sceneName)
    {
        if (!File.Exists(savePath)) return;

        string json = File.ReadAllText(savePath);
        GameSaveData data = JsonUtility.FromJson<GameSaveData>(json);

        var character = data.characterStates.Find(c => c.id == characterId);
        if (character != null && !character.visitedScenes.Contains(sceneName))
        {
            character.visitedScenes.Add(sceneName);
            File.WriteAllText(savePath, JsonUtility.ToJson(data));
            Debug.Log($"Character {characterId} visited scene {sceneName}.");
        }
    }


    // When a character reaches the exit of a level, record their current position as the return spawn point for that scene
    public void MarkCharacterReachedExit(string characterId, string sceneName, Vector2? returnPosition = null)
    {
        Debug.Log($"[SaveManager] MarkCharacterReachedExit called for {characterId} in {sceneName} (returnPosition={returnPosition})");
        if (!File.Exists(savePath)) return;

        string json = File.ReadAllText(savePath);
        GameSaveData data = JsonUtility.FromJson<GameSaveData>(json);

        var character = data.characterStates.Find(c => c.id == characterId);
        if (character != null)
        {
            character.reachedExit = true;

            if (returnPosition.HasValue)
            {
                var rsp = character.returnSpawnPoints.Find(r => r.sceneName == sceneName);
                if (rsp != null)
                    rsp.returnPosition = returnPosition.Value;
                else
                    character.returnSpawnPoints.Add(new ReturnSpawnPointData { sceneName = sceneName, returnPosition = returnPosition.Value });
            }

            File.WriteAllText(savePath, JsonUtility.ToJson(data));
            Debug.Log($"Marked {characterId} as having reached the exit in {sceneName}.");
        }
    }


    // a method to get the stored return spawn point per scene for a character:
    public Vector2? GetReturnSpawnPoint(string characterId, string sceneName)
    {
        if (!File.Exists(savePath)) return null;

        string json = File.ReadAllText(savePath);
        GameSaveData data = JsonUtility.FromJson<GameSaveData>(json);

        var character = data.characterStates.Find(c => c.id == characterId);
        if (character == null) return null;

        var rsp = character.returnSpawnPoints.Find(r => r.sceneName == sceneName);
        if (rsp != null)
            return rsp.returnPosition;

        return null;
    }

    public void ResetReachedExitForSceneForAllCharacters(string sceneName)
    {
        if (!File.Exists(savePath)) return;

        string json = File.ReadAllText(savePath);
        GameSaveData data = JsonUtility.FromJson<GameSaveData>(json);

        bool changed = false;

        foreach (var character in data.characterStates)
        {
            if (character.reachedExit && character.visitedScenes.Contains(sceneName))
            {
                character.reachedExit = false;
                // Optionally clear the return spawn point for that scene
                character.returnSpawnPoints.RemoveAll(rsp => rsp.sceneName == sceneName);
                changed = true;
            }
        }

        if (changed)
        {
            File.WriteAllText(savePath, JsonUtility.ToJson(data));
            Debug.Log($"Reset reachedExit for all characters for scene {sceneName}.");
        }
    }

    public int GetSceneOrderIndex(string sceneName)
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogWarning("[SaveManager] GetSceneOrderIndex called with null or empty sceneName.");
            return -1;
        }
        if (sceneOrder.TryGetValue(sceneName, out int index))
            return index;
        return -1; // Unknown scene order
    }



}
