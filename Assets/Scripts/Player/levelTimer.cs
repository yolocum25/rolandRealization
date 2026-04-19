using UnityEngine;
using TMPro;

public class LevelTimer : MonoBehaviour
{
    public static LevelTimer Instance;

    [Header("Ajustes de Tiempo")]
    [SerializeField] private float timeLimit = 300f; 
    private float currentTime;
    private bool timerRunning = true;

    [Header("Sounds")]
    [SerializeField] private AudioSource AudioSource;
    
    
    [Header("Referencias de UI")]
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private Color warningColor = Color.red;
    private Color originalColor;

    public bool IsTimeUp()
    {
        return currentTime <= 0;
    }
    
    private void Awake()
    {
        if (AudioSource != null && !AudioSource.isPlaying)
        {
            AudioSource.Play();
        }
        if (Instance == null) Instance = this;
        
        currentTime = timeLimit;
        if (timerText != null) originalColor = timerText.color;
    }

    private void OnEnable()
    {
        if (EventManager.Instance != null)
            EventManager.Instance.OnVictory += StopTimer;
    }

    private void OnDisable()
    {
        if (EventManager.Instance != null)
            EventManager.Instance.OnVictory -= StopTimer;
    }

    void Update()
    {
        if (!timerRunning) return;

        if (currentTime > 0)
        {
            currentTime -= Time.deltaTime;
            UpdateTimerDisplay();
            
           
            if (currentTime <= 30f && timerText != null)
            {
                timerText.color = warningColor;
            }
        }
        else
        {
            currentTime = 0;
            timerRunning = false;
            HandleTimeOut();
        }
    }

    void UpdateTimerDisplay()
    {
        if (timerText == null) return;

        int minutes = Mathf.FloorToInt(currentTime / 60);
        int seconds = Mathf.FloorToInt(currentTime % 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    private void StopTimer() => timerRunning = false;

    private void HandleTimeOut()
    {
        if (EventManager.Instance != null)
        {
            EventManager.Instance.GameOver();
        }
    }

   
    public string GetTimeElapsedFormatted()
    {
        float elapsed = timeLimit - currentTime;
        int minutes = Mathf.FloorToInt(elapsed / 60);
        int seconds = Mathf.FloorToInt(elapsed % 60);
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}