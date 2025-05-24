using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Linq;

public class GameBootstrapper : MonoBehaviour
{
    [SerializeField] private string fallbackScene = "Level_1"; // Set to your starting scene

    private string previousScene;


    private void Awake()
    {
        Debug.Log("[GameBootstrapper] Bootstrapping game...");

        // Prevent duplicate persistent objects (optional, defensive)
        if (Object.FindObjectsByType<GameBootstrapper>(FindObjectsSortMode.None).Length > 1)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }
    private void OnEnable()
    {
        SceneManager.activeSceneChanged += OnActiveSceneChanged;
    }

    private void OnDisable()
    {
        SceneManager.activeSceneChanged -= OnActiveSceneChanged;
    }


    private void Start()
    {
        Debug.Log("[GameBootstrapper] Start called");

        // Load saved game if it exists
        if (SaveManager.Instance.HasSave())
        {
            Debug.Log("[GameBootstrapper] Save found, loading game...");
            SaveManager.Instance.LoadGame();

            // Delay scene load so managers have time to initialize
            string sceneToLoad = SaveManager.Instance.GetLastSceneName();
            StartCoroutine(LoadSceneDelayed(sceneToLoad));
        }
        else
        {
            if (SceneManager.GetActiveScene().name == fallbackScene)
            {
                Debug.Log("[GameBootstrapper] No save found, already in fallback scene, spawning character...");
                // Directly spawn the character here, no need to reload the scene
                SpawnManager.Instance.RefreshSpawnPoints();
                var startPos = SpawnManager.Instance.GetStartSpawnPoint() ?? Vector2.zero;
                var instance = SpawnManager.Instance.SpawnCharacterById("StartingCharacter", startPos);
                var data = CharacterManager.Instance.GetCharacterById("StartingCharacter");
                if (data != null)
                {
                    data.instance = instance;
                    data.lastPosition = startPos;
                    CharacterManager.Instance.SetActiveCharacter(CharacterManager.Instance.Characters.ToList().IndexOf(data));
                }
                previousScene = fallbackScene;
            }
            else
            {
                Debug.Log("[GameBootstrapper] No save found, loading fallback scene...");
                StartCoroutine(LoadFallbackSceneAndSpawn());
            }
        }

    }

    private System.Collections.IEnumerator LoadSceneDelayed(string sceneName)
    {
        yield return null; // Delay by one frame to ensure Awake() on managers runs first
       Debug.Log($"[GameBootstrapper] LoadSceneDelayed Loading scene '{sceneName}'");
        SceneManager.LoadScene(sceneName);

        yield return null; // Wait an additional frame so new scene loads before we initialize characters

        // Set the newly loaded scene as active
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneName));

        InitializeCharactersInScene();
        previousScene = sceneName;
    }

    private void OnActiveSceneChanged(Scene oldScene, Scene newScene)
    {
        string oldName = !string.IsNullOrEmpty(oldScene.name) ? oldScene.name : previousScene;
        Debug.Log($"[GameBootstrapper] OnActiveSceneChanged: oldScene='{oldName}', newScene='{newScene.name}'");

        if (string.IsNullOrEmpty(oldName)) //|| oldName == newScene.name)
        {
            previousScene = newScene.name;
            Debug.Log($"[GameBootstrapper] Skipping scene init: oldScene.name='{oldName}', newScene.name='{newScene.name}'");
            return;
        }

        Debug.Log($"[GameBootstrapper] Scene changed from '{oldName}' to '{newScene.name}'");
        if (SaveManager.Instance == null)
            return;

        InitializeCharactersInScene();
        CharacterManager.Instance?.SetActiveCharacter(0); // Reset active character to first one
        

        int oldIndex = SaveManager.Instance.GetSceneOrderIndex(oldName);
        int newIndex = SaveManager.Instance.GetSceneOrderIndex(newScene.name);

        Debug.Log($"[GameBootstrapper] Scene changed from '{oldName}' to '{newScene.name}' (old index: {oldIndex}, new index: {newIndex})");

        if (oldIndex > newIndex)
        {
            SaveManager.Instance.ResetReachedExitForSceneForAllCharacters(oldName);
        }

        previousScene = newScene.name;
        SaveManager.Instance.SaveGame();

        // CHECK VALUE OF reachedEXIT FOR CURRENT SCENE-------
        var currentScene = newScene.name;
        foreach (var charData in CharacterManager.Instance.Characters.Where(c => c.isUnlocked))
        {
            var saveData = SaveManager.Instance.GetCharacterSaveDataById(charData.id);
            if (saveData != null)
            {
                bool hasReturnPoint = saveData.returnSpawnPoints.Any(rsp => rsp.sceneName == currentScene);
                Debug.Log($"[SaveManager] Character '{charData.id}' reachedExit: {saveData.reachedExit}, hasReturnPoint for '{currentScene}': {hasReturnPoint}");
            }
            else
            {
                Debug.LogWarning($"[SaveManager] No save data found for character '{charData.id}'.");
            }
        }
    }

    private void InitializeCharactersInScene()
    {
        //CharacterManager.Instance?.InitializeUnlockedCharacters();  //TESTING PURPOSES ONLY
        Debug.Log("[GameBootstrapper] InitializeCharactersInScene called");

        if (SpawnManager.Instance == null)
        {
            Debug.LogWarning("[GameBootstrapper] SpawnManager.Instance is null!");
        }
        else
        {
            SpawnManager.Instance.RefreshSpawnPoints();
        }
        SpawnManager.Instance.RefreshSpawnPoints();

        //Checks how many characters are unlocked
        var unlocked = CharacterManager.Instance.Characters.Where(c => c.isUnlocked).ToList();
        Debug.Log($"[GameBootstrapper] Unlocked characters count: {unlocked.Count}");

        //foreach (var charData in SaveManager.Instance.GetCharactersThatReachedExit())
        foreach (var charData in CharacterManager.Instance.Characters.Where(c => c.isUnlocked))
        {

            var saveData = SaveManager.Instance.GetCharacterSaveDataById(charData.id);
            Vector2? returnPoint = null;

            if (saveData != null && saveData.reachedExit)
            {
                // Only use the return point if reachedExit is true
                returnPoint = SaveManager.Instance.GetReturnSpawnPoint(charData.id, SceneManager.GetActiveScene().name);
            }

            Debug.Log($"[GameBootstrapper] For character '{charData.id}' in scene '{SceneManager.GetActiveScene().name}', reachedExit={saveData?.reachedExit}, returnPoint={returnPoint}");

            Vector2 spawnPos = returnPoint
                ?? SpawnManager.Instance.GetReturnSpawnPoint()
                ?? SpawnManager.Instance.GetStartSpawnPoint()
                ?? Vector2.zero;

            var instance = SpawnManager.Instance.SpawnCharacterOnSceneLoad(charData.id, spawnPos);

            var cmChar = CharacterManager.Instance.GetCharacterById(charData.id);
            if (cmChar != null)
            {
                cmChar.instance = instance;
                cmChar.lastPosition = spawnPos;
            }
            // Log the return point for this character in the current scene
            //var returnPoint = SaveManager.Instance.GetReturnSpawnPoint(charData.id, SceneManager.GetActiveScene().name);
            //Debug.Log($"[GameBootstrapper] For character '{charData.id}' in scene '{SceneManager.GetActiveScene().name}', returnPoint={returnPoint}");


            // ---log reachedExit state from SaveManager ---
            //var saveData = SaveManager.Instance.GetCharacterSaveDataById(charData.id);
            //if (saveData != null)
            //{
            //    Debug.Log($"[SaveManager] Character '{charData.id}' reachedExit in this save: {saveData.reachedExit}");
            //}
            //else
            //{
            //    Debug.LogWarning($"[SaveManager] No save data found for character '{charData.id}'.");
            //}
            //// ---------------------------------------------------------------
            //if (charData.isUnlocked)
            //{
            //    Vector2 spawnPos = SaveManager.Instance.GetReturnSpawnPoint(charData.id, UnityEngine.SceneManagement.SceneManager.GetActiveScene().name)
            //                   ?? SpawnManager.Instance.GetReturnSpawnPoint()
            //                   ?? SpawnManager.Instance.GetStartSpawnPoint()
            //                   ?? Vector2.zero;

            //    //Vector2 spawnPos = SaveManager.Instance.GetReturnSpawnPoint(charData.id, UnityEngine.SceneManagement.SceneManager.GetActiveScene().name)
            //    //   ?? SpawnManager.Instance.GetStartSpawnPoint()
            //    //   ?? Vector2.zero;


            //    var instance = SpawnManager.Instance.SpawnCharacterOnSceneLoad(charData.id, spawnPos);

            //    var cmChar = CharacterManager.Instance.GetCharacterById(charData.id);
            //    if (cmChar != null)
            //    {
            //        cmChar.instance = instance;
            //        cmChar.lastPosition = spawnPos;
            //    }
        }
        
    }

    private IEnumerator LoadFallbackSceneAndSpawn()
    {
        if (SceneManager.GetActiveScene().name != fallbackScene)
        {
            SceneManager.LoadScene(fallbackScene);
            yield return null; // Wait for the scene to load
                               // Set the newly loaded scene as active
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(fallbackScene)); ;

            // OnActiveSceneChanged will handle spawning after the scene loads
            yield break;
        }

        SceneManager.LoadScene(fallbackScene);
        yield return null; // Wait for the scene to load

        // Ensure spawn points are refreshed from the scene
        SpawnManager.Instance.RefreshSpawnPoints();

        // Spawn the main character at the start spawn point
        var startPos = SpawnManager.Instance.GetStartSpawnPoint() ?? Vector2.zero;
        var instance = SpawnManager.Instance.SpawnCharacterById("StartingCharacter", startPos);
        var data = CharacterManager.Instance.GetCharacterById("StartingCharacter");
        if (data != null)
        {
            data.instance = instance;
            data.lastPosition = startPos;
            CharacterManager.Instance.SetActiveCharacter(CharacterManager.Instance.Characters.ToList().IndexOf(data));
        }
        previousScene = fallbackScene;
    }

}

