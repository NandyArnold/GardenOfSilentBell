using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    public string characterId;
    public Transform spawnTransform;

    private void Awake()
    {
        if (spawnTransform == null)
            spawnTransform = transform;
    }
}

