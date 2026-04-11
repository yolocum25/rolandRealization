using UnityEngine;

public class VictoryManager : MonoBehaviour
{
    [Header("configurationWin")]
    [SerializeField] private int enemiesToWin = 9;
    [SerializeField] private string enemyTag = "Enemy";

    [Header("ActualSate")]
    [SerializeField] private int currentKills = 0;
    private bool hasWon = false;

    private void Start()
    {
        if (EventManager.Instance != null)
        {
            EventManager.Instance.OnCharacterDead -= HandleCharacterDeath; 
            EventManager.Instance.OnCharacterDead += HandleCharacterDeath;
        }
    }
    private void OnEnable()
    {
        if (EventManager.Instance != null)
        {
            EventManager.Instance.OnCharacterDead += HandleCharacterDeath;
        }
    }

    private void OnDisable()
    {
        if (EventManager.Instance != null)
        {
            EventManager.Instance.OnCharacterDead -= HandleCharacterDeath;
        }
    }

    private void HandleCharacterDeath(GameObject deadCharacter)
    {
        if (hasWon) return;
        
        if (deadCharacter.CompareTag(enemyTag))
        {
            currentKills++;
           ;

            if (currentKills >= enemiesToWin)
            {
                TriggerVictory();
            }
        }
    }

    private void TriggerVictory()
    {
        hasWon = true;
        if (EventManager.Instance != null)
        {
            EventManager.Instance.Victory();
        }
    }
}