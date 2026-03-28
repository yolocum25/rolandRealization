using System;
using Unity.VisualScripting;
using UnityEngine;

public class charactersHealthSystem : MonoBehaviour,IDamageable
{
    [SerializeField] protected float maxhealth = 100;
    
    //este es un evento que se disparara cuando me hagan daño
    public event Action OnDamaged;
    
    
    protected float currentHealth;


    protected virtual void Awake()
    {
        currentHealth = maxhealth;
    }


    public virtual void TakeDamage (float damage)
    {
        currentHealth -= damage;
        OnDamaged.Invoke();
        
        if (currentHealth <= 0)
        {
            Destroy(gameObject);
        }
        
        
    }
}
