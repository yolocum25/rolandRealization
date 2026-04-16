using Unity.Cinemachine;

namespace chapter3_scripts
{
   using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;


public class LevelEndCinematic2D : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private GameObject playerGameObject; 
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private PlayerAttackSystem playerAttack;
    [SerializeField] private Animator playerAnimator; 

    [Header("Configuración de Movimiento")]
    [SerializeField] private Transform walkToPoint; 
    [SerializeField] private float walkSpeed = 2f;
    [SerializeField] private string speedParameterName = "Speed"; 

    [Header("Cinemachine")]
    [SerializeField] private CinemachineCamera finalVirtualCamera;
    
    [Header("UI & Transición")]
    [SerializeField] private Image fadeOverlay; 
    [SerializeField] private float waitTimeAfterWalking = 2f;
    [SerializeField] private float fadeDuration = 2f;
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    private bool isCinematicStarted = false;

    private void Awake()
    {
        // Aseguramos que empiece invisible
        if (fadeOverlay != null)
        {
            Color c = fadeOverlay.color;
            c.a = 0f;
            fadeOverlay.color = c;
            fadeOverlay.gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isCinematicStarted)
        {
            Debug.Log("PASO 1: Jugador detectado en el Trigger");
            StartCoroutine(EndLevelRoutine());
        }
    }

    private IEnumerator EndLevelRoutine()
    {
        isCinematicStarted = true;

        // Bloquear controles
        if (playerInput != null) playerInput.enabled = false;
        if (playerAttack != null) playerAttack.enabled = false;

        // Activar cámara
        if (finalVirtualCamera != null) finalVirtualCamera.Priority = 100;

        // Movimiento
        if (walkToPoint != null && playerGameObject != null)
        {
            Debug.Log("PASO 2: Empezando a caminar...");
            if (playerAnimator != null) playerAnimator.SetFloat(speedParameterName, 1f);

            // Bucle de movimiento
            float distance = Vector2.Distance(playerGameObject.transform.position, walkToPoint.position);
            while (distance > 0.1f)
            {
                playerGameObject.transform.position = Vector2.MoveTowards(
                    playerGameObject.transform.position, 
                    walkToPoint.position, 
                    walkSpeed * Time.deltaTime
                );
                
                distance = Vector2.Distance(playerGameObject.transform.position, walkToPoint.position);
                yield return null; 
            }

            Debug.Log("PASO 3: Llegué al punto. Deteniendo animación.");
            if (playerAnimator != null) playerAnimator.SetFloat(speedParameterName, 0f);
        }

        Debug.Log("PASO 4: Esperando pausa dramática...");
        yield return new WaitForSeconds(waitTimeAfterWalking);

        Debug.Log("PASO 5: Iniciando Fade a negro...");
        yield return StartCoroutine(FadeToBlack());

        Debug.Log("PASO 6: Cargando escena: " + mainMenuSceneName);
        SceneManager.LoadScene(mainMenuSceneName);
    }

    private IEnumerator FadeToBlack()
    {
        if (fadeOverlay == null)
        {
            Debug.LogError("¡ERROR! No hay imagen asignada en FadeOverlay");
            yield break;
        }

        fadeOverlay.gameObject.SetActive(true);
        Color c = fadeOverlay.color;
        float t = 0;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            c.a = Mathf.Lerp(0f, 1f, t / fadeDuration);
            fadeOverlay.color = c;
            yield return null;
        }
        
        c.a = 1f;
        fadeOverlay.color = c;
        Debug.Log("Fade Out completado al 100%");
    }
}
}