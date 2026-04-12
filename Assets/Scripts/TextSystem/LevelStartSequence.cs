using UnityEngine;
using UnityEngine.InputSystem;

public class LevelStartSequencer : MonoBehaviour
{
    [Header("Referencias de UI")]
    [SerializeField] private GameObject narrativeCanvas; // El de los diálogos
    [SerializeField] private GameObject gameHUD;          // El de vida/timer

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

        // 1. Bloqueamos al jugador por completo
        if (playerInput != null) playerInput.enabled = false;
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

    
        if (playerInput != null) playerInput.enabled = true;
        if (playerAttack != null) playerAttack.enabled = true;
        
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        
       
        
        
    }
    
    
    
}
    
