using UnityEngine;

public class EnemyTouchDamage : MonoBehaviour
{
    [SerializeField] private float damageToPlayer = 15f;
    [SerializeField] private float attackInterval = 1.0f;
    private float nextAttackTime;

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (Time.time >= nextAttackTime)
            {
                // Buscamos la interfaz para ser más genéricos
                if (collision.gameObject.TryGetComponent(out IDamageable health)) 
                {
                    health.TakeDamage(damageToPlayer);
                    nextAttackTime = Time.time + attackInterval;
                    
                    // IMPORTANTE: Aquí podrías añadir un aviso de Stagger al jugador 
                    // si quieres que la barra negativa suba más.
                }
            }
        }
    }
}