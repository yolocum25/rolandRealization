using UnityEngine;
using UnityEngine.SceneManagement; 

public partial class MenuPausa : MonoBehaviour
{
    [SerializeField] private MonoBehaviour scriptPausaPlayer; 
    [SerializeField] private string nombreMetodoPausa = "OnPauseToggle";

    
    public void Resume()
    {
        scriptPausaPlayer.SendMessage(nombreMetodoPausa, default(UnityEngine.InputSystem.InputAction.CallbackContext));
    }

   
    public void ReLoad()
    {
        Time.timeScale = 1f;
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }

    
    public void GoMenu(string MainMenu)
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(MainMenu);
    }
}