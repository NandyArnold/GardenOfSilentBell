using UnityEngine;

public class SpawnTestController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Debug.Log($"[SpawnTestController] {gameObject.name} ({GetInstanceID()}) Spawning StartingCharacter at (2, 0)"); Debug.Log($"[SpawnTestController] {gameObject.name} ({GetInstanceID()}) Spawning StartingCharacter at (2, 0)");
            SpawnManager.Instance.SpawnCharacterById("StartingCharacter", new Vector2(2, 0));
        }
    }
}
