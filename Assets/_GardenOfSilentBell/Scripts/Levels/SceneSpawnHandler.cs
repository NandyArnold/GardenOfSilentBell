using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TextCore.Text;

public class SceneSpawnHandler : MonoBehaviour
{
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Get active character ID from CharacterManager
        string characterId = CharacterManager.Instance.GetActiveCharacterId();

        // Try to get saved return spawn point for this character and scene
        Vector2? spawnPos = SaveManager.Instance.GetReturnSpawnPoint(characterId, scene.name);

        if (spawnPos.HasValue)
        {
            Debug.Log($"[SceneSpawnHandler] Spawning character '{characterId}' at saved return position {spawnPos.Value} in scene '{scene.name}'.");
            CharacterManager.Instance.activeCharacter.transform.position = spawnPos.Value;
        }
        else
        {
            Debug.LogWarning($"[SceneSpawnHandler] No saved spawn point found for character '{characterId}' in scene '{scene.name}'. Attempting to use default spawn point.");
            // Fallback: use SpawnManager's default start spawn point if available
            Vector2? defaultSpawn = SpawnManager.Instance.GetStartSpawnPoint();

            if (defaultSpawn.HasValue)
            {
                Debug.Log($"[SceneSpawnHandler] No saved spawn point found for '{characterId}'. Using default start spawn point {defaultSpawn.Value}.");
                CharacterManager.Instance.activeCharacter.transform.position = defaultSpawn.Value;
            }
            else
            {
                Debug.LogWarning($"[SceneSpawnHandler] No spawn point found for character '{characterId}' in scene '{scene.name}'.");
            }
        }
    }
}
