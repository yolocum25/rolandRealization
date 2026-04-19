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
    [Header("Refe")]
    [SerializeField] private GameObject playerGameObject; 
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private PlayerAttackSystem playerAttack;
    [SerializeField] private Animator playerAnimator; 

    [Header("Confi de Movement")]
    [SerializeField] private Transform walkToPoint; 
    [SerializeField] private float walkSpeed = 2f;
    [SerializeField] private string speedParameterName = "Speed"; 

    [Header("Audio")]
    [SerializeField] private AudioSource cinematicMusicSource;
    
    
    [Header("Cinemachine")]
    [SerializeField] private CinemachineCamera finalVirtualCamera;
    
    [Header("Progress")]
    [SerializeField] private string currentLevelName = "Sudenly One Day...";
    
    [Header("UI & Transition")]
    [SerializeField] private Image fadeOverlay; 
    [SerializeField] private float waitTimeAfterWalking = 2f;
    [SerializeField] private float fadeDuration = 2f;
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    private bool isCinematicStarted = false;

    private void Awake()
    {
      
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
            
            StartCoroutine(EndLevelRoutine());
        }
    }

    private IEnumerator EndLevelRoutine()
    {
        isCinematicStarted = true;
        if (cinematicMusicSource != null)
        {
            cinematicMusicSource.Play();
        }

        
        if (playerInput != null) playerInput.enabled = false;
        if (playerAttack != null) playerAttack.enabled = false;

       
        if (finalVirtualCamera != null) finalVirtualCamera.Priority = 100;

       
        if (walkToPoint != null && playerGameObject != null)
        {
           
            if (playerAnimator != null) playerAnimator.SetFloat(speedParameterName, 1f);

            
            float distance = Vector2.Distance(playerGameObject.transform.position, walkToPoint.position);
            
            
            while (distance > 0.3f)
            {
               
                Vector3 targetPos = new Vector3(walkToPoint.position.x, playerGameObject.transform.position.y, playerGameObject.transform.position.z);
                
                playerGameObject.transform.position = Vector2.MoveTowards(
                    playerGameObject.transform.position, 
                    targetPos, 
                    walkSpeed * Time.deltaTime
                );
                
                distance = Vector2.Distance(playerGameObject.transform.position, targetPos);
                
                
                yield return null; 
            }

            
            if (playerAnimator != null) playerAnimator.SetFloat(speedParameterName, 0f);
        }

        
        yield return new WaitForSeconds(waitTimeAfterWalking);
        LevelCheckerManager.MarkLevelAsCompleted(currentLevelName);

       
        yield return StartCoroutine(FadeToBlack());

        
        SceneManager.LoadScene(mainMenuSceneName);
        Cursor.lockState = CursorLockMode.None; 
        Cursor.visible = true;
    }

    private IEnumerator FadeToBlack()
    {
        if (fadeOverlay == null)
        {
            
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
        
    }
}
}