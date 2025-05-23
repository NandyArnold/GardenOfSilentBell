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
            SceneManager.LoadScene(nextIndex);
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            int prevIndex = SceneManager.GetActiveScene().buildIndex - 1;
            if (prevIndex < 0) prevIndex = SceneManager.sceneCountInBuildSettings - 1;
            SceneManager.LoadScene(prevIndex);
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            SaveManager.Instance.SaveGame();
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            SaveManager.Instance.LoadGame();
            string sceneToLoad = SaveManager.Instance.GetLastSceneName();
            if (!string.IsNullOrEmpty(sceneToLoad) && sceneToLoad != SceneManager.GetActiveScene().name)
            {
                SceneManager.LoadScene(sceneToLoad);
            }
        }
    }
}