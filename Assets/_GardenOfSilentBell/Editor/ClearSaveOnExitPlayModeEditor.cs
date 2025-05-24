#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using System.IO;

[InitializeOnLoad]
public static class ClearSaveOnExitPlayMode
{
    static ClearSaveOnExitPlayMode()
    {
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }

    private static void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.ExitingPlayMode)
        {
            string savePath = Path.Combine(Application.persistentDataPath, "save.json");
            Debug.Log($"[ClearSaveOnExitPlayMode] Checking for save at: {savePath}");
            if (File.Exists(savePath))
            {
                try
                {
                    File.Delete(savePath);
                    Debug.Log("[ClearSaveOnExitPlayMode] Save file deleted on exiting play mode.");
                }
                catch (IOException ex)
                {
                    Debug.LogError($"[ClearSaveOnExitPlayMode] Failed to delete save file: {ex.Message}");
                }
            }
            else
            {
                Debug.Log("[ClearSaveOnExitPlayMode] No save file found to delete.");
            }
        }
    }
}
#endif
