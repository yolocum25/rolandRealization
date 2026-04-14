using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class RangedEnemyAI : MonoBehaviour
{
    private enum EnemyState { Patrolling, Chasing }
    [SerializeField] private EnemyState currentState = EnemyState.Patrolling;

    [Header("Zonas de Distancia")]
    [SerializeField] private float detectionRange = 10f; 
    [SerializeField] private float stopDistance = 7f;      
    [SerializeField] private float dangerZone = 4f;       

    [Header("Patrulla")]
    [SerializeField] private Transform[] waypoints;
    [SerializeField] private float waitTimeAtPoint = 2f;
    private int currentWaypointIndex = 0;
    private float waitTimer;
    private bool isWaiting;

    [Header("Movimiento")]
    [SerializeField] private float moveSpeed = 2.5f;
    
    [Header("Ataque (Pooling)")]
    [SerializeField] private Transform firePoint;
    [SerializeField] private float fireRate = 2f;
    private float nextFireTime;
    private bool isAttacking; // Para bloquear movimiento durante la animación

    private Transform player;
    private Rigidbody2D rb;
    private SpriteRenderer sprite;
    private Animator anim;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        
        // --- 1. GESTIÓN DE ESTADOS ---
        if (distanceToPlayer <= detectionRange)
            currentState = EnemyState.Chasing;
        else
            currentState = EnemyState.Patrolling;

        // --- 2. EJECUCIÓN DE LÓGICA ---
        switch (currentState)
        {
            case EnemyState.Patrolling:
                PatrolLogic();
                break;
            case EnemyState.Chasing:
                CombatDistancingLogic(distanceToPlayer);
                break;
        }
    }

    private void PatrolLogic()
    {
        if (waypoints == null || waypoints.Length == 0) return;

        isAttacking = false; // Por si acaso
        Transform target = waypoints[currentWaypointIndex];
        float diffX = target.position.x - transform.position.x;

        if (isWaiting)
        {
            StopMovement();
            waitTimer += Time.deltaTime;
            if (waitTimer >= waitTimeAtPoint)
            {
                isWaiting = false;
                currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
            }
            return;
        }

        // Si llegamos al punto (umbral de 0.5f)
        if (Mathf.Abs(diffX) < 0.5f)
        {
            isWaiting = true;
            waitTimer = 0;
            StopMovement();
        }
        else
        {
            Move(Mathf.Sign(diffX), moveSpeed);
        }
    }

    private void CombatDistancingLogic(float distance)
    {
        isWaiting = false;
        float diffX = player.position.x - transform.position.x;
        float directionToPlayer = Mathf.Sign(diffX);

        if (distance < dangerZone)
        {
            // RETROCEDER: Se mueve al lado contrario, pero mira al player
            Move(-directionToPlayer, moveSpeed * 1.2f);
            Flip(directionToPlayer); 
        }
        else if (distance > stopDistance)
        {
            // PERSEGUIR
            Move(directionToPlayer, moveSpeed);
        }
        else
        {
            // SAFE ZONE: Disparar
            StopMovement();
            Flip(directionToPlayer);

            if (Time.time >= nextFireTime && !isAttacking)
            {
                ExecuteShoot();
                nextFireTime = Time.time + fireRate;
            }
        }
    }

    private void ExecuteShoot()
    {
        isAttacking = true;
        if (anim != null) anim.SetTrigger("isShooting");
        // Nota: isAttacking debe volver a false mediante un Animation Event que llame a ResetAttack()
        // O podrías usar un Invoke para desbloquearlo tras X tiempo:
        Invoke(nameof(ResetAttack), 1f); 
    }

    // FUNCIÓN PARA EL EVENTO DE ANIMACIÓN
    public void PerformShoot()
    {
        // Accedemos al Singleton del Manager
        GameObject bullet = BulletPoolManager.Instance.GetBullet();
    
        if (bullet != null)
        {
            bullet.transform.position = firePoint.position;
            bullet.transform.rotation = firePoint.rotation;
            bullet.SetActive(true); // Al activarse, se dispara el OnEnable de la bala
        
            if (bullet.TryGetComponent(out Rigidbody2D bRb))
            {
                // Disparar hacia el jugador
                Vector2 dir = (player.position - firePoint.position).normalized;
                bRb.linearVelocity = dir * 12f; 
            }
        }
    }

    public void ResetAttack() => isAttacking = false;

    private void Move(float dirX, float speed)
    {
        if (isAttacking) return;
        rb.linearVelocity = new Vector2(dirX * speed, rb.linearVelocity.y);
        Flip(dirX);
        if (anim != null) anim.SetBool("isWalking", true);
    }

    private void StopMovement()
    {
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        if (anim != null) anim.SetBool("isWalking", false);
    }

    private void Flip(float dirX)
    {
        if (dirX > 0) 
            sprite.flipX = true; 
        else if (dirX < 0) 
            sprite.flipX = false; 
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green; Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.yellow; Gizmos.DrawWireSphere(transform.position, stopDistance);
        Gizmos.color = Color.red; Gizmos.DrawWireSphere(transform.position, dangerZone);
    }
}