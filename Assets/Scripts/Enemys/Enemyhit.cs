using UnityEngine;

public class EnemyTouchDamage : MonoBehaviour
{
    [Header("Configuración de Ataque")]
    [SerializeField] private float damageToPlayer = 15f;
    [SerializeField] private float attackInterval = 1.0f; // Daño cada 1 segundo
    
    private float nextAttackTime;
    
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (Time.time >= nextAttackTime)
            {
                // Busca el sistema de salud
                var health = collision.gameObject.GetComponent<charactersHealthSystem>();
            
                if (health != null) 
                {
                    health.TakeDamage(damageToPlayer);
                    nextAttackTime = Time.time + attackInterval;
                }
            }
        }
    }
}