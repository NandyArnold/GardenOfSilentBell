using UnityEngine;
using System.Collections.Generic;
using System.IO;
using static CharacterManager;
using System.Linq;

[System.Serializable]
public class SceneObjectState
{
    public string objectId;
    public Vector3 position;
    public Quaternion rotation;
    public Vector3 scale;
    public bool isActive;
    public bool isDestroyed;
}

[System.Serializable]
public class SceneSaveData
{
    public string sceneName;
    public List<SceneObjectState> objects = new List<SceneObjectState>();
}

[System.Serializable]
public class GameSaveData
{
    public string currentScene;
    public List<CharacterSaveData> characterStates;
    public List<SceneSaveData> sceneStates = new List<SceneSaveData>();
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
    public bool hasMetUp;
    public List<ReturnSpawnPointData> returnSpawnPoints = new List<ReturnSpawnPointData>();
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
       
    }

    public void SaveGame()
    {
        GameSaveData saveData;
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            saveData = JsonUtility.FromJson<GameSaveData>(json);
            if (saveData.characterStates == null)
                saveData.characterStates = new List<CharacterSaveData>();
        }
        else
        {
            saveData = new GameSaveData();
            saveData.characterStates = new List<CharacterSaveData>();
        }

        saveData.currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

        foreach (var character in CharacterManager.Instance.Characters)
        {
            var existing = saveData.characterStates.Find(c => c.id == character.id);
            if (existing == null)
            {
                existing = new CharacterSaveData { id = character.id };
                saveData.characterStates.Add(existing);
            }
            existing.isUnlocked = character.isUnlocked;
            existing.savedPosition = character.instance != null
                ? (Vector2)character.instance.transform.position
                : character.lastPosition;
            existing.isActive = character.instance == CharacterManager.Instance.activeCharacter;
            var companionFollow = character.instance != null ? character.instance.GetComponent<CompanionFollow>() : null;
            if (companionFollow != null)
            {
                //Debug.Log($"[SaveManager] CompanionFollow for '{character.id}' found. hasMetUp={companionFollow.hasMetUp}");
            }
            else
            {
                //Debug.Log($"[SaveManager] CompanionFollow for '{character.id}' is NULL.");
            }
            existing.hasMetUp = companionFollow != null ? companionFollow.hasMetUp : character.hasMetUp;

            // Do NOT overwrite reachedExit or returnS
            // Do NOT overwrite reachedExit or returnSpawnPoints here!
        }

        var sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        var objects = Object.FindObjectsByType<PuzzleObject>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        var sceneData = new SceneSaveData { sceneName = sceneName };

        foreach (var obj in objects)
        {
             Debug.Log($"[SaveManager] Found PuzzleObject: {obj.name}, ID: {obj.objectId}, Active: {obj.gameObject.activeSelf}, Pos: {obj.transform.position}");
            sceneData.objects.Add(obj.CaptureState());
        }

        // Replace existing scene save or add new
        var existingSceneData = saveData.sceneStates.Find(s => s.sceneName == sceneName);
        if (existingSceneData != null)
            saveData.sceneStates.Remove(existingSceneData);

        saveData.sceneStates.Add(sceneData);

        File.WriteAllText(savePath, JsonUtility.ToJson(saveData));
       
        //Debug.Log("[SaveManager]Game saved. Save data after SaveGame:\n" + JsonUtility.ToJson(saveData, true));
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
        Debug.Log("[SaveManager] Loading character state: " + savePath);
        CharacterManager.Instance.LoadCharacterStates(saveData.characterStates);

        var sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        var sceneData = saveData.sceneStates.Find(s => s.sceneName == sceneName);
        if (sceneData != null)
        {
            var allPuzzleObjects = Object.FindObjectsByType<PuzzleObject>(FindObjectsSortMode.None);
            foreach (var state in sceneData.objects)
            {
                var match = allPuzzleObjects.FirstOrDefault(o => o.objectId == state.objectId);
                if (match != null)
                    match.ApplyState(state);
            }
        }
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

        // Update reachedExit and return position for all unlocked characters
        foreach (var character in data.characterStates.Where(c => c.isUnlocked))
        {
            character.reachedExit = true;

            if (returnPosition.HasValue)
            {
                var rsp = character.returnSpawnPoints.Find(r => r.sceneName == sceneName);
                if (rsp != null)
                {
                    rsp.returnPosition = returnPosition.Value;
                }
                else
                {
                    character.returnSpawnPoints.Add(new ReturnSpawnPointData
                    {
                        sceneName = sceneName,
                        returnPosition = returnPosition.Value
                    });
                }
            }

            Debug.Log($"Marked {character.id} as having reached the exit in {sceneName}.");
        }

        File.WriteAllText(savePath, JsonUtility.ToJson(data));
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



}
