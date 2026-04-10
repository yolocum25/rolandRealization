using UnityEngine;

public class EnemyTouchDamage : MonoBehaviour
{
    [SerializeField] private float damageToPlayer = 15f;
    [SerializeField] private float attackInterval = 1.0f;
    
    [Header("Knockback Settings")]
    [SerializeField] private float knockbackForce = 8f; // Fuerza horizontal
    [SerializeField] private EnemyAI aiScript;         // Referencia a la IA

    private float nextAttackTime;
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (!this.enabled) return; 

        if (collision.gameObject.CompareTag("Player"))
        {
            if (Time.time >= nextAttackTime)
            {
                if (collision.gameObject.TryGetComponent(out IDamageable playerHealth)) 
                {
                    playerHealth.TakeDamage(damageToPlayer);
                    nextAttackTime = Time.time + attackInterval;

                    ApplyHorizontalKnockback(collision.transform.position);
                }
            }
        }
    }

    private void ApplyHorizontalKnockback(Vector3 playerPosition)
    {
        if (rb == null) return;

        // 1. Pausar la IA
        if (aiScript != null) aiScript.enabled = false;

        // 2. Dirección: Si el jugador está a la derecha (pos.x mayor), vamos a la izquierda (-1)
        float direction = transform.position.x < playerPosition.x ? -1f : 1f;

        // 3. Aplicar velocidad horizontal pura
        // Mantenemos rb.linearVelocity.y para que la gravedad siga funcionando si está cayendo
        rb.linearVelocity = new Vector2(direction * knockbackForce, rb.linearVelocity.y);

        // 4. Reactivar IA después de un breve tiempo (0.2s - 0.3s)
        CancelInvoke(nameof(ReactivateAI));
        Invoke(nameof(ReactivateAI), 0.25f); 
    }

    private void ReactivateAI()
    {
        if (aiScript != null) aiScript.enabled = true;
    }
}