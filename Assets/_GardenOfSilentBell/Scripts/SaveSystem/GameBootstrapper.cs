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
                Debug.Log("[GameBootstrapper]-START Refreshing spawn points and spawning character in fallback scene...");
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

        //InitializeCharactersInScene();
        previousScene = sceneName;
    }

    private void OnActiveSceneChanged(Scene oldScene, Scene newScene)
    {
        if (newScene.buildIndex == 0)
            return;

        if (SaveManager.Instance == null)
            return;

        InitializeCharactersInScene();
        Debug.Log($"[GameBootstrapper] Initialized Characters, setting active'{oldScene.name}' to '{newScene.name}'");
        //  CharacterManager.Instance?.SetActiveCharacter(0); // Reset active character to first one
        if (!CharacterManager.Instance.Characters.Any(c => c.isActive))
        {
            CharacterManager.Instance.SetActiveCharacter(0);
        }

        int oldIndex = oldScene.buildIndex;
        int newIndex = newScene.buildIndex;

       
        if (oldIndex > newIndex)
        {
            SaveManager.Instance.ResetReachedExitForSceneForAllCharacters(oldScene.name);
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

        if (CharacterHUDManager.Instance != null)
        {
            Debug.Log("[GameBootstrapper] Refreshing CharacterHUDManager UI after scene change.");
            CharacterHUDManager.Instance.UpdateCharacterBar();
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
            Debug.Log("[GameBootstrapper]-InitializeCHaracterInScene() Refreshing spawn points ");
            SpawnManager.Instance.RefreshSpawnPoints();
            SpawnManager.Instance.SpawnAllSceneCharacters();
            //FollowManager.Instance.AssignFollowTargets();
        }
       

        //Checks how many characters are unlocked
        var unlocked = CharacterManager.Instance.Characters.Where(c => c.isUnlocked).ToList();
        Debug.Log($"[GameBootstrapper] Unlocked characters count: {unlocked.Count}");


        foreach (var charData in unlocked)
        {
            SaveManager.Instance.MarkSceneVisited(charData.id, SceneManager.GetActiveScene().name);
            StartCoroutine(ChooseAndSpawnCharacterCoroutine(charData.id));
        }
        var activeChar = unlocked.FirstOrDefault(c => c.isActive);
        if (activeChar != null)
        {
            int idx = CharacterManager.Instance.Characters.ToList().IndexOf(activeChar);
            if (idx >= 0)
                CharacterManager.Instance.SetActiveCharacter(idx);
        }
        else if (unlocked.Count > 0)
        {
            // Fallback: set the first unlocked character as active
            int idx = CharacterManager.Instance.Characters.ToList().IndexOf(unlocked[0]);
            if (idx >= 0)
                CharacterManager.Instance.SetActiveCharacter(idx);
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
        Debug.Log("[GameBootstrapper]-- COROUTINE LoadFallbackSceneAndSpawn : Refreshing spawn points in fallback scene...");
        SpawnManager.Instance.RefreshSpawnPoints();

        // Spawn the main character at the start spawn point
        var startPos = SpawnManager.Instance.GetStartSpawnPoint() ?? Vector2.zero;
        var instance = SpawnManager.Instance.SpawnCharacterById("StartingCharacter", startPos);
        var data = CharacterManager.Instance.GetCharacterById("StartingCharacter");
        Debug.Log($"[GameBootstrapper]-- COROUTINE LoadFallbackSceneAndSpawn : Spawning 'StartingCharacter' at {startPos}");
        if (data != null)
        {
            data.instance = instance;
            data.lastPosition = startPos;
            Debug.Log($"[GameBootstrapper]-- COROUTINE LoadFallbackSceneAndSpawn : Setting 'StartingCharacter' as active character");
            CharacterManager.Instance.SetActiveCharacter(CharacterManager.Instance.Characters.ToList().IndexOf(data));
        }
        previousScene = fallbackScene;
    }

    private IEnumerator ChooseAndSpawnCharacterCoroutine(string characterId)
    {
        var saveData = SaveManager.Instance.GetCharacterSaveDataById(characterId);
        Vector2? returnPoint = null;
        bool reachedExit = saveData != null && saveData.reachedExit;

        if (reachedExit)
            returnPoint = SaveManager.Instance.GetReturnSpawnPoint(characterId, SceneManager.GetActiveScene().name);

        Vector2? sceneReturnPoint = SpawnManager.Instance.GetReturnSpawnPoint();
        Vector2? startPoint = SpawnManager.Instance.GetStartSpawnPoint();

        string spawnType;
        Vector2 spawnPos;
        if (returnPoint.HasValue)
        {
            spawnType = "PerCharacterReturn";
            spawnPos = returnPoint.Value;
        }
       
        else if (startPoint.HasValue)
        {
            spawnType = "Start";
            spawnPos = startPoint.Value;
        }
        else
        {
            spawnType = "DefaultZero";
            spawnPos = Vector2.zero;
        }

        Debug.Log($"[GameBootstrapper] Spawning '{characterId}' using {spawnType} at {spawnPos}");

        var instance = SpawnManager.Instance.SpawnCharacterOnSceneLoad(characterId, spawnPos);

        var cmChar = CharacterManager.Instance.GetCharacterById(characterId);
        if (cmChar != null)
        {
            cmChar.instance = instance;
            cmChar.lastPosition = spawnPos;

            
            if (saveData != null)
            {
                var companionFollow = instance.GetComponent<CompanionFollow>();
                if (companionFollow != null)
                {
                    companionFollow.SetHasMetUp(saveData.hasMetUp);
                }
            }
        }

        yield return null;
    }


}

