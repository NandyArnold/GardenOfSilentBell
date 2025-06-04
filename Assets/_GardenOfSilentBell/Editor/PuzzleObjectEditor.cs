#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PuzzleObject))]
public class PuzzleObjectEditor : Editor
{
    public override void OnInspectorGUI()
    {
        PuzzleObject puzzle = (PuzzleObject)target;

        // Auto-add UniqueObjectIdAssigner if not already present
        if (puzzle.GetComponent<UniqueObjectIdAssigner>() == null)
        {
            Undo.AddComponent<UniqueObjectIdAssigner>(puzzle.gameObject);
            Debug.Log($"[PuzzleObjectEditor] Auto-added UniqueObjectIdAssigner to '{puzzle.gameObject.name}'");
        }

        // Draw default inspector
        DrawDefaultInspector();
    }
}
#endif
