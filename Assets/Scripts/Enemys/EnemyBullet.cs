using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    [SerializeField] private float damage = 10f;
    [SerializeField] private float lifeTime = 5f;

    private void OnEnable()
    {
        // Si la bala no choca con nada en X segundos, vuelve al pool
        CancelInvoke();
        Invoke(nameof(Deactivate), lifeTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Buscamos tu sistema de IDamageable
        IDamageable damageable = collision.GetComponent<IDamageable>();
        
        if (damageable != null && collision.CompareTag("Player"))
        {
            damageable.TakeDamage(damage);
            Deactivate();
        }
        else if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            // Si choca con el suelo/paredes
            Deactivate();
        }
    }

    private void Deactivate()
    {
        gameObject.SetActive(false);
    }
}