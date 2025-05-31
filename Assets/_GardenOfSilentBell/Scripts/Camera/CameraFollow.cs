using UnityEngine;

using Unity.Cinemachine;

public class CameraFollow : MonoBehaviour
{
    public static CameraFollow Instance { get; private set; }

    private CinemachineCamera vCam;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        vCam = GetComponent<CinemachineCamera>();
    }

    public void SetTarget(Transform target)
    {
        if (vCam != null)
        {
            vCam.Follow = target;
        }
    }
    public void JumpFollow(Transform target)
    {
        if (vCam != null)
        {
            vCam.Follow = target;
            
        }
    }
}
