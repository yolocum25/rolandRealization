using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Player;

public class StaggerUIFeedback : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private CharactersHealthSystem healthSystem;
    [SerializeField] private Image stateIndicatorImage; 

    [Header("Configuración Visual")]
    [SerializeField] private Color normalColor = Color.green;
    [SerializeField] private Color staggeredColor = Color.yellow;
    [SerializeField] private float staggerDuration = 4.0f;

    [Header("Configuración de Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip staggerSound;

    private void OnEnable()
    {
        if (healthSystem != null)
            healthSystem.OnStagger += StartStaggerFeedback;
    }

    private void OnDisable()
    {
        if (healthSystem != null)
            healthSystem.OnStagger -= StartStaggerFeedback;
    }

    private void Start()
    {
        if (stateIndicatorImage != null)
            stateIndicatorImage.color = normalColor;
    }

    private void StartStaggerFeedback()
    {
       
        StartCoroutine(StaggerSequence());
    }

    private IEnumerator StaggerSequence()
    {
        
        if (stateIndicatorImage != null)
            stateIndicatorImage.color = staggeredColor;
        
        if (audioSource != null && staggerSound != null)
            audioSource.PlayOneShot(staggerSound);
        
        yield return new WaitForSeconds(staggerDuration);
        if (stateIndicatorImage != null)
            
            stateIndicatorImage.color = normalColor;
    }
}