using System;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public static EventManager Instance {get; private set; }
    
    
    public event Action< float,float >  OnPlayerDamaged;
    
    public event Action<GameObject> OnCharacterDead;
    
    public event Action OnVictory;
    
    public event Action OnGameOver;
    
    

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        
        
        
    }

    public void PlayerDamage(float currentHealth, float maxHealth)
    {
        OnPlayerDamaged?.Invoke(currentHealth, maxHealth);
    }
    
    public void CharacterDead(GameObject character)
    {
        OnCharacterDead?.Invoke(character);
        
    }
    
    public void Victory()
    {
        OnVictory?.Invoke();
    }

    public void GameOver()
    {
        OnGameOver?.Invoke();
    }
}


