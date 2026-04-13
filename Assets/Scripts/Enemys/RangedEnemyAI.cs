using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(EnemyHealthSystem))]
public class RangedEnemyAI : MonoBehaviour
{
    private enum EnemyState { Patrolling, Chasing, Dead }
    [SerializeField] private EnemyState currentState = EnemyState.Patrolling;

    [Header("Detection & Combat Ranges")]
    [SerializeField] private float detectionRange = 10f; 
    [SerializeField] private float stopDistance = 7f;      
    [SerializeField] private float safeDistance = 4f;      

    [Header("Patrol Settings")]
    [SerializeField] private Transform[] waypoints;
    [SerializeField] private float waitTimeAtPoint = 2f;
    private int currentWaypointIndex = 0;
    private float waitTimer;
    private bool isWaiting;

    [Header("Movement & Combat")]
    [SerializeField] private float moveSpeed = 2.5f;
    [SerializeField] private float fireRate = 2f;
    [SerializeField] private Transform firePoint;
    private float nextFireTime;
    private bool isAttacking;

    private Transform player;
    private Rigidbody2D rb;
    private SpriteRenderer sprite;
    private Animator anim;
    private EnemyHealthSystem healthSystem; // Referencia a tu script de vida

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        healthSystem = GetComponent<EnemyHealthSystem>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    void Update()
    {
        // Si tu sistema de vida base tiene una variable para saber si está muerto, úsala aquí
        // Por ejemplo: if (healthSystem.currentHealth <= 0)
        if (player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        
        // Cambiamos de estado según distancia
        if (distanceToPlayer <= detectionRange)
            currentState = EnemyState.Chasing;
        else
            currentState = EnemyState.Patrolling;

        switch (currentState)
        {
            case EnemyState.Patrolling:
                PatrolLogic();
                break;
            case EnemyState.Chasing:
                ChaseLogic(distanceToPlayer);
                break;
        }
    }

    private void PatrolLogic()
    {
        if (waypoints == null || waypoints.Length == 0) return;

        Transform target = waypoints[currentWaypointIndex];
        float distanceX = target.position.x - transform.position.x;

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

        if (Mathf.Abs(distanceX) < 0.3f)
        {
            isWaiting = true;
            waitTimer = 0;
            StopMovement();
        }
        else
        {
            MoveTowards(target.position, moveSpeed);
        }
    }

    private void ChaseLogic(float distance)
    {
        isWaiting = false;
        LookAtTarget(player.position);

        if (distance < safeDistance)
        {
            // HUIR
            Vector2 fleeTarget = transform.position + (transform.position - player.position);
            MoveTowards(fleeTarget, moveSpeed * 1.2f);
        }
        else if (distance > stopDistance)
        {
            // ACERCARSE
            MoveTowards(player.position, moveSpeed);
        }
        else
        {
            // DISPARAR
            StopMovement();
            if (Time.time >= nextFireTime && !isAttacking)
            {
                StartShootSequence();
                nextFireTime = Time.time + fireRate;
            }
        }
    }

    private void MoveTowards(Vector2 target, float speed)
    {
        if (isAttacking) return;
        float diffX = target.x - transform.position.x;
        float direction = Mathf.Sign(diffX);
        rb.linearVelocity = new Vector2(direction * speed, rb.linearVelocity.y);
        sprite.flipX = (direction < 0);
        if (anim != null) anim.SetBool("isWalking", true);
    }

    private void StopMovement()
    {
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        if (anim != null) anim.SetBool("isWalking", false);
    }

    private void LookAtTarget(Vector2 target)
    {
        float diffX = target.x - transform.position.x;
        sprite.flipX = (diffX < 0);
    }

    private void StartShootSequence()
    {
        isAttacking = true;
        StopMovement();
        if (anim != null) anim.SetTrigger("isShooting");
        Invoke(nameof(ResetAttack), 1.5f);
    }

    public void PerformShoot() // Evento de Animación
    {
        CancelInvoke(nameof(ResetAttack));
        GameObject bullet = BulletPoolManager.Instance.GetBullet();
        if (bullet != null)
        {
            bullet.transform.position = firePoint.position;
            bullet.SetActive(true);
            if (bullet.TryGetComponent(out Rigidbody2D bulletRb))
            {
                Vector2 shootDir = (player.position - firePoint.position).normalized;
                bulletRb.linearVelocity = shootDir * 12f;
            }
        }
        ResetAttack();
    }

    private void ResetAttack() => isAttacking = false;

    private void OnDisable()
    {
        StopMovement();
    }
}