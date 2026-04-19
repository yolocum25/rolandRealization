using UnityEngine;
using TMPro; 
using UnityEngine.UI; 
using UnityEngine.InputSystem;

public partial class DialogueManager : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject dialoguePanel;
    public TextMeshProUGUI textDisplay;
    public Image portraitDisplay;

    private PlayerInput _playerInput;

    private void Awake()
    {
       
        _playerInput = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInput>();
        dialoguePanel.SetActive(false);
    }

    public void StartDialogue(Sprite characterSprite, string message)
    {
        
        dialoguePanel.SetActive(true);
        
      
        textDisplay.text = message;
        portraitDisplay.sprite = characterSprite;

        
        _playerInput.SwitchCurrentActionMap("UI");
        
        
        Time.timeScale = 0f; 
    }

    public void EndDialogue()
    {
        dialoguePanel.SetActive(false);
        
        
        _playerInput.SwitchCurrentActionMap("Player");
        
        Time.timeScale = 1f;
    }
    
    public void OnNextText(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
           
            EndDialogue();
        }
    }
}