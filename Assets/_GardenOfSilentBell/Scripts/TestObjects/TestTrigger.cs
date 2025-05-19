using UnityEngine;

public class TestTrigger : MonoBehaviour
{
    public void Ping()
    {
        Debug.Log("Ping received." + Time.time);
    }
}