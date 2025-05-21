using UnityEngine;

public class PuzzleManager : MonoBehaviour
{
    public void CompleteSecondPuzzle()
    {
        // Called when puzzle is solved
        CharacterManager.Instance.SetMetUp();
    }
}
