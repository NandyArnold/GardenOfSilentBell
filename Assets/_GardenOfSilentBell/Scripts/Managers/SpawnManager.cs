using System.Collections.Generic;
using UnityEngine;

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

    [SerializeField] private List<SpawnPoint> spawnPoints;

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

    private Vector2? GetSpawnPointForCharacter(string id)
    {
        var point = spawnPoints.Find(p => p.characterId == id);
        return point?.spawnTransform?.position;
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

}
