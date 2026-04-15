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

    private void Start()
    {
        currentHealth = maxHealth;
    }

    // Este es el método que llama tu sistema de interacción (la E)
    public void GenerateInteraction()
    {
        // Si el evento ya empezó, no hacemos nada más al pulsar E
        if (eventStarted || isDead) return;

        // Buscamos al jugador (siguiendo tu lógica de la escalera)
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null && survivalTimer != null)
        {
            // 1. Efectos visuales/sonoros
            if (activationSound != null) activationSound.Play();

            // 2. Iniciamos la lógica
            eventStarted = true;
            survivalTimer.StartTimer();

            Debug.Log("<color=cyan>¡Objetivo activado por el jugador! Empezando defensa.</color>");
            
            // Opcional: Podrías cambiar el color del objeto o encender una luz aquí
        }
    }

    // Implementación de IDamageable para que los enemigos le peguen
    public void TakeDamage(float amount)
    {
        if (isDead || !eventStarted) return;

        currentHealth -= amount;

        if (currentHealth <= 0)
        {
            Die();
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