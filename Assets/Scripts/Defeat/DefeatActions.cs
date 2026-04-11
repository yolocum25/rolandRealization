using UnityEngine;
using UnityEngine.SceneManagement;

public class DefeatActions : MonoBehaviour
{
    public void RestartLevel()
    {
        
        Time.timeScale = 1f;
        
       
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
      
    }

    public void GoToMainMenu(string MainMenu)
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(MainMenu);
       
    }
}