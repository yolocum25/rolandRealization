using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    [SerializeField] private float damage = 10f;
    [SerializeField] private float lifeTime = 5f;

    private void OnEnable()
    {
        CancelInvoke();
        Invoke(nameof(Deactivate), lifeTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        
        IDamageable damageable = collision.GetComponent<IDamageable>();
        
        if (damageable != null)
        {
          
            if (collision.CompareTag("Player") || collision.CompareTag("Defense"))
            {
                damageable.TakeDamage(damage);
                Deactivate();
                return; 
            }
        }

        
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            Deactivate();
        }
    }

    private void Deactivate()
    {
        gameObject.SetActive(false);
    }
}