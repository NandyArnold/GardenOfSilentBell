//// Place in Assets/Editor or any folder named "Editor"
//using UnityEngine;
//using UnityEditor;

//[CustomEditor(typeof(TriggerUnlockSwitch))]
//public class TriggerUnlockSwitchEditor : Editor
//{
//    public override void OnInspectorGUI()
//    {
//        TriggerUnlockSwitch switchScript = (TriggerUnlockSwitch)target;

//        // Get the CharacterManager (or provide a fallback)
//        var characterManager = FindObjectOfType<CharacterManager>();
//        if (characterManager == null)
//        {
//            EditorGUILayout.HelpBox("CharacterManager not found in scene!", MessageType.Warning);
//            DrawDefaultInspector();
//            return;
//        }

//        string[] characterNames = characterManager.GetAllCharacterNames();

//        int currentIndex = Mathf.Max(0, System.Array.IndexOf(characterNames, switchScript.characterToUnlock));
//        int selectedIndex = EditorGUILayout.Popup("Character to Unlock", currentIndex, characterNames);

//        switchScript.characterToUnlock = characterNames[selectedIndex];

//        // Draw the rest of the default inspector
//        DrawDefaultInspectorExcept("characterToUnlock");

//        // Mark scene dirty if changed
//        if (GUI.changed)
//            EditorUtility.SetDirty(target);
//    }

//    void DrawDefaultInspectorExcept(params string[] ignoreFields)
//    {
//        SerializedProperty prop = serializedObject.GetIterator();
//        prop.NextVisible(true); // Skip script reference

//        while (prop.NextVisible(false))
//        {
//            if (System.Array.Exists(ignoreFields, field => field == prop.name))
//                continue;

//            EditorGUILayout.PropertyField(prop, true);
//        }

//        serializedObject.ApplyModifiedProperties();
//    }
//}
