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
        // 1. Buscamos si el objeto tiene la interfaz de daño
        IDamageable damageable = collision.GetComponent<IDamageable>();
        
        if (damageable != null)
        {
          
            if (collision.CompareTag("Player") || collision.CompareTag("Defense"))
            {
                damageable.TakeDamage(damage);
                Deactivate();
                return; // Salimos para evitar procesar más colisiones
            }
        }

        // 3. Si choca con el escenario (suelo/paredes)
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