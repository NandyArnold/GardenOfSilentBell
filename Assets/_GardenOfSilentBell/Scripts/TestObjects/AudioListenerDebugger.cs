using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
public class AudioListenerDebugger : MonoBehaviour
{

    private void Awake()
    {
        var listeners = Object.FindObjectsByType<AudioListener>(FindObjectsSortMode.None);
        if (listeners.Length > 1)
        {
            Debug.LogWarning("[AudioListenerDebugger] Multiple AudioListeners found:");
            foreach (var l in listeners)
                Debug.Log($"AudioListener on: {l.gameObject.name} (active: {l.enabled})");
        }
    }

    private void OnEnable()
    {
        
    }

    void Update()
    {
        // This is just to keep the script active, no functionality needed here
    }
}
