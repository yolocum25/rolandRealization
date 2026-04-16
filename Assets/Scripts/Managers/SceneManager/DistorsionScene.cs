using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Collections;

// Esta clase NO hereda de MonoBehaviour porque es solo un contenedor de datos
[System.Serializable]
public class DialogueLine
{
    [TextArea(3, 5)] public string text;
    public AudioClip voice;
}

// Esta clase SÍ hereda de MonoBehaviour
public class DistorsionScene : MonoBehaviour
{
    [Header("Canvas & HUD")]
    [SerializeField] private GameObject narrativeCanvas; 
    [SerializeField] private GameObject gameHUD;     
    
    [Header("Imágenes (UI Image)")] 
    public Image rolandSeenAngelica;
    public Image backScreen;
    public Image distorsedRoland;
    
    [Header("Videos/Texturas (RawImage)")]
    public RawImage angelicaTalks;
    public RawImage distorsion;
    public RawImage timeParadox;
    
    [Header("Audio Sources")]
    public AudioSource voiceSource;      
    public AudioSource ambientAudioSource; 
    
    [Header("Configuración de Diálogos")]
    public TextMeshProUGUI dialogueText;
    [SerializeField] private float typingSpeed = 0.05f;
    
    [Header("Secuencias de Diálogo")]
    public DialogueLine[] block1; 
    public int indexToChangeToAngelica; 
    public DialogueLine[] block2; 
    public DialogueLine[] block3; 

    [Header("Referencias de Jugador")]
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private PlayerAttackSystem playerAttack;

    private bool isTyping;
    private bool playerProceed; 

    // --- ENTRADA DE USUARIO ---
    public void OnNextText(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (isTyping) isTyping = false; 
            else playerProceed = true;      
        }
    }

    private void Awake()
    {
        Time.timeScale = 0f;
        PrepareInitialState();
    }
    
    private void Start()
    {
        SetupPlayerControls();
        StartCoroutine(CinematicMasterRoutine());
    }

    private IEnumerator CinematicMasterRoutine()
    {
        // --- PARTE 1: Roland ve a Angelica ---
        rolandSeenAngelica.gameObject.SetActive(true);
        angelicaTalks.gameObject.SetActive(false);
        backScreen.gameObject.SetActive(false);;
        
        for (int i = 0; i < block1.Length; i++)
        {
            if (i == indexToChangeToAngelica)
            {
                rolandSeenAngelica.gameObject.SetActive(false);
                angelicaTalks.gameObject.SetActive(true);
            }
            yield return StartCoroutine(PlayDialogue(block1[i]));
        }

        // --- PARTE 2: Pantalla Negra y Sonido Ambiente ---
        angelicaTalks.gameObject.SetActive(false);
        backScreen.gameObject.SetActive(true); 
        distorsion.gameObject.SetActive(true); 
        
        if (ambientAudioSource != null) ambientAudioSource.Play();

        for (int i = 0; i < block2.Length; i++)
        {
            yield return StartCoroutine(PlayDialogue(block2[i]));
        }

        // --- PARTE 3: Distorsión y Roland Distorsionado ---
        distorsion.gameObject.SetActive(false);
        distorsedRoland.gameObject.SetActive(true);

        for (int i = 0; i < block3.Length; i++)
        {
            yield return StartCoroutine(PlayDialogue(block3[i]));
        }

        // --- FINAL: Time Paradox ---
        timeParadox.gameObject.SetActive(true);
        backScreen.gameObject.SetActive(true); 
        
        yield return new WaitForSecondsRealtime(3f); 

        EndCinematic();
    }

    private IEnumerator PlayDialogue(DialogueLine line)
    {
        dialogueText.text = "";
        isTyping = true;
        playerProceed = false;

        if (line.voice != null && voiceSource != null)
        {
            voiceSource.clip = line.voice;
            voiceSource.Play();
        }

        foreach (char letter in line.text.ToCharArray())
        {
            if (!isTyping) 
            {
                dialogueText.text = line.text;
                break;
            }
            dialogueText.text += letter;
            yield return new WaitForSecondsRealtime(typingSpeed);
        }

        isTyping = false;
        yield return new WaitUntil(() => playerProceed);
        playerProceed = false;
    }

    private void PrepareInitialState()
    {
        if (gameHUD != null) gameHUD.SetActive(false);
        if (narrativeCanvas != null) narrativeCanvas.SetActive(true);
        
        rolandSeenAngelica.gameObject.SetActive(false);
        angelicaTalks.gameObject.SetActive(false);
        backScreen.gameObject.SetActive(false);
        distorsion.gameObject.SetActive(false);
        distorsedRoland.gameObject.SetActive(false);
        timeParadox.gameObject.SetActive(false);
    }

    private void SetupPlayerControls()
    {
        if (playerInput != null)
        {
            playerInput.enabled = true;
            playerInput.SwitchCurrentActionMap("UI");
        }

        if (playerAttack != null) playerAttack.enabled = false;
        
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
    
    public void UI_NextButton()
    {
        if (isTyping) 
        {
            // Si está escribiendo, detenemos el efecto de escribir para mostrar el texto completo
            isTyping = false; 
        }
        else 
        {
            // Si ya terminó de escribir, permitimos que pase a la siguiente frase
            playerProceed = true;
        }
    }

    private void EndCinematic()
    { 
        Time.timeScale = 1f;
        if (narrativeCanvas != null) narrativeCanvas.SetActive(false);
        if (gameHUD != null) gameHUD.SetActive(true);
        
        if (playerInput != null) 
        {
            playerInput.SwitchCurrentActionMap("Player"); 
        }
        
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}