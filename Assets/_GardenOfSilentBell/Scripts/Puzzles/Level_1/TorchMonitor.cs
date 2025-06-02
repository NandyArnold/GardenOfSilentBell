using UnityEngine;

public class TorchMonitor : MonoBehaviour
{
    public GameObject torch1;
    public GameObject torch2;
    public GameObject objectToActivate;

    void Update()
    {
        if (!torch1.activeSelf && !torch2.activeSelf && !objectToActivate.activeSelf)
        {
            objectToActivate.SetActive(true);
            Debug.Log("Both torches are out. Activated target object.");
        }
    }
}
