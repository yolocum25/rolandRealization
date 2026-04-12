using UnityEngine;
using UnityEngine.SceneManagement;

public class VictoryActions : MonoBehaviour
{
    [Header("Configuración de Escenas")]
    [SerializeField] private string nextLevelName;
    [SerializeField] private string mainMenuName = "MainMenu";

    public void LoadNextLevel()
    {
       
        Time.timeScale = 1f;
        
        string currentLevelName = SceneManager.GetActiveScene().name;
        LevelCheckerManager.MarkLevelAsCompleted(currentLevelName);
        
        if (!string.IsNullOrEmpty(nextLevelName))
        {
            SceneManager.LoadScene(nextLevelName);
        }
        else
        {
            
            int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
            if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
            {
                SceneManager.LoadScene(nextSceneIndex);
            }
        }
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuName);
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}