using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    private enum EnemyState { Patrolling, Chasing }
    [SerializeField] private EnemyState currentState = EnemyState.Patrolling;

    [Header("Detection")]
    [SerializeField] private float detectionRange = 6f;
    [SerializeField] private float stopDistance = 0.8f;

    [Header("Patrol Settings")]
    [SerializeField] private Transform[] waypoints;
    [SerializeField] private float waitTimeAtPoint = 2f;
    private int currentWaypointIndex = 0;
    private float waitTimer;
    private bool isWaiting;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 2.5f;
    
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

        
        if (Mathf.Abs(distanceX) < 0.2f)
        {
            isWaiting = true;
            waitTimer = 0;
            StopMovement();
        }
        else
        {
            MoveTowards(target.position);
        }
    }

    private void ChaseLogic(float distance)
    {
        isWaiting = false; 
        if (distance > stopDistance)
            MoveTowards(player.position);
        else
            StopMovement();
    }

    private void MoveTowards(Vector2 target)
    {
        
        float diffX = target.x - transform.position.x;
    
        
        if (Mathf.Abs(diffX) < 0.1f) 
        {
            StopMovement();
            return;
        }

        float direction = Mathf.Sign(diffX);
    
        
        rb.linearVelocity = new Vector2(direction * moveSpeed, rb.linearVelocity.y);

        
        if (direction > 0) sprite.flipX = false;
        else if (direction < 0) sprite.flipX = true;

        if (anim != null) anim.SetBool("isWalking", true);
    }
    private void StopMovement()
    {
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        if (anim != null) anim.SetBool("isWalking", false);
    }

    private void OnDrawGizmos()
    {
       
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        
        if (waypoints != null)
        {
            Gizmos.color = Color.cyan;
            foreach (var wp in waypoints)
                if (wp != null) Gizmos.DrawSphere(wp.position, 0.2f);
        }
    }
    
    
    private void OnDisable()
    {
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            if (anim != null) anim.SetBool("isWalking", false);
        }
    }
}