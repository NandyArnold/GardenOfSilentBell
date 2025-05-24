using UnityEngine;
using UnityEngine.SceneManagement;

public class GameBootstrapper : MonoBehaviour
{
    [SerializeField] private string fallbackScene = "IntroScene"; // Set to your starting scene

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

        CharacterManager.Instance?.InitializeUnlockedCharacters();

        foreach (var charData in SaveManager.Instance.GetCharactersThatReachedExit())
        {
            if (charData.isUnlocked)
            {
                Vector2 spawnPos = SpawnManager.Instance.GetStartSpawnPoint() ?? Vector2.zero;
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
