using System.Linq;
using UnityEngine;

public class ReachedExitTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        // Find the character in CharacterManager by matching instance GameObject
        var character = CharacterManager.Instance.Characters.FirstOrDefault(c => c.instance == other.gameObject);


        if (character == null)
        {
            Debug.LogWarning("No matching character found in CharacterManager for the object entering exit trigger.");
            return;
        }

        string characterId = character.id;
        string currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        Vector2 returnPos = other.transform.position;

        SaveManager.Instance.MarkCharacterReachedExit(characterId, currentScene, returnPos);

        Debug.Log($"Character {characterId} reached exit at {currentScene} and return position saved.");
    }
}
