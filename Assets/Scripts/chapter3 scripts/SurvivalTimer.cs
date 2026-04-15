using UnityEngine;
using TMPro; // Para el texto de la UI

public class SurvivalTimer : MonoBehaviour
{
    [Header("confi Timer")]
    [SerializeField] private float timeRemaining = 60f; 
    private bool timerIsRunning = false;

    [Header("canvas UI")]
    [SerializeField] private TextMeshProUGUI timerText;

    [Header("Unlock")]
    [SerializeField] private GameObject doorObject; 

    
    public bool IsRunning()
    {
        return timerIsRunning;
    }
    
    public void StartTimer()
    {
        timerIsRunning = true;
        
    }

    private void Update()
    {
        if (timerIsRunning)
        {
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;
                DisplayTime(timeRemaining);
            }
            else
            {
                timeRemaining = 0;
                timerIsRunning = false;
                FinishSurvivalEvent();
            }
        }
    }

    private void DisplayTime(float timeToDisplay)
    {
        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    private void FinishSurvivalEvent()
    {
        
        EnemySpawner[] allSpawners = Object.FindObjectsByType<EnemySpawner>(FindObjectsSortMode.None);
        foreach (EnemySpawner spawner in allSpawners)
        {
            spawner.StopSpawning();
        }

        
        if (doorObject != null)
        {
            
            Destroy(doorObject);
            
        }
        GameObject[] remainingEnemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in remainingEnemies)
        {
            
            Destroy(enemy); 
        }
    }

    
    public bool IsSurvivalComplete()
    {
        return timeRemaining <= 0;
    }
}