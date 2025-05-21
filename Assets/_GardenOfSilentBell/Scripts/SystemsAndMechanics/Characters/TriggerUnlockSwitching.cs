using UnityEngine;

public class TriggerUnlockSwitching : MonoBehaviour
{
    private bool triggered = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (triggered) return;

        if (other.CompareTag("Player"))
        {
            triggered = true;
            CharacterManager.Instance.EnableSwitching();

            // Optional: trigger cutscene/camera pan/etc here
        }
    }
}
