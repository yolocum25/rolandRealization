using UnityEngine;

public class EnemyTouchDamage : MonoBehaviour
{
    [SerializeField] private float damageToPlayer = 15f;
    [SerializeField] private float attackInterval = 1.0f;
    
    [Header("Audio Settings")]
     [SerializeField] private AudioSource audioSource;
    
    [Header("Knockback Settings")]
    [SerializeField] private float knockbackForce = 8f; 
    [SerializeField] private EnemyAI aiScript;         

    
    
    private float nextAttackTime;
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!this.enabled) return; 

        if (collision.gameObject.CompareTag("Player"))
        {
            if (audioSource != null && !audioSource.isPlaying)
            {
                audioSource.Play();
            }

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

        
        if (aiScript != null) aiScript.enabled = false;

       
        float direction = transform.position.x < playerPosition.x ? -1f : 1f;

        
        rb.linearVelocity = new Vector2(direction * knockbackForce, rb.linearVelocity.y);

        
        CancelInvoke(nameof(ReactivateAI));
        Invoke(nameof(ReactivateAI), 0.25f); 
    }

    private void ReactivateAI()
    {
        if (aiScript != null) aiScript.enabled = true;
    }
}