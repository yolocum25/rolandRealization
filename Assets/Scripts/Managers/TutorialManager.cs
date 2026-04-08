using UnityEngine;
using UnityEngine.InputSystem;

public class TutorialManager : MonoBehaviour
{
    
    
    [Header("Canvas References")]
    public GameObject canvasNarration; 
    public GameObject canvasUI_HUD;    

    [Header("Dialogue System")]
    public DialogueSystem dialogueSystem; 

    private PlayerInput playerInput;

    void Awake()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerInput = player.GetComponent<PlayerInput>();
        }
    }

    void Start()
    {
        canvasUI_HUD.SetActive(false);     
        canvasNarration.SetActive(true);   
        
        if (playerInput != null)
        {
            playerInput.SwitchCurrentActionMap("UI");
        }

        // Cambiado a "ActivateDialogue" para que coincida con el otro script
        if (dialogueSystem != null)
        {
            dialogueSystem.ActivateDialogue();
        }
    }

    public void EndTutorialNarrative()
    {
        if (canvasNarration != null) Destroy(canvasNarration);      
        
        if (canvasUI_HUD != null) canvasUI_HUD.SetActive(true);  
        
        if (playerInput != null)
        {
            playerInput.SwitchCurrentActionMap("Player");
        }
        
        Destroy(gameObject);
    }
}