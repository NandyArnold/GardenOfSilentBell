#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;

[ExecuteInEditMode]
[DisallowMultipleComponent]
public class UniqueObjectIdAssigner : MonoBehaviour
{
    private void Awake()
    {
        AssignIfMissing();
    }

    private void AssignIfMissing()
    {
        var puzzle = GetComponent<PuzzleObject>();
        if (puzzle == null) return;

        if (string.IsNullOrEmpty(puzzle.objectId))
        {
            puzzle.objectId = Guid.NewGuid().ToString();
            EditorUtility.SetDirty(puzzle);
            Debug.Log($"Assigned new objectId: {puzzle.objectId} to {gameObject.name}");
        }
    }
}
#endif
