using UnityEngine;
using System.Collections.Generic;


public class PlayerAttackSystem : MonoBehaviour // O PlayerSystem si heredas de ahí
{
    #region AnimParameters
    // Usar Hash es más eficiente que usar Strings cada frame
    private static readonly int AttackTrigger = Animator.StringToHash("attack");
    #endregion

    [Header("Settings")]
    [SerializeField] private LayerMask whatIsDamageable;
    [SerializeField] private Transform attackPoint; 
    [SerializeField] private float baseAttackRadius = 0.5f;
    [SerializeField] private float baseDamage = 20f;

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
            anim.SetTrigger(AttackTrigger);
            // Forzamos el ataque manualmente sin esperar a la animación
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
        // El daño no se activa aquí, sino mediante el Animation Event "OpenAttackWindow"
    }

    private void CheckForDamage()
    {
        
        if (attackPoint == null) return;
        float posPerc = EmotionManager.Instance.positiveBar.FillPercentage;
        float negPerc = EmotionManager.Instance.negativeBar.FillPercentage;
        
        float currentDamage = baseDamage + (baseDamage * negPerc);
        float currentRadius = baseAttackRadius + (baseAttackRadius * posPerc * 0.5f);
        
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, currentRadius, whatIsDamageable);

        foreach (Collider2D enemy in hitEnemies)
        {
            // Intentamos obtener el componente de daño
            if (enemy.TryGetComponent(out IDamageable damageable) && !alreadyDamaged.Contains(damageable))
            {
                damageable.TakeDamage(currentDamage);
                alreadyDamaged.Add(damageable);
            }
        }
    }

    // --- EVENTOS DE ANIMACIÓN (Se llaman desde la línea de tiempo) ---

    public void OpenAttackWindow()
    {
        attacking = true;
    }

    public void CloseAttackWindow()
    {
        attacking = false;
        alreadyDamaged.Clear(); // Limpiamos la lista para el próximo tajo
    }

    // Para ver el rango del ataque en el editor
    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.red;

       
        float visualRadius = baseAttackRadius;
        
       
        if (Application.isPlaying && EmotionManager.Instance != null)
        {
            float posPerc = EmotionManager.Instance.positiveBar.FillPercentage;
            visualRadius = baseAttackRadius + (baseAttackRadius * posPerc * 0.5f);
        }

        Gizmos.DrawWireSphere(attackPoint.position, visualRadius);
    }
}