using UnityEngine;
using UnityEngine.InputSystem;

public class PauseManager : MonoBehaviour
{
    [Header("Configuración de Input")]
    [SerializeField] private PlayerInput pInput;
    private bool isPaused = false;

    [Header("Referencias de Scripts")]
    [SerializeField] private MonoBehaviour movementScript;
    [SerializeField] private MonoBehaviour attackScript; // Añadido para mayor control

    [Header("Referencias de UI")]
    [SerializeField] private GameObject pauseCanvas;
    [SerializeField] private GameObject gameHUD;

    private void Awake()
    {
        if (pInput == null) pInput = GetComponent<PlayerInput>();
        if (pInput == null) Debug.LogError("¡No hay PlayerInput encontrado!");
    }

    private void OnEnable()
    {
        // Usamos la instancia 'pInput' y nombres de acciones estándar
        // Asegúrate que en tu Action Asset se llamen así
        pInput.actions["Pause"].performed += OnPauseToggle;
    }

    private void OnDisable()
    {
        pInput.actions["Pause"].performed -= OnPauseToggle;
    }

    // Esta es la función principal que maneja todo
    public void OnPauseToggle(InputAction.CallbackContext ctx)
    {
        // 1. Seguridad: Si el componente estaba apagado (por un diálogo o stagger), lo encendemos
        if (!pInput.enabled) pInput.enabled = true;

        // 2. Invertimos el estado
        isPaused = !isPaused;

        if (isPaused)
            ActivatePause();
        else
            ResumeGame();
    }

    // Función pública para que los BOTONES del Canvas la llamen
    public void TogglePauseFromButton()
    {
        isPaused = !isPaused;
        if (isPaused) ActivatePause();
        else ResumeGame();
    }

    private void ActivatePause()
    {
        Time.timeScale = 0f;
        
        // Desactivar scripts de juego
        if (movementScript != null) movementScript.enabled = false;
        if (attackScript != null) attackScript.enabled = false;

        // UI
        if (pauseCanvas != null) pauseCanvas.SetActive(true);
        if (gameHUD != null) gameHUD.SetActive(false);
        
        // Cambiar mapa de control a UI
        pInput.SwitchCurrentActionMap("UI");
        
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        
        Debug.Log("Juego Pausado");
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;

        // Reactivar scripts de juego
        if (movementScript != null) movementScript.enabled = true;
        if (attackScript != null) attackScript.enabled = true;

        // UI
        if (pauseCanvas != null) pauseCanvas.SetActive(false);
        if (gameHUD != null) gameHUD.SetActive(true);
        
        // Volver al mapa de control del Jugador
        pInput.SwitchCurrentActionMap("Player");
        
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        
        Debug.Log("Juego Reanudado");
    }
}