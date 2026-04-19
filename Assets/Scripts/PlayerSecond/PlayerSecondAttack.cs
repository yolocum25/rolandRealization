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
    private bool attacking; 
    private List<IDamageable> alreadyDamaged = new List<IDamageable>();

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        if (!enabled) return;

        
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

        
        OpenAttackWindow1();
        

        CancelInvoke(nameof(CloseAttackWindow1)); 
        Invoke(nameof(CloseAttackWindow1), 0.5f); 
    }

   

    public void OpenAttackWindow1()
    {
        attacking = true;
        alreadyDamaged.Clear(); 

       
        if (anim != null) anim.SetBool("attacking", true);
        
       
    }

    public void CloseAttackWindow1()
    {
        attacking = false;
        alreadyDamaged.Clear(); 

       
        if (anim != null) anim.SetBool("attacking", false);
    }

   

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