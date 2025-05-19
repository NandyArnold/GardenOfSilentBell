using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class Pushable : MonoBehaviour, IInteractable
{
    public bool IsBeingMoved { get; private set; }
    private Transform player;
    private FixedJoint2D joint;

    private void Awake()
    {
        joint = gameObject.AddComponent<FixedJoint2D>();
        joint.enabled = false;
        joint.autoConfigureConnectedAnchor = false;
    }

    public void Interact()
    {
        if (!IsBeingMoved)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
                joint.connectedBody = player.GetComponent<Rigidbody2D>();
                joint.enabled = true;
                IsBeingMoved = true;
                Debug.Log("Started pushing/pulling.");
            }
        }
        else
        {
            Detach();
        }
    }

    public void Detach()
    {
        joint.connectedBody = null;
        joint.enabled = false;
        IsBeingMoved = false;
        player = null;
        Debug.Log("Stopped pushing/pulling.");
    }
}
