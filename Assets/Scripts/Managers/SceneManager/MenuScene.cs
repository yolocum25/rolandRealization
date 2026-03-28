using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuScene : MonoBehaviour
{
    
    
    
    public void StartGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void OptionsMenu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Options");
    }
    
    
    public void QuitGame()
    {
        UnityEditor.EditorApplication.isPlaying = false;
    }
}
