using UnityEngine;
using System.Linq;



public class ReachedExitInteractable : MonoBehaviour, IInteractable
{
    public void Interact()
    {
        var activeCharacter = CharacterManager.Instance.activeCharacter;
        if (activeCharacter == null) return;

        var character = CharacterManager.Instance.Characters
            .FirstOrDefault(c => c.instance == activeCharacter);

        if (character == null)
        {
            Debug.LogWarning("No matching character found in CharacterManager during interaction.");
            return;
        }

        string characterId = character.id;
        string currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        Vector2 returnPos = SpawnManager.Instance.GetReturnSpawnPoint() ?? character.instance.transform.position;

        SaveManager.Instance.MarkCharacterReachedExit(characterId, currentScene, returnPos);

        Debug.Log($"[ReachedExitTrigger] Character {characterId} marked as exited from {currentScene} at position {returnPos}.");
    }
}   

