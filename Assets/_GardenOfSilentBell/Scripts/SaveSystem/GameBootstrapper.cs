using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameBootstrapper : MonoBehaviour
{
    [SerializeField] private string fallbackScene = "IntroScene"; // Set to your starting scene

    private string previousScene;

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
        Debug.Log("[GameBootstrapper] Bootstrapping game...");

        // Prevent duplicate persistent objects (optional, defensive)
        if (Object.FindObjectsByType<GameBootstrapper>(FindObjectsSortMode.None).Length > 1)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

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
            Debug.Log("[GameBootstrapper] No save found, loading fallback scene...");
            SceneManager.LoadScene(fallbackScene);
        }

    }

    private System.Collections.IEnumerator LoadSceneDelayed(string sceneName)
    {
        yield return null; // Delay by one frame to ensure Awake() on managers runs first
       
        SceneManager.LoadScene(sceneName);

        yield return null; // Wait an additional frame so new scene loads before we initialize characters

        InitializeCharactersInScene();
        previousScene = sceneName;
    }

    private void OnActiveSceneChanged(Scene oldScene, Scene newScene)
    {
        if (previousScene == null)
        {
            previousScene = oldScene.name;
            return;
        }

        if (SaveManager.Instance == null)
            return;

        // Determine scene order indices
        int oldIndex = SaveManager.Instance.GetSceneOrderIndex(oldScene.name);
        int newIndex = SaveManager.Instance.GetSceneOrderIndex(newScene.name);

        if (oldIndex > newIndex)
        {
            // Moving backward in scene order -> reset reachedExit for the old scene
            SaveManager.Instance.ResetReachedExitForSceneForAllCharacters(oldScene.name);
        }

        InitializeCharactersInScene();

        previousScene = newScene.name;
    }

    private void InitializeCharactersInScene()
    {
        CharacterManager.Instance?.InitializeUnlockedCharacters();

        foreach (var charData in SaveManager.Instance.GetCharactersThatReachedExit())
        {
            if (charData.isUnlocked)
            {
                Vector2 spawnPos = SaveManager.Instance.GetReturnSpawnPoint(charData.id, UnityEngine.SceneManagement.SceneManager.GetActiveScene().name)
                               ?? SpawnManager.Instance.GetReturnSpawnPoint()
                               ?? SpawnManager.Instance.GetStartSpawnPoint()
                               ?? Vector2.zero;

                var instance = SpawnManager.Instance.SpawnCharacterById(charData.id, spawnPos);

                var cmChar = CharacterManager.Instance.GetCharacterById(charData.id);
                if (cmChar != null)
                {
                    cmChar.instance = instance;
                    cmChar.lastPosition = spawnPos;
                }
            }
        }
    }
}

