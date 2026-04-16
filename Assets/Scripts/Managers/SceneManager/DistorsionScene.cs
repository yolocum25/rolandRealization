using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Collections;
using UnityEngine.Video;


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
    [SerializeField] private GameObject dialogueUI;
    
    [Header("Imágenes (UI Image)")] 
    public Image rolandSeenAngelica;
    public Image backScreen;
    public Image distorsedRoland;
    public Image rolandRelise;
    
    [Header("Videos/Texturas (RawImage)")]
    public RawImage angelicaTalks;
    public RawImage distorsion;
    public RawImage timeParadox;
    public RawImage rolandEnd;
    
    [Header("Audio Sources")]
    public AudioSource voiceSource;      
    public AudioSource ambientAudioSource;
    public AudioSource endingMusicSource;
    
    [Header("Configuración de Diálogos")]
    public TextMeshProUGUI dialogueText;
    [SerializeField] private float typingSpeed = 0.05f;
    [SerializeField] private float fadeDuration = 0.8f;
    
    
    [Header("Secuencias de Diálogo")]
    public DialogueLine[] block1; 
    public int indexToChangeToAngelica; 
    public DialogueLine[] block2; 
    public DialogueLine[] block3; 
    public DialogueLine[] block4; 
    public int indexToRelise;

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
        
        yield return StartCoroutine(FadeIn(rolandSeenAngelica));
        // angelicaTalks.gameObject.SetActive(false);
        // backScreen.gameObject.SetActive(false);;
        
        for (int i = 0; i < block1.Length; i++)
        {
            if (i == indexToChangeToAngelica)
            {
                // rolandSeenAngelica.gameObject.SetActive(false);
                // angelicaTalks.gameObject.SetActive(true);
                yield return StartCoroutine(FadeOut(rolandSeenAngelica));
                yield return StartCoroutine(FadeIn(angelicaTalks));
            }
            yield return StartCoroutine(PlayDialogue(block1[i]));
        }

        // --- PARTE 2: Pantalla Negra y Sonido Ambiente ---
        // angelicaTalks.gameObject.SetActive(false);
        // backScreen.gameObject.SetActive(true); 
        // distorsion.gameObject.SetActive(true); 
        // --- PARTE 2: Distorsión ---
        // --- PARTE 2: Distorsión ---
        yield return StartCoroutine(FadeOut(angelicaTalks));
        Color bc = backScreen.color;
        bc.a = 1f;
        backScreen.color = bc;
        if (dialogueUI != null) dialogueUI.SetActive(false); 

        yield return StartCoroutine(FadeIn(distorsion)); 

        VideoPlayer vp = distorsion.GetComponent<VideoPlayer>();

        if (vp != null)
        {
            vp.Play();
    
            // Esperamos un momento para que el video arranque
            yield return new WaitForSecondsRealtime(0.5f);

            // Si el Update Mode está en Unscaled Time, esto funcionará perfecto:
            // Esperamos hasta que el video deje de reproducirse O llegue al final
            while (vp.isPlaying && vp.time < (vp.length - 0.2f))
            {
                yield return null; 
            }
    
            // Si por alguna razón vp.time no avanza, esto es un seguro de vida:
            // Si el video se queda "congelado", forzamos la salida tras su duración real
            // yield return new WaitForSecondsRealtime((float)vp.length);
        }
        else
        {
            yield return new WaitForSecondsRealtime(60f);
        }

        yield return StartCoroutine(FadeOut(distorsion));
        if (dialogueUI != null) dialogueUI.SetActive(true);

       

        // --- PARTE 3: Distorsión y Roland Distorsionado ---
        // distorsion.gameObject.SetActive(false);
        // distorsedRoland.gameObject.SetActive(true);
        
        yield return StartCoroutine(FadeIn(distorsedRoland));
        
        for (int i = 0; i < block2.Length; i++)
        {
            yield return StartCoroutine(PlayDialogue(block2[i]));
        }
        for (int i = 0; i < block3.Length; i++)
        {
            yield return StartCoroutine(PlayDialogue(block3[i]));
        }

        // --- PARTE 4: Time Paradox ---
        Debug.Log("Iniciando transición a Paradox...");
        
        // 1. Forzamos el FadeOut pero con un seguro
        yield return StartCoroutine(FadeOut(distorsedRoland));
        
        // 2. SEGURO DE VIDA: Si después del Fade sigue activo, lo apagamos a la fuerza
        if (distorsedRoland != null) distorsedRoland.gameObject.SetActive(false); 

        // 3. Ocultar UI
        if (dialogueUI != null) dialogueUI.SetActive(false); 

        // 4. Mostrar Paradox
        yield return StartCoroutine(FadeIn(timeParadox));

        VideoPlayer vpPara = timeParadox.GetComponent<VideoPlayer>();
        if (vpPara != null)
        {
            Debug.Log("Reproduciendo Video Paradox");
            vpPara.Play();
            yield return new WaitForSecondsRealtime(0.5f);
            while (vpPara.isPlaying && vpPara.time < (vpPara.length - 0.2f))
            {
                yield return null; 
            }
        }
        else
        {
            Debug.LogWarning("No se encontró VideoPlayer en TimeParadox, esperando 25s");
            yield return new WaitForSecondsRealtime(40f);
        }

       // La UI se activa AQUÍ para empezar a leer las frases sobre fondo negro
       
       if (endingMusicSource != null) 
       {
           endingMusicSource.volume = 1f; // Nos aseguramos de que tenga volumen
           endingMusicSource.Play();
       }
       
        if (dialogueUI != null) dialogueUI.SetActive(true); 

        for (int i = 0; i < block4.Length; i++)
        {
            // Cuando llegamos al índice marcado...
            if (i == indexToRelise)
            {
                // Aparece Roland Relise mientras el texto sigue ahí
                yield return StartCoroutine(FadeIn(rolandRelise));
                
                // OPCIONAL: Si por algún motivo la UI se ocultó, 
                // nos aseguramos de que esté activa al aparecer la imagen
                if (dialogueUI != null) dialogueUI.SetActive(true);
            }
            
            yield return StartCoroutine(PlayDialogue(block4[i]));
        }

        // --- FINAL: Video Roland End ---
        // Ahora sí, ocultamos todo para el gran final
        if (dialogueUI != null) dialogueUI.SetActive(false); 
        if (rolandRelise.gameObject.activeSelf) yield return StartCoroutine(FadeOut(rolandRelise));

        yield return StartCoroutine(FadeIn(rolandEnd));
        // ... (Aquí va tu bloque de VideoPlayer para rolandEnd que ya sabes que funciona)

        VideoPlayer vpEnd = rolandEnd.GetComponent<VideoPlayer>();
        if (vpEnd != null)
        {
            vpEnd.Play();
            yield return new WaitForSecondsRealtime(0.5f);
            while (vpEnd.isPlaying && vpEnd.time < (vpEnd.length - 0.2f))
            {
                yield return null; 
            }
        }
        else
        {
            yield return new WaitForSecondsRealtime(10f);
        }

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
    
        
        if (endingMusicSource != null)
        {
            endingMusicSource.Stop();        // Nos aseguramos de que no esté sonando
            endingMusicSource.playOnAwake = false; // Refuerzo por código
        }
        
        // --- SOLUCIÓN AL FLASH DE LA CÁMARA ---
        if (backScreen != null)
        {
            backScreen.gameObject.SetActive(true); // Siempre encendido
            Color c = backScreen.color;
            c.a = 1f; // Siempre opaco (negro total)
            backScreen.color = c;
        
            // Esto fuerza a que se mueva al fondo de la UI (pero dentro del Canvas)
            backScreen.transform.SetAsFirstSibling(); 
        }

        // Ponemos el Alpha a 0 de todo lo demás
        SetImageAlpha(rolandSeenAngelica, 0);
        SetImageAlpha(distorsedRoland, 0);
        SetRawImageAlpha(angelicaTalks, 0);
        SetRawImageAlpha(distorsion, 0);
        SetRawImageAlpha(timeParadox, 0);
    }

    
    private IEnumerator PlayVideoAndWait(RawImage target)
    {
        VideoPlayer vp = target.GetComponent<VideoPlayer>();
        if (vp != null)
        {
            vp.Play();
            yield return new WaitForSecondsRealtime(0.5f);
            while (vp.isPlaying && vp.time < (vp.length - 0.2f)) yield return null;
        }
        else
        {
            yield return new WaitForSecondsRealtime(3f);
        }
    }
    // Función específica para Image
    private void SetImageAlpha(Image img, float alpha)
    {
        if (img != null)
        {
            Color c = img.color;
            c.a = alpha;
            img.color = c;
            img.gameObject.SetActive(alpha > 0); // Solo se activa si el alpha es mayor a 0
        }
    }

    // Función específica para RawImage
    private void SetRawImageAlpha(RawImage img, float alpha)
    {
        if (img != null)
        {
            Color c = img.color;
            c.a = alpha;
            img.color = c;
            img.gameObject.SetActive(alpha > 0);
        }
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
    
    
    private IEnumerator FadeIn(Graphic element) // Graphic engloba Image y RawImage
    {
        if (element == null) yield break;
        element.gameObject.SetActive(true);
        Color c = element.color;
        float t = 0;
        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;
            c.a = Mathf.Lerp(0, 1, t / fadeDuration);
            element.color = c;
            yield return null;
        }
        c.a = 1;
        element.color = c;
    }

    private IEnumerator FadeOut(Graphic element)
    {
        if (element == null) yield break;
        Color c = element.color;
        float t = 0;
        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;
            c.a = Mathf.Lerp(1, 0, t / fadeDuration);
            element.color = c;
            yield return null;
        }
        c.a = 0;
        element.color = c;
        element.gameObject.SetActive(false);
    }
}