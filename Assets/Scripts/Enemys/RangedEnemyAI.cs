using UnityEngine;

namespace Enemys
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class RangedEnemyAI : MonoBehaviour
    {
        protected enum EnemyState { Patrolling, Chasing }
        [SerializeField] protected EnemyState currentState = EnemyState.Patrolling;

        [Header("Distance")]
        [SerializeField] protected float detectionRange = 10f; 
        [SerializeField] protected float stopDistance = 7f;      
        [SerializeField] protected float dangerZone = 4f;       

        [Header("Patroll")]
        [SerializeField] protected Transform[] waypoints;
        [SerializeField] protected float waitTimeAtPoint = 2f;
        protected int currentWaypointIndex = 0;
        protected float waitTimer;
        protected bool isWaiting;

        [Header("Movement")]
        [SerializeField] protected float moveSpeed = 2.5f;
    
        [Header("Attack")]
        [SerializeField] protected Transform firePoint;
        [SerializeField] protected float fireRate = 2f;
        protected float nextFireTime;
        protected bool isAttacking; 

        protected Transform player;
        protected Rigidbody2D rb;
        protected SpriteRenderer sprite;
        protected Animator anim;

      
        protected virtual void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            sprite = GetComponent<SpriteRenderer>();
            anim = GetComponent<Animator>();
            player = GameObject.FindGameObjectWithTag("Player")?.transform;
        }

       
        protected virtual void Update()
        {
            if (player == null) return;

            float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        
            UpdateState(distanceToPlayer);
            ExecuteStateLogic(distanceToPlayer);
        }

      
        protected virtual void UpdateState(float distanceToPlayer)
        {
            if (distanceToPlayer <= detectionRange)
                currentState = EnemyState.Chasing;
            else
                currentState = EnemyState.Patrolling;
        }

        
        protected virtual void ExecuteStateLogic(float distanceToPlayer)
        {
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

        protected virtual void PatrolLogic()
        {
            if (waypoints == null || waypoints.Length == 0) return;

            isAttacking = false; 
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

        protected virtual void CombatDistancingLogic(float distance)
        {
            isWaiting = false;
            float diffX = player.position.x - transform.position.x;
            float directionToPlayer = Mathf.Sign(diffX);

            if (distance < dangerZone)
            {
                Move(-directionToPlayer, moveSpeed * 1.2f);
                Flip(directionToPlayer); 
            }
            else if (distance > stopDistance)
            {
                Move(directionToPlayer, moveSpeed);
            }
            else
            {
                StopMovement();
                Flip(directionToPlayer);

                if (Time.time >= nextFireTime && !isAttacking)
                {
                    ExecuteShoot();
                    nextFireTime = Time.time + fireRate;
                }
            }
        }

        protected virtual void ExecuteShoot()
        {
            isAttacking = true;
            if (anim != null) anim.SetTrigger("isShooting");
            Invoke(nameof(ResetAttack), 1f); 
        }

        
        public virtual void PerformShoot()
        {
            
            if (this == null || firePoint == null) return;

           
            if (player == null) return;

            GameObject bullet = BulletPoolManager.Instance.GetBullet();

            if (bullet != null)
            {
                bullet.transform.position = firePoint.position;
                bullet.transform.rotation = firePoint.rotation;
                bullet.SetActive(true);

                if (bullet.TryGetComponent(out Rigidbody2D bRb))
                {
                    
                    Vector2 dir = (player.position - firePoint.position).normalized;
                    bRb.linearVelocity = dir * 12f; 
                }
            }
        }

        public void ResetAttack() => isAttacking = false;

        protected void Move(float dirX, float speed)
        {
            if (isAttacking) return;
            rb.linearVelocity = new Vector2(dirX * speed, rb.linearVelocity.y);
            Flip(dirX);
            if (anim != null) anim.SetBool("isWalking", true);
        }

        protected void StopMovement()
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            if (anim != null) anim.SetBool("isWalking", false);
        }

        protected void Flip(float dirX)
        {
            if (dirX > 0) 
                sprite.flipX = true; 
            else if (dirX < 0) 
                sprite.flipX = false; 
        }

        protected virtual void OnDrawGizmos()
        {
            Gizmos.color = Color.green; Gizmos.DrawWireSphere(transform.position, detectionRange);
            Gizmos.color = Color.yellow; Gizmos.DrawWireSphere(transform.position, stopDistance);
            Gizmos.color = Color.red; Gizmos.DrawWireSphere(transform.position, dangerZone);
        }
    }
}