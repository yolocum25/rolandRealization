using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class VictoryScreen : MonoBehaviour
{
    [Header("Componens")]
    [SerializeField] private Image winTextImage; 
    [SerializeField] private TextMeshProUGUI killsText;
    [SerializeField] private TextMeshProUGUI timeText;
    
    [Header("Sounds")]
    [SerializeField] private AudioSource victoryAudioSource;





    private void OnEnable()
    {
        // 1. Buscamos el spawner en la escena y lo apagamos
        EnemySpawnerV2 spawner = Object.FindFirstObjectByType<EnemySpawnerV2>();

        if (spawner != null)
        {
            spawner.StopSpawning();
        }

        // 2. Ahora que el spawner está muerto, borramos los enemigos
        ClearAllEnemies();

        if (EventManager.Instance != null)
            EventManager.Instance.OnVictory += DisplayStats;

        DisplayStats();
    }


    private void OnDisable()
    {
        if (EventManager.Instance != null)
            EventManager.Instance.OnVictory -= DisplayStats;
    }

    private void DisplayStats()
    {
        if (victoryAudioSource != null && !victoryAudioSource.isPlaying)
        {
            victoryAudioSource.Play();
        }
        
        // Mostramos el total de enemigos (los 9 que configuramos en el VictoryManager)
        if (killsText != null)
        {
            killsText.text = "All anomalies has been Obliterated!";
        }
        
        // Obtenemos el tiempo del LevelTimer
        if (timeText != null && LevelTimer.Instance != null)
        {
            timeText.text = "In: <color=yellow>" + LevelTimer.Instance.GetTimeElapsedFormatted() + "</color>";
        }
        
        if (winTextImage != null)
        {
           
        }
    }
    
    
    private void ClearAllEnemies()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemies)
        {
            // Solo destruimos si el objeto aún existe
            if (enemy != null) Destroy(enemy);
        }
    }
}