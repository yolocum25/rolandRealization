using Player;
using UnityEngine;

public class EnemyHealthSystem : CharactersHealthSystem
{
    
   
    
    
    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);
        
        if (EmotionManager.Instance != null)
        {
            EmotionManager.Instance.GainPositive(damage * 0.5f);
        }
    }
    
    public void ResetStagger()
    {
        staggered = false;
    }
    
    
    
}