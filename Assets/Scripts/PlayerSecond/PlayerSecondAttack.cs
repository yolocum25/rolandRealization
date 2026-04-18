using UnityEngine;
using System.Collections.Generic;

public class PlayerSecondAttack : MonoBehaviour 
{
    [Header("Configuración de Ataque")]
    [SerializeField] private LayerMask whatIsDamageable;
    [SerializeField] private Transform attackPoint; 
    [SerializeField] private float baseAttackRadius = 0.5f;
    [SerializeField] private float baseDamage = 20f;

    [Header("Sistema de Cooldown")]
    [SerializeField] private float attackCooldown = 2f; 
    private float nextAttackTime = 0f;

    [Header("Audio")]
    [SerializeField] private AudioSource playerAudioSource; 
    [SerializeField] private AudioClip attackSound;
    
    private Animator anim;
    private bool attacking; // Esta variable controla la lógica de daño
    private List<IDamageable> alreadyDamaged = new List<IDamageable>();

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        if (!enabled) return;

        // Atacamos si pulsamos click, ha pasado el cooldown y no estamos atacando ya
        if (Input.GetMouseButtonDown(0) && Time.time >= nextAttackTime && !attacking)
        {
            PerformAttack();
        }

        if (attacking)
        {
            CheckForDamage();
        }
    }

    private void PerformAttack()
    {
        nextAttackTime = Time.time + attackCooldown;

        if (playerAudioSource != null && attackSound != null)
            playerAudioSource.PlayOneShot(attackSound);

        // 1. Abrimos la ventana (Lógica y Animación)
        OpenAttackWindow1();
        
        // 2. Programamos el cierre a los 0.5 segundos
        CancelInvoke(nameof(CloseAttackWindow1)); 
        Invoke(nameof(CloseAttackWindow1), 0.5f); 
    }

    // --- MÉTODOS DE CONTROL DE VENTANA ---

    public void OpenAttackWindow1()
    {
        attacking = true;
        alreadyDamaged.Clear(); // Limpiamos la lista al empezar un ataque nuevo

        // Si tu Animator usa un Bool llamado "attacking"
        if (anim != null) anim.SetBool("attacking", true);
        
        // Si tu Animator usa un Trigger llamado "attack" (Descomenta la línea de abajo si es así)
        // if (anim != null) anim.SetTrigger("attack");
    }

    public void CloseAttackWindow1()
    {
        attacking = false;
        alreadyDamaged.Clear(); 

        // IMPORTANTE: Ponemos el bool en false para que la animación termine
        if (anim != null) anim.SetBool("attacking", false);
    }

    // ---------------------------------------

    private void CheckForDamage()
    {
        if (attackPoint == null) return;

        float sorrowFactor = SorrownessManager.Instance.GetSorrowLevel();
        float currentDamage = baseDamage + (baseDamage * sorrowFactor);
        
        float boxSizeXY = baseAttackRadius * 2f;
        Vector2 attackBoxSize = new Vector2(boxSizeXY, boxSizeXY);
        
        Collider2D[] hitEnemies = Physics2D.OverlapBoxAll(attackPoint.position, attackBoxSize, 0f, whatIsDamageable);

        foreach (Collider2D enemy in hitEnemies)
        {
            if (enemy.TryGetComponent(out IDamageable damageable) && !alreadyDamaged.Contains(damageable))
            {
                damageable.TakeDamage(currentDamage);
                alreadyDamaged.Add(damageable);

                // --- AÑADE ESTA LÍNEA ---
                if(SorrownessManager.Instance != null) 
                    SorrownessManager.Instance.PlayerDealtDamage();
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.yellow;
        float size = baseAttackRadius * 2f;
        Gizmos.DrawWireCube(attackPoint.position, new Vector3(size, size, 1f));
    }
}