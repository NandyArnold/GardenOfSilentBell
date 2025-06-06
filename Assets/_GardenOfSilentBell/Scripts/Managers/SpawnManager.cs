using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.TextCore.Text;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager Instance { get; private set; }
    
    [System.Serializable]
    public class SpawnPoint
    {
        public string characterId;
        public Transform spawnTransform;
    }


    [SerializeField] private Transform startSpawnPoint;
    [SerializeField] private Transform returnSpawnPoint;

    [SerializeField] private List<SpawnPoint> spawnPoints= new List<SpawnPoint>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

    }

    public GameObject SpawnCharacterById(string id, Vector2 fallbackPosition)
    {
        var characterData = CharacterManager.Instance.GetCharacterById(id);
        if (characterData == null || characterData.characterPrefab == null)
        {
            Debug.LogWarning($"[SpawnManager] No prefab for character ID '{id}'.");
            return null;
        }

        Vector2 spawnPos = GetSpawnPointForCharacter(id) ?? fallbackPosition;
        GameObject instance = Instantiate(characterData.characterPrefab, spawnPos, Quaternion.identity);
        instance.name = id;
        return instance;
    }

    public GameObject SpawnCharacterOnSceneLoad(string id, Vector2 position)
    {
        Debug.Log($"[SPAWN DEBUG] SpawnCharacterOnSceneLoad called for {id} at {position}");

        var characterData = CharacterManager.Instance.GetCharacterById(id);
        if (characterData == null || characterData.characterPrefab == null)
        {
            Debug.LogWarning($"[SpawnManager] No prefab for character ID '{id}'.");
            return null;
        }

        // Always use the provided position, ignore per-character spawn points
        GameObject instance = Instantiate(characterData.characterPrefab, position, Quaternion.identity);
        instance.name = id;
        Debug.Log($"[SPAWN DEBUG] SpawnCharacterOnSceneLoad returning: {(instance ? instance.name : "NULL")}");
        return instance;
    }


  


public void RefreshSpawnPoints()
    {
         // Load saved data to get spawn points
                                          // Find by tag, name, or component type as you prefer
        spawnPoints.ForEach(sp =>
        {
            // Look for a GameObject in the scene with a matching name
            var found = GameObject.Find($"Spawn_{sp.characterId}");
            if (found != null)
            {
                sp.spawnTransform = found.transform;
            }
            else
            {
                //Debug.LogWarning($"[SpawnManager] No spawn transform found for characterId: {sp.characterId}");
            }
        });


        var start = GameObject.FindWithTag("StartSpawnPoint");
        var ret = GameObject.FindWithTag("ReturnSpawnPoint");

        // Update spawnPoints by finding all GameObjects with a SpawnPoint component
        //spawnPoints = FindObjectsOfType<SpawnPoint>().ToList();

        startSpawnPoint = start ? start.transform : null;
        returnSpawnPoint = ret ? ret.transform : null;
    }

    private Vector2? GetSpawnPointForCharacter(string id)
    {
        var point = spawnPoints.Find(p => p.characterId == id);
        if (point == null)
        {
            //Debug.LogWarning($"[SpawnManager] No SpawnPoint entry found for characterId: '{id}'.");
            return null;
        }

        if (point.spawnTransform == null)
        {
            //Debug.LogWarning($"[SpawnManager] SpawnPoint for characterId: '{id}' has null spawnTransform.");
            return null;
        }

        return point.spawnTransform.position;
    }

    public GameObject SpawnPrefab(GameObject prefab, Vector2 position)
    {
        return Instantiate(prefab, position, Quaternion.identity);
    }

    public Vector2? GetStartSpawnPoint()
    {
        return startSpawnPoint != null ? (Vector2?)startSpawnPoint.position : null;
    }

    public Vector2? GetReturnSpawnPoint()
    {
        return returnSpawnPoint != null ? (Vector2?)returnSpawnPoint.position : null;
    }



    public void SpawnAllSceneCharacters()
    {
        Debug.Log("[SPAWN DEBUG] SpawnAllSceneCharacters called - THIS SHOULD NOT HAPPEN!");
        foreach (var character in CharacterManager.Instance.Characters)
        {
            if (character.isUnlocked || character.id == "StartingCharacter")
                continue;

            var spawnPoint = GetSpawnPointForCharacter(character.id);
            if (spawnPoint.HasValue)
            {
                var instance = SpawnCharacterOnSceneLoad(character.id, spawnPoint.Value);
                character.instance = instance;
                character.lastPosition = spawnPoint.Value;
            }
            else
            {
                //Debug.LogWarning($"[SpawnManager] No spawn point found for character '{character.id}' in this scene.");
            }
        }
    }


}
