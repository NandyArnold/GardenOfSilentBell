using UnityEngine;

public class PrisonDestructionZone : MonoBehaviour
{
    [SerializeField] private GameObject prisonRoot; // Assign in Inspector
    [SerializeField] private GameObject diamond;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("TelekinesisObject")) // or any check you use for the diamond
        {
            Debug.Log("Diamond touched trigger. Destroying prison.");
            Destroy(prisonRoot);
            Destroy(diamond);
        }
    }
}
