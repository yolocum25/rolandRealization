using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuScene : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        
    }

    public void OptionsMenu()
    {
        SceneManager.LoadScene("Options");
    }
    
    
    public void QuitGame()
    {
        UnityEditor.EditorApplication.isPlaying = false;
    }
}
