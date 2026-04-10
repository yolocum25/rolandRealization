using UnityEngine;

public class EnemyHealthSystem : charactersHealthSystem
{
    
   
    
    
    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);
        
        if (EmotionManager.Instance != null)
        {
            // Subimos la barra azul del jugador
            EmotionManager.Instance.GainPositive(damage * 0.5f);
        }
    }
    
    public void ResetStagger()
    {
        staggered = false;
    }
    
    
    
}