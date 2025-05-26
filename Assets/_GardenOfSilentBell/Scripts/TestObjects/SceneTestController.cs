using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTestController : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
        {
            int nextIndex = (SceneManager.GetActiveScene().buildIndex + 1) % SceneManager.sceneCountInBuildSettings;
            Debug.Log($"[SceneTestController]Game Saved Going to next scene index: {nextIndex}");
            SaveManager.Instance.SaveGame(); // Save before going to next scene
            SceneManager.LoadScene(nextIndex);
        }
        else if (Input.GetKeyDown(KeyCode.B))
        {
            int prevIndex = SceneManager.GetActiveScene().buildIndex - 1;
            if (prevIndex < 0) prevIndex = SceneManager.sceneCountInBuildSettings - 1;
            Debug.Log($"[SceneTestController]Game Saved Going back to scene index: {prevIndex}");
            SaveManager.Instance.SaveGame(); // Save before going back
            SceneManager.LoadScene(prevIndex);
        }
        else if (Input.GetKeyDown(KeyCode.K))
        {
            SaveManager.Instance.SaveGame();
        }
        //else if (Input.GetKeyDown(KeyCode.L))
        //{
        //    SaveManager.Instance.LoadGame();
        //    string sceneToLoad = SaveManager.Instance.GetLastSceneName();
        //    if (!string.IsNullOrEmpty(sceneToLoad) && sceneToLoad != SceneManager.GetActiveScene().name)
        //    {
        //        SceneManager.LoadScene(sceneToLoad);
        //    }
        //}
    }
}