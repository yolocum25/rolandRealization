using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Collections;

public class DialogueSystem : MonoBehaviour
{
    [Header("UI References")]
    public GameObject narrationCanvas;
    public TextMeshProUGUI dialogueText;
    public Image portraitImage;

    [Header("Content")]
    [TextArea(3, 5)] public string[] sentences;
    public Sprite[] portraits; 
    [SerializeField] private float typingSpeed = 0.05f;

    private int index;
    private bool isTyping;
    private PlayerInput playerInput;

    void Awake()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null) playerInput = player.GetComponent<PlayerInput>();
    }

    // Nombre exacto: ActivateDialogue
    public void ActivateDialogue()
    {
        index = 0;
        narrationCanvas.SetActive(true);
        
        if (playerInput != null) playerInput.SwitchCurrentActionMap("UI");
        
        StartCoroutine(TypeSentence());
    }

    IEnumerator TypeSentence()
    {
        isTyping = true;
        dialogueText.text = "";
        
        if (portraits.Length > index && portraits[index] != null)
            portraitImage.sprite = portraits[index];

        foreach (char letter in sentences[index].ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSecondsRealtime(typingSpeed);
        }
        isTyping = false;
    }

    public void OnNextSentence(InputAction.CallbackContext context)
    {
        if (context.performed) HandleAdvance();
    }

    public void NextSentenceManual()
    {
        HandleAdvance();
    }

    private void HandleAdvance()
    {
        // Verificamos que el array no esté vacío y el índice sea válido
        if (sentences == null || sentences.Length == 0 || index >= sentences.Length)
        {
            FinishDialogue();
            return;
        }

        if (isTyping)
        {
            StopAllCoroutines();
            dialogueText.text = sentences[index]; // Aquí daba el error
            isTyping = false;
        }
        else
        {
            index++;
            if (index < sentences.Length)
            {
                StartCoroutine(TypeSentence());
            }
            else
            {
                FinishDialogue();
            }
        }
    }
    void FinishDialogue()
    {
        // Buscamos el manager con el nuevo nombre
        TutorialManager tutorial = Object.FindFirstObjectByType<TutorialManager>();
        
        if (tutorial != null)
        {
            tutorial.EndTutorialNarrative();
        }
        else
        {
            narrationCanvas.SetActive(false);
            if (playerInput != null) playerInput.SwitchCurrentActionMap("Player");
            Destroy(gameObject);
        }
    }
}