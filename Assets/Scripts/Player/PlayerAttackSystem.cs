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
    [SerializeField] private Transform attackPoint; // El punto desde donde sale el círculo de daño
    [SerializeField] private float attackRadius = 0.5f;
    [SerializeField] private float damage = 20f;

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

           
            if (Input.GetMouseButtonDown(0) && !attacking)
            {
                HandleAttackInput();
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
        // Asegúrate de que attackPoint no sea null para evitar el error "NullReference"
        if (attackPoint == null) return;

        // Esta línea detecta a todos los enemigos en el círculo
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRadius, whatIsDamageable);

        foreach (Collider2D enemy in hitEnemies)
        {
            // Intentamos obtener el componente de daño
            if (enemy.TryGetComponent(out IDamageable damageable) && !alreadyDamaged.Contains(damageable))
            {
                damageable.TakeDamage(damage);
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
        Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
    }
}