using System;
using Unity.VisualScripting;
using UnityEngine;

public class charactersHealthSystem : MonoBehaviour,IDamageable
{
    [SerializeField] protected float maxhealth = 186;
    [SerializeField] protected float staggerThreshold = 70f;
    
    public event Action OnDamaged;
    public event Action OnStagger;
    
  
    public float GetCurrentHealth() => currentHealth;
    public float GetMaxHealth() => maxhealth;
    protected float currentHealth;
    protected bool staggered = false;
    public bool isInvulnerable = false;
    
    
    protected virtual void Awake()
    {
        currentHealth = maxhealth;
    }


    public virtual void TakeDamage (float damage)
    {
        currentHealth -= damage;
        OnDamaged?.Invoke();
        
        if (currentHealth <= staggerThreshold && !staggered)
        {
            staggered = true;
            OnStagger?.Invoke();
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }
    protected virtual void Die()
    {
        // 1. Avisar al mundo (Evento)
        if (EventManager.Instance != null)
            EventManager.Instance.CharacterDead(this.gameObject);

        // 2. Llamada directa de seguridad (Esto garantiza que funcione)
        CharacterDeathVisuals visuals = GetComponent<CharacterDeathVisuals>();
        if (visuals != null)
        {
            visuals.HandleDeath(this.gameObject);
        }
    }
}
