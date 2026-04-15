using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefenseTarget : MonoBehaviour, IDamageable, IInteractable
{
    [Header("Ajustes de Vida")]
    [SerializeField] private float maxHealth = 500f;
    private float currentHealth;
    private bool isDead = false;
    private bool eventStarted = false;
    

    [Header("Referencias y Audio")]
    [SerializeField] private SurvivalTimer survivalTimer;
    [SerializeField] private AudioSource activationSound; // Sonido al pulsar 'E'
    [SerializeField] private GameObject timerTextUI;
    private void Start()
    {
        currentHealth = maxHealth;
    }

   
    public void GenerateInteraction()
    {
       
        if (eventStarted || isDead) return;

        // Buscamos al jugador (siguiendo tu lógica de la escalera)
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        
        if (timerTextUI != null)
        {
            timerTextUI.SetActive(true);
        }

        if (player != null && survivalTimer != null)
        {
            // 1. Efectos visuales/sonoros
            if (activationSound != null) activationSound.Play();

            // 2. Iniciamos la lógica
            eventStarted = true;
            survivalTimer.StartTimer();

            
        }
    }

    public void TakeDamage(float amount)
    {
        
        if (isDead || !gameObject.activeInHierarchy) return;

        currentHealth -= amount;

        
        if (gameObject.activeInHierarchy)
        {
            StartCoroutine(FlashRed());
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private IEnumerator FlashRed()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.color = Color.red;
            yield return new WaitForSeconds(0.1f);
            sr.color = Color.white;
        }
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        // Avisamos al EventManager para que salte el Game Over
        if (EventManager.Instance != null)
        {
            EventManager.Instance.TriggerGameOver();
        }

        // El objeto se desactiva al ser destruido
        gameObject.SetActive(false);
    }
}