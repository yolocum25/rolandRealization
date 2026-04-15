using Enemys;
using UnityEngine;

public class DefenseEnemyAI : RangedEnemyAI 
{
    [Header("Configuración de Defensa")]
    [SerializeField] protected float targetDetectionRange = 15f;
    protected Transform defenseTarget;

    protected override void Awake()
    {
        
        base.Awake();

       
        DefenseTarget target = Object.FindFirstObjectByType<DefenseTarget>();
        if (target != null) defenseTarget = target.transform;
    }

    
    protected override void UpdateState(float distanceToPlayer)
    {
        
        if (distanceToPlayer <= detectionRange)
        {
            currentState = EnemyState.Chasing;
        }
      
        else if (defenseTarget != null)
        {
            
            currentState = EnemyState.Chasing; 
        }
       
        else
        {
            currentState = EnemyState.Patrolling;
        }
    }

    
    protected override void ExecuteStateLogic(float distanceToPlayer)
    {
        
        if (distanceToPlayer > detectionRange && defenseTarget != null)
        {
            AttackDefenseTarget();
        }
        else
        {
           
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
        
        if (this == null || firePoint == null) return;

        
        Transform currentTarget = (defenseTarget != null) ? defenseTarget : player;

        
        if (currentTarget == null) return;

        GameObject bullet = BulletPoolManager.Instance.GetBullet();

        if (bullet != null)
        {
            bullet.transform.position = firePoint.position;
            bullet.transform.rotation = firePoint.rotation;
            bullet.SetActive(true);

            if (bullet.TryGetComponent(out Rigidbody2D bRb))
            {
                
                Vector2 dir = ((Vector2)currentTarget.position - (Vector2)firePoint.position).normalized;
                bRb.linearVelocity = dir * 12f; 
            }
        }
    }
}