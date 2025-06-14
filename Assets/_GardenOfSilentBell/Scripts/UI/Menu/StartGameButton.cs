using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGameButton : MonoBehaviour
{
    public GameObject canvasToDisable; // Assign the button itself or its parent

    public void OnStartGamePressed()
    {
        //Debug.Log("[StartGameButton] Play pressed, loading Level_1...");
        if (canvasToDisable != null)
        {
            canvasToDisable.SetActive(false);
        }

        SceneManager.LoadScene("Level_1");
    }
}

