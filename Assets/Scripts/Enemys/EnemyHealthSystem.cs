using UnityEngine;

public class EnemyHealthSystem : charactersHealthSystem
{
    
    
    public override void TakeDamage(float damage)
    {
        
        base.TakeDamage(damage);
        
        Debug.Log($"Enemy Hit: {currentHealth}/{maxhealth}");
    }
}