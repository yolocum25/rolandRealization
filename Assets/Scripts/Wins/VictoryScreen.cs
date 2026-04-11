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
            killsText.text = "Defeated Enemys: <color=orange>9 / 9</color>";
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
}