using UnityEngine;
using UnityEngine.InputSystem;

public class PauseManager : MonoBehaviour
{
    [Header("Configuración de Input")]
    [SerializeField] private PlayerInput pInput;
    private bool isPaused = false;

    [Header("Referencias de Scripts")]
    [SerializeField] private MonoBehaviour movementScript;
    [SerializeField] private MonoBehaviour attackScript; 

    [Header("Referencias de UI")]
    [SerializeField] private GameObject pauseCanvas;
    [SerializeField] private GameObject gameHUD;

    private void Awake()
    {
        if (pInput == null) pInput = GetComponent<PlayerInput>();
      
    }

    private void OnEnable()
    {
        
        pInput.actions["Pause"].performed += OnPauseToggle;
    }

    private void OnDisable()
    {
       
    }

  
    public void OnPauseToggle(InputAction.CallbackContext ctx)
    {
        
        if (!pInput.enabled) pInput.enabled = true;

        
        isPaused = !isPaused;

        if (isPaused)
            ActivatePause();
        else
            ResumeGame();
    }

   
    public void TogglePauseFromButton()
    {
        isPaused = !isPaused;
        if (isPaused) ActivatePause();
        else ResumeGame();
    }

    private void ActivatePause()
    {
        Time.timeScale = 0f;
        
        
        if (movementScript != null) movementScript.enabled = false;
        if (attackScript != null) attackScript.enabled = false;

       
        if (pauseCanvas != null) pauseCanvas.SetActive(true);
        if (gameHUD != null) gameHUD.SetActive(false);
        
       
        pInput.SwitchCurrentActionMap("UI");
        
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        
        Debug.Log("Juego Pausado");
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;

        
        if (movementScript != null) movementScript.enabled = true;
        if (attackScript != null) attackScript.enabled = true;

       
        if (pauseCanvas != null) pauseCanvas.SetActive(false);
        if (gameHUD != null) gameHUD.SetActive(true);
        
       
        pInput.SwitchCurrentActionMap("Player");
        
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}