using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    [SerializeField] private float damage = 10f;
    [SerializeField] private float lifeTime = 5f;
    [SerializeField] private GameObject impactEffect;

    private void OnEnable()
    {
        // Programamos la desactivación por si no choca con nada
        Invoke(nameof(DesactivateBullet), lifeTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 1. Intentamos obtener la interfaz IDamageable del objeto con el que chocamos
        // Buscamos tanto en el objeto como en sus padres (por si el collider está en un hijo)
        IDamageable damageable = collision.GetComponentInParent<IDamageable>();

        if (damageable != null)
        {
            // 2. Aplicamos el daño a través de la interfaz
            damageable.TakeDamage(damage);
            
            // Lógica visual de impacto
            ImpactLogic();
        }
        else if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            // 3. Si choca con el suelo/paredes también se destruye
            ImpactLogic();
        }
    }

    private void ImpactLogic()
    {
        if (impactEffect != null)
        {
            Instantiate(impactEffect, transform.position, Quaternion.identity);
        }

        // Si usas Pooling, desactivamos. Si no, Destroy(gameObject).
        gameObject.SetActive(false);
    }

    private void DesactivateBullet()
    {
        gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        CancelInvoke();
    }
}