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
       
        EnemySpawnerV2 spawner = Object.FindFirstObjectByType<EnemySpawnerV2>();

        if (spawner != null)
        {
            spawner.StopSpawning();
        }

       
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
        
      
        if (killsText != null)
        {
            killsText.text = "All anomalies has been Obliterated!";
        }
        
       
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
           
            if (enemy != null) Destroy(enemy);
        }
    }
}