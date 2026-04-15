using Enemys;
using UnityEngine;

public class DefenseEnemyAI : RangedEnemyAI // Aquí se establece la herencia
{
    [Header("Configuración de Defensa")]
    [SerializeField] protected float targetDetectionRange = 15f;
    protected Transform defenseTarget;

    protected override void Awake()
    {
        // Ejecuta el Awake del padre para obtener rb, anim, player, etc.
        base.Awake();

        // Buscamos el objetivo de defensa
        DefenseTarget target = Object.FindFirstObjectByType<DefenseTarget>();
        if (target != null) defenseTarget = target.transform;
    }

    // Sobreescribimos la forma en que el enemigo decide qué estado usar
    protected override void UpdateState(float distanceToPlayer)
    {
        // 1. Prioridad: Si el jugador está cerca, usamos la lógica de combate del padre
        if (distanceToPlayer <= detectionRange)
        {
            currentState = EnemyState.Chasing;
        }
        // 2. Si el jugador no está, pero el objetivo de defensa sí, "asediamos"
        else if (defenseTarget != null)
        {
            // Reutilizamos el estado Chasing pero apuntaremos al objetivo en la lógica
            currentState = EnemyState.Chasing; 
        }
        // 3. Si no hay nada, patrullamos
        else
        {
            currentState = EnemyState.Patrolling;
        }
    }

    // Sobreescribimos la ejecución para que, si no hay jugador, ataque a la estatua
    protected override void ExecuteStateLogic(float distanceToPlayer)
    {
        // Si el jugador está lejos pero el objetivo de defensa existe, atacamos la defensa
        if (distanceToPlayer > detectionRange && defenseTarget != null)
        {
            AttackDefenseTarget();
        }
        else
        {
            // En cualquier otro caso (está el player o patrulla), usamos lo del padre
            base.ExecuteStateLogic(distanceToPlayer);
        }
    }

    protected virtual void AttackDefenseTarget()
    {
        float distanceToObj = Vector2.Distance(transform.position, defenseTarget.position);
        float diffX = defenseTarget.position.x - transform.position.x;
        float directionToTarget = Mathf.Sign(diffX);

        if (distanceToObj > stopDistance)
        {
            Move(directionToTarget, moveSpeed);
        }
        else
        {
            StopMovement();
            Flip(directionToTarget);

            if (Time.time >= nextFireTime && !isAttacking)
            {
                ExecuteShoot();
                nextFireTime = Time.time + fireRate;
            }
        }
    }

    
    public virtual void PerformShoot()
    {
        // 1. Seguridad: Si el enemigo o el punto de disparo no existen (el error de antes)
        if (this == null || firePoint == null) return;

        // 2. Prioridad de objetivo: Si existe un objetivo de defensa, vamos a por él.
        // Si no, intentamos usar al jugador como respaldo.
        Transform currentTarget = (defenseTarget != null) ? defenseTarget : player;

        // 3. Si no hay absolutamente nadie, abortamos
        if (currentTarget == null) return;

        GameObject bullet = BulletPoolManager.Instance.GetBullet();

        if (bullet != null)
        {
            bullet.transform.position = firePoint.position;
            bullet.transform.rotation = firePoint.rotation;
            bullet.SetActive(true);

            if (bullet.TryGetComponent(out Rigidbody2D bRb))
            {
                // 4. DIRECCIÓN CORRECTA: Usamos el currentTarget (que será la base)
                Vector2 dir = ((Vector2)currentTarget.position - (Vector2)firePoint.position).normalized;
                bRb.linearVelocity = dir * 12f; 
            }
        }
    }
}