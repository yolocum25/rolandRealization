using System;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public static EventManager Instance {get; private set; }
    
    
    public event Action< float,float >  OnPlayerDamaged;
    
    
    

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
}