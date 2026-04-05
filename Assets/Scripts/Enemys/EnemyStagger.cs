using UnityEngine;
using System.Collections;

public class EnemyStaggerResponse : MonoBehaviour
{
    [SerializeField] private EnemyHealthSystem healthSystem;
    [SerializeField] private MonoBehaviour enemyAI; 
    [SerializeField] private Animator anim;
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
        // 1. Detener movimiento y poner animación
        if (enemyAI != null) enemyAI.enabled = false;
        if (anim != null) anim.SetTrigger("Stagger");

        

        yield return new WaitForSeconds(stunDuration);

        
        if (enemyAI != null) enemyAI.enabled = true;
        
        
    }
}