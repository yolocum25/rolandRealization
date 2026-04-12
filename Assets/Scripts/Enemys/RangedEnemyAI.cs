using UnityEngine;

[RequireComponent(typeof(Animator))] // Nos aseguramos de que haya un Animator
public class RangedEnemyAI : MonoBehaviour
{
    [Header("Rangos de Distancia")]
    [SerializeField] private float viewRange = 10f;
    [SerializeField] private float safeDistance = 5f;
    [SerializeField] private float stopDistance = 7f;

    [Header("Patrulla")]
    [SerializeField] private Transform[] patrolPoints;
    [SerializeField] private float patrolSpeed = 2f;
    private int currentPatrolIndex;

    [Header("Combate")]
    [SerializeField] private float runSpeed = 4f;
    [SerializeField] private float fireRate = 1.5f; // Tiempo entre disparos
    [SerializeField] private Transform firePoint;
    private float nextFireTime;

    private Transform player;
    private Rigidbody2D rb;
    private Animator animator; // Referencia al Animator
    private bool playerInSight;
    private bool isAttacking; // Nuevo: Para no movernos mientras disparamos

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>(); // Obtenemos el Animator
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Update()
    {
        if (player == null) return;

        // Si estamos en medio de la animación de ataque, no hacemos nada más
        if (isAttacking) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        playerInSight = distanceToPlayer < viewRange;

        if (playerInSight)
        {
            HandleCombat(distanceToPlayer);
        }
        else
        {
            Patrol();
        }

        // Actualizamos animaciones de movimiento (si tienes)
        UpdateMovementAnimation();
    }

    private void HandleCombat(float distance)
    {
        LookAtTarget(player.position);

        if (distance < safeDistance)
        {
            // HUIR
            Vector2 runDirection = (transform.position - player.position).normalized;
            rb.linearVelocity = runDirection * runSpeed;
        }
        else if (distance > stopDistance)
        {
            // ACERCARSE
            Vector2 approachDirection = (player.position - transform.position).normalized;
            rb.linearVelocity = approachDirection * (runSpeed * 0.7f);
        }
        else
        {
            // MANTENER DISTANCIA (Rango ideal)
            rb.linearVelocity = Vector2.zero;
            
            // LÓGICA DE DISPARO ACTIVA
            if (Time.time >= nextFireTime)
            {
                StartShootSequence(); // 1. Iniciamos la secuencia visual
                nextFireTime = Time.time + fireRate;
            }
        }
    }

    private void Patrol()
    {
        if (patrolPoints.Length == 0) return;

        Transform target = patrolPoints[currentPatrolIndex];
        Vector2 direction = (target.position - transform.position).normalized;
        rb.linearVelocity = direction * patrolSpeed;

        LookAtTarget(target.position);

        if (Vector2.Distance(transform.position, target.position) < 0.5f)
        {
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
        }
    }

    // --- PARTE 1 DEL DISPARO: LA ANIMACIÓN ---
    private void StartShootSequence()
    {
        isAttacking = true; // Bloqueamos el movimiento
        rb.linearVelocity = Vector2.zero; // Nos paramos en seco
        
        // Activamos el Trigger en el Animator
        if (animator != null)
        {
            animator.SetTrigger("isShooting");
        }
    }

    // --- PARTE 2 DEL DISPARO: LA BALA (Llamada por el Animation Event) ---
    // IMPORTANTE: Este nombre debe coincidir con el del Animation Event
    public void PerformShoot()
    {
        if (player == null) return;

        GameObject bullet = BulletPoolManager.Instance.GetBullet();
        if (bullet != null)
        {
            bullet.transform.position = firePoint.position;
            bullet.transform.rotation = firePoint.rotation;
            bullet.SetActive(true);
            
            if (bullet.TryGetComponent(out Rigidbody2D bulletRb))
            {
                Vector2 shootDir = (player.position - firePoint.position).normalized;
                bulletRb.linearVelocity = shootDir * 12f; // Velocidad de la bala
            }
        }
        
        // La animación ha terminado su fase crítica, permitimos movimiento de nuevo
        // (Dependiendo de tu animación, quizás quieras llamar a esto en otro evento al FINAL del clip)
        isAttacking = false; 
    }

    private void LookAtTarget(Vector3 target)
    {
        if (target.x > transform.position.x)
            transform.localScale = new Vector3(1, 1, 1);
        else
            transform.localScale = new Vector3(-1, 1, 1);
    }

    private void UpdateMovementAnimation()
    {
        if (animator != null)
        {
            // Si tienes una animación de "Caminar" (Walk) controlada por un float "Speed"
            animator.SetFloat("Speed", rb.linearVelocity.magnitude);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, safeDistance);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, stopDistance);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, viewRange);
    }
}