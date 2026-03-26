using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuScene : MonoBehaviour
{
    
    public event Action OnFirstTimeSeen;
    
    public void StartGame(bool Skip)
    {
        
        OnFirstTimeSeen?.Invoke();
        EventManager.Instance.IntroScene(false);
        
        
        
        
        
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
