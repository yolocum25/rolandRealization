using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Collections;

public class DialogueSystem : MonoBehaviour
{
    #region AnimParameters
    
    private static readonly int AttackTrigger = Animator.StringToHash("attack");
    #endregion
    
    
    [Header("UI References")]
    public GameObject narrationCanvas;
    public TextMeshProUGUI dialogueText;
    public Image portraitImage;
    
    [Header("References")]
    [SerializeField] private Animator anim;
    

    [Header("Content")]
    [TextArea(3, 5)] public string[] sentences;
    public Sprite[] portraits; 
    public AudioClip[] dialogueSounds;
    [SerializeField] private float typingSpeed = 0.05f;
    [SerializeField] private PlayerAttackSystem playerAttackScript;

    [Header("Audio Settings")]
    private AudioSource audioSource;
    
    [Header("Skip Settings")]
    [SerializeField] private bool canSkip = true;
    
    private int index;
    private bool isTyping;
    private PlayerInput playerInput;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null) playerInput = player.GetComponent<PlayerInput>();
        
        
    }

    
    public void SkipAllDialogue()
    {
        StopAllCoroutines();
        if (audioSource != null) audioSource.Stop();
        FinishDialogue();
    }
    public void ActivateDialogue()
    {
        index = 0;
        narrationCanvas.SetActive(true);

        if (playerInput != null) playerInput.SwitchCurrentActionMap("UI");
        if (playerAttackScript != null) playerAttackScript.enabled = false;;
        
        StartCoroutine(TypeSentence());
    }

    IEnumerator TypeSentence()
    {
        isTyping = true;
        dialogueText.text = "";
        
        if (portraits.Length > index && portraits[index] != null)
            portraitImage.sprite = portraits[index];
        
        if (audioSource != null && dialogueSounds.Length > index && dialogueSounds[index] != null)
        {
            audioSource.clip = dialogueSounds[index];
            audioSource.Play();
        }

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
        if (sentences == null || sentences.Length == 0 || index >= sentences.Length)
        {
            FinishDialogue();
            return;
        }

        if (isTyping)
        {
            StopAllCoroutines();
            dialogueText.text = sentences[index]; 
            isTyping = false;
            if (audioSource != null) audioSource.Stop();
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
                if (playerAttackScript != null) playerAttackScript.enabled = true;
            }
        }
    }
    void FinishDialogue()
    {
        isTyping = false; 

        
        if (anim != null)
        {
            anim.Play("Idle"); 
            anim.SetBool("attacking", false); 
        }

      
        if (playerInput != null) 
        {
            playerInput.SwitchCurrentActionMap("Player");
        }

      
        if (playerAttackScript != null) 
        {
            playerAttackScript.enabled = true;
            playerAttackScript.CloseAttackWindow(); 
        }

       
        TutorialManager tutorial = Object.FindFirstObjectByType<TutorialManager>();
        if (tutorial != null)
        {
            tutorial.EndTutorialNarrative();
        }
        else
        {
            narrationCanvas.SetActive(false);
            Destroy(gameObject);
        }
    }
    
}