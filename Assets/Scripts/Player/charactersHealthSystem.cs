using System;
using Unity.VisualScripting;
using UnityEngine;

public class charactersHealthSystem : MonoBehaviour,IDamageable
{
    [SerializeField] protected float maxhealth = 186;
    [SerializeField] protected float staggerThreshold = 70f;
    
    [Header("Dead effects")]
    [SerializeField] private GameObject deathVFXPrefab;
    
    public event Action OnDamaged;
    public event Action OnStagger;
    
  
    public float GetCurrentHealth() => currentHealth;
    public float GetMaxHealth() => maxhealth;
    protected float currentHealth;
    protected bool staggered = false;

    
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
        if (deathVFXPrefab != null)
        {
            Instantiate(deathVFXPrefab, transform.position, Quaternion.identity);
        }
        Destroy(gameObject);
        
    }
}
