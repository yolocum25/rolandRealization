using UnityEngine;
using System.Collections.Generic;


public class PlayerAttackSystem : MonoBehaviour 
{
    #region AnimParameters
    private static readonly int AttackTrigger = Animator.StringToHash("attack");
    #endregion

    [Header("Settings")]
    [SerializeField] private LayerMask whatIsDamageable;
    [SerializeField] private Transform attackPoint; 
    [SerializeField] private float baseAttackRadius = 0.5f;
    [SerializeField] private float baseDamage = 20f;

    [Header("Audio Settings")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip attackSound;
    [SerializeField] private AudioSource playerAudioSource; 
    
    private Animator anim;
    private bool attacking;
    private List<IDamageable> alreadyDamaged = new();

    void Awake()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (!enabled) return;

        if (Input.GetMouseButtonDown(0))
        {
            if (audioSource != null && attackSound != null)
            {
                playerAudioSource.PlayOneShot(attackSound);
            }
            anim.SetTrigger(AttackTrigger);
            
            attacking = true; 
            Invoke("CloseAttackWindow", 0.5f); // Lo cerramos a los 0.5 segundos
        }

        if (attacking)
        {
            CheckForDamage();
        }
    }

    private void HandleAttackInput()
    {
        anim.SetTrigger(AttackTrigger);
        
    }

    private void CheckForDamage()
    {
        
        if (attackPoint == null) return;
        float posPerc = EmotionManager.Instance.positiveBar.FillPercentage;
        float negPerc = EmotionManager.Instance.negativeBar.FillPercentage;
        
        float currentDamage = baseDamage + (baseDamage * negPerc);
        float baseScale = baseAttackRadius;
        float boxSizeXY = baseScale * 2f;
        baseScale = baseAttackRadius + (baseAttackRadius * posPerc * 0.5f);
        Vector2 attackBoxSize = new Vector2(boxSizeXY, boxSizeXY);
        
        Collider2D[] hitEnemies = Physics2D.OverlapBoxAll(attackPoint.position,attackBoxSize,0f, whatIsDamageable);

        foreach (Collider2D enemy in hitEnemies)
        {
            
            if (enemy.TryGetComponent(out IDamageable damageable) && !alreadyDamaged.Contains(damageable))
            {
                damageable.TakeDamage(currentDamage);
                alreadyDamaged.Add(damageable);
            }
        }
    }

  

    public void OpenAttackWindow()
    {
        attacking = true;
    }

    public void CloseAttackWindow()
    {
        attacking = false;
        alreadyDamaged.Clear(); 
    }

    
    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.cyan;

       
        float visualRadius = baseAttackRadius;
        
        float baseScale = baseAttackRadius;
        
        if (Application.isPlaying && EmotionManager.Instance != null)
        {
            float posPerc = EmotionManager.Instance.positiveBar.FillPercentage;
            visualRadius = baseAttackRadius + (baseAttackRadius * posPerc * 0.5f);
            baseScale = baseAttackRadius + (baseAttackRadius * posPerc * 0.5f);
        }
        float cubeSizeXY = baseScale * 2f; 
        Vector3 cubeSize = new Vector3(cubeSizeXY, cubeSizeXY, 1f);
        
        Gizmos.DrawWireCube(attackPoint.position, cubeSize);
    }
}