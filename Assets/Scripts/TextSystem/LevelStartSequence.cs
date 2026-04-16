using UnityEngine;
using UnityEngine.InputSystem;

public class LevelStartSequencer : MonoBehaviour
{
    [Header("Referencias de UI")]
    [SerializeField] private GameObject narrativeCanvas; 
    [SerializeField] private GameObject gameHUD;          

    [Header("Referencias del Jugador")]
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private PlayerAttackSystem playerAttack;
    
    [SerializeField] private AudioSource ambientAudioSource;
    

    private void Awake()
    {
       
        Time.timeScale = 0f;
        
        if (gameHUD != null) gameHUD.SetActive(false);
        if (narrativeCanvas != null) narrativeCanvas.SetActive(true);
        
        if (ambientAudioSource != null)
        {
            ambientAudioSource.loop = true;
            ambientAudioSource.ignoreListenerPause = true;
            ambientAudioSource.Play();
        }
    }

    private void Start()
    {
        InitiateNarrativeMode();
    }

    private void InitiateNarrativeMode()
    {

        if (playerInput != null) playerInput.enabled = true;
            
        playerInput.SwitchCurrentActionMap("UI");

        if (playerAttack != null) playerAttack.enabled = false;
        // 2. Liberamos el ratón para que el jugador pueda interactuar con el diálogo
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        // 3. Aquí es donde tu DialogueSystem empezaría a escribir.
        // Si tu script de diálogo necesita un "Play", llámalo aquí.
    }

    /// <summary>
    /// ESTE MÉTODO DEBE SER LLAMADO POR EL DIALOGUE SYSTEM AL TERMINAR
    /// </summary>
    public void EndNarrativeAndStartGame()
    {
        Time.timeScale = 1f;

        // 1. Intercambio de Canvases
        if (narrativeCanvas != null) narrativeCanvas.SetActive(false);
        if (gameHUD != null) gameHUD.SetActive(true);

    
        if (playerInput != null) 
        {
            playerInput.enabled = true; 
            playerInput.SwitchCurrentActionMap("Player"); 
        }
        
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        
       
        
        
    }
    
    
    
}
    
