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
       
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        
    }

   
    public void EndNarrativeAndStartGame()
    {
        Time.timeScale = 1f;

       
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
    
