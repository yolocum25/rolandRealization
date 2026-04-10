using UnityEngine;
using System.Collections;

public class EnemyStaggerResponse : MonoBehaviour
{
    [SerializeField] private EnemyHealthSystem healthSystem;
    [SerializeField] private MonoBehaviour enemyAI; 
    [SerializeField] private Animator anim;
    [SerializeField] private EnemyTouchDamage touchDamage;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float stunDuration = 3f;

    private void OnEnable()
    {
        healthSystem.OnStagger += HandleStagger;
    }

    private void OnDisable()
    {
        healthSystem.OnStagger -= HandleStagger;
    }

    private void HandleStagger()
    {
        StartCoroutine(StaggerRoutine());
    }

    private IEnumerator StaggerRoutine()
    {
        // 1. Apagamos la IA (esto ejecutará el OnDisable que pusimos arriba)
        if (enemyAI != null) enemyAI.enabled = false;
        if (touchDamage != null) touchDamage.enabled = false;

        if (rb != null)
        {
            // 2. CONGELAMOS EL RIGIDBODY COMPLETAMENTE
            rb.linearVelocity = Vector2.zero;
            rb.constraints = RigidbodyConstraints2D.FreezeAll; // Bloquea X, Y y Rotación
        }
    
        if (anim != null) anim.SetBool("Stagger", true);
        yield return null; 

        yield return new WaitForSeconds(stunDuration);
   
        if (anim != null) anim.SetBool("Stagger", false);

        // 3. DEVOLVEMOS LA LIBERTAD (Solo congelamos rotación Z)
        if (rb != null)
        {
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        }

        yield return new WaitForSeconds(0.1f);

        if (enemyAI != null) enemyAI.enabled = true;
        if (touchDamage != null) touchDamage.enabled = true;
    }
}