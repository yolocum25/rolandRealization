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
        // Buscamos el PlayerInput que ya tienes en el Personaje
        _playerInput = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInput>();
        dialoguePanel.SetActive(false);
    }

    public void StartDialogue(Sprite characterSprite, string message)
    {
        // 1. Activamos el Panel
        dialoguePanel.SetActive(true);
        
        // 2. Cambiamos el texto y la imagen
        textDisplay.text = message;
        portraitDisplay.sprite = characterSprite;

        // 3. ¡AQUÍ ESTÁ LA MAGIA! Cambiamos el mapa de controles
        // Esto desactiva automáticamente el Salto y Movimiento
        _playerInput.SwitchCurrentActionMap("UI");
        
        // Congelamos el tiempo o las físicas si es necesario
        Time.timeScale = 0f; 
    }

    public void EndDialogue()
    {
        dialoguePanel.SetActive(false);
        
        // Volvemos al control del jugador
        _playerInput.SwitchCurrentActionMap("Player");
        
        Time.timeScale = 1f;
    }
    
    public void OnNextText(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            // Aquí iría la lógica para pasar a la siguiente frase
            // Si no hay más frases, llamamos a EndDialogue()
            EndDialogue();
        }
    }
}