using UnityEngine;

[RequireComponent(typeof(UnlockCharacter))]
public class TriggerUnlockSwitch : MonoBehaviour
{
    [Tooltip("Should the trigger deactivate itself after activation?")]
    public bool oneTimeUse = true;

    private bool hasActivated = false;
    private UnlockCharacter unlocker;

    private void Awake()
    {
        unlocker = GetComponent<UnlockCharacter>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasActivated || !other.CompareTag("Player")) return;

        if (unlocker.TryUnlockCharacter())
        {
            hasActivated = true;
            if (oneTimeUse) gameObject.SetActive(false);
        }
    }
}
