
        using System.Collections;
    using System.Collections.Generic;
    using Player;
    using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

[RequireComponent(typeof(Rigidbody2D))]

public class PlayerSecond : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float movementSpeed = 8f;
    [SerializeField] private float jumpHeight = 3f;

    [Header("Ground Detection")]
    [SerializeField] private Transform feet;
    [SerializeField] private float detectionRadius = 0.2f;
    [SerializeField] private LayerMask whatIsGround;
    
    [Header("Dash Settings")]
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private float dashCooldown = 4f;
    [SerializeField] private float dashDistance = 8f;
    private bool canDash = true;
    private bool isDashing;
    private bool isPaused;
    
    [Header("Attack - Shaft")]
    [SerializeField] private float shaftDamage = 50f;
    [SerializeField] private float shaftRadius = 4f;
    
    [Header("SlashDash Settings")]
    [SerializeField] private LayerMask whatIsDamageable;
    [SerializeField] private Transform attackPointDash; 
    [SerializeField] private float baseAttackRadiusDash = 4;
    [SerializeField] private float baseDamageDash = 60f;
    private bool canSlashDash = true;
    private bool isSlashDashing;
    private bool SlashDash;
    private bool clickDuringDash = false;
    private PlayerHealthSystem health;
    
    [Header("Audio SlashDash")]
    [SerializeField] private AudioSource playerAudioSource; 
    [SerializeField] private AudioClip slashDashSound;
    
    [Header("Furioso Settings")]
    [SerializeField] private float furiosoSearchRadius = 10f;
    [SerializeField] private float damagePerHit = 15f;
    [SerializeField] private float timeBetweenHits = 0.15f; 
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private AudioClip furiosoSound;
    
    
    
    
    private Rigidbody2D rb;
    private Animator anim;
    private bool isGrounded;
    private Vector2 inputVector;
    private Vector3 initialScale;
    private List<IDamageable> alreadyDamaged = new();
    private float originalGravity; 
    private bool isExecutingFurioso = false;
    private Vector3 positionBeforeFurioso;
    private SpriteRenderer playerSprite;
    private int originalOrder;
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private PlayerSecondAttack playerAttack;
    
    
    
    
    public PlayerInput PlayerInput { get; private set; }

    private void Awake()
    {
        playerSprite = GetComponent<SpriteRenderer>();
        health = GetComponent<PlayerHealthSystem>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        PlayerInput = GetComponent<PlayerInput>();
        initialScale = transform.localScale;
    
        // Guardamos la gravedad original aquí
        originalGravity = rb.gravityScale; 

        rb.freezeRotation = true;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
    }

    private void OnEnable()
    {
        var actions = playerInput.actions;
        actions["Move"].performed += UpdateMovement;
        actions["Move"].canceled += UpdateMovement;
        actions["Jump"].started += Jump;
        actions["Dash"].started += OnDashPerformed;
        actions["Shaft"].started += OnShaftPerformed;
        actions["Furioso"].started += OnFuriosoPerformed;
        actions["SlashDash"].started += OnSlashInput;

        // PlayerInput.actions["Player/Pause"].performed += OnPauseToggle;
        // PlayerInput.actions["UI/UnPause"].performed += OnPauseToggle;
    }

    private void OnDisable()
    {
        var actions = playerInput.actions;
        actions["Move"].performed -= UpdateMovement;
        actions["Move"].canceled -= UpdateMovement;
        actions["Jump"].started -= Jump;
        actions["Dash"].started -= OnDashPerformed;
        actions["Shaft"].started -= OnShaftPerformed;
        actions["Furioso"].started -= OnFuriosoPerformed;
        actions["SlashDash"].started -= OnSlashInput;
       
        inputVector = Vector2.zero;
        if (rb != null) 
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        }
        if (anim != null) anim.SetFloat("Speed", 0);
    }
    
    

    private void UpdateMovement(InputAction.CallbackContext ctx)
    {
        if (!enabled) 
        {
            inputVector = Vector2.zero;
            return;
        }
        inputVector = ctx.ReadValue<Vector2>();
    }

    private void Jump(InputAction.CallbackContext ctx)
    {
        if (!enabled || isExecutingFurioso) return;

        if (ctx.started && isGrounded)
        {
           
            float sorrowPerc = SorrownessManager.Instance.GetSorrowLevel();

            
            float extraJump = jumpHeight * (sorrowPerc * 0.5f);
            float finalJumpHeight = jumpHeight + extraJump;

            
            float gravity = Physics2D.gravity.y * rb.gravityScale;
            float jumpVelocity = Mathf.Sqrt(finalJumpHeight * -2f * gravity);

            
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpVelocity);
        
            isGrounded = false; 

            
        }
    }

    void Update()
    {
        
        if (isPaused) return;
        
        if (PlayerInput == null || PlayerInput.currentActionMap == null)
        {
            return; 
        }

        
        if (PlayerInput.currentActionMap.name != "PlayerSecond")
        {
           
            inputVector = Vector2.zero;

            
            if (anim != null) 
            {
                anim.SetFloat("Speed", 0);
            }
            
            return; 
        }
        
        GroundCheck(); 
        FlipSprite();  
        if (anim != null)
        {
            anim.SetFloat("Speed", Mathf.Abs(inputVector.x));
            anim.SetBool("isGrounded", isGrounded);
        }
    }

    private void FixedUpdate()
    {
       
        if (PlayerInput == null || PlayerInput.currentActionMap == null) 
        {
            return;
        }
        
        if (PlayerInput.currentActionMap.name != "PlayerSecond") 
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            return;
        }
        if (!enabled) 
        {
            
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            return;
        }
        if (isDashing || isSlashDashing) return;
        Move();
    }

    
    private void Move()
    {
        
        if (isDashing || isSlashDashing || isExecutingFurioso) return; 
        
        float sorrowPerc = SorrownessManager.Instance.GetSorrowLevel();
        
        float finalSpeed = movementSpeed + (movementSpeed * sorrowPerc * 0.4f);

        
        rb.linearVelocity = new Vector2(inputVector.x * finalSpeed, rb.linearVelocity.y);
    }
        private void OnFuriosoPerformed(InputAction.CallbackContext ctx)
        {
           
            if (!isExecutingFurioso && SorrownessManager.Instance.GetSorrowLevel() >= 0.9f)
            {
                StartCoroutine(ExecuteFuriosoRoutine());
            }
        }
        
        private void OnShaftPerformed(InputAction.CallbackContext ctx)
        {
            if (isExecutingFurioso) return;

            anim.SetTrigger("Shaft");
    
            
            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, shaftRadius, enemyLayer);
            foreach (Collider2D hit in hits)
            {
                if (hit.TryGetComponent(out IDamageable target))
                {
                    target.TakeDamage(shaftDamage);
                }
            }
        }
        
        
    private void OnSlashInput(InputAction.CallbackContext ctx)
    {
       
        if (ctx.started && isDashing && !isSlashDashing)
        {
            clickDuringDash = true;
        }
    }
  private IEnumerator ExecuteFuriosoRoutine()
{
    isExecutingFurioso = true;
    
    SpriteRenderer sr = GetComponent<SpriteRenderer>();
    positionBeforeFurioso = transform.position; 
    originalOrder = playerSprite.sortingOrder;
    playerSprite.sortingOrder = 100; 

    // 1. Detección de enemigos en el rango original
    Collider2D[] potentialTargets = Physics2D.OverlapCircleAll(positionBeforeFurioso, furiosoSearchRadius, enemyLayer);
    List<Transform> enemiesInRange = new List<Transform>();
    
    foreach (var col in potentialTargets) 
    {
        // Solo añadimos si es enemigo (evitamos al player si por error tiene la misma capa)
        if (!col.CompareTag("Player")) enemiesInRange.Add(col.transform);
    }

    if (enemiesInRange.Count == 0)
    {
        Debug.Log("No hay enemigos cerca para liberar la furia.");
        isExecutingFurioso = false;
        yield break;
    }

    // Preparación de estado
    rb.simulated = false; 
    SorrownessManager.Instance.ResetSorrow(); 
    Transform currentTarget = enemiesInRange[0];

    // 2. Bucle de 9 golpes encadenados
    for (int i = 1; i <= 9; i++)
    {
        // Lógica de "Salto entre objetivos"
        if (enemiesInRange.Count > 1)
        {
            // Salta al siguiente enemigo de la lista en cada iteración
            currentTarget = enemiesInRange[i % enemiesInRange.Count];
        }
        // Si el enemigo original murió o desapareció, buscamos el siguiente disponible
        if (currentTarget == null) 
        {
             // Filtro rápido para no romper el bucle
             enemiesInRange.RemoveAll(item => item == null);
             if(enemiesInRange.Count > 0) currentTarget = enemiesInRange[0];
             else break; 
        }

        // --- TELETRANSPORTE ---
        // Nos posicionamos a la izquierda o derecha del enemigo
        float offset = (currentTarget.position.x > transform.position.x) ? -1.2f : 1.2f;

// Forzamos la Z a un valor negativo (ej. -0.5f) para que esté más cerca de la cámara que el enemigo
        transform.position = new Vector3(currentTarget.position.x + offset, currentTarget.position.y, -0.5f);
        
        FlipTowards(currentTarget.position);

        // --- DISPARAR ANIMACIÓN ---
        anim.SetInteger("FuriosoIndex", i); // Indica cuál de las 9 toca
        anim.SetTrigger("NextHit");         // Dispara la transición instantánea

        // --- INSTANCIA DE DAÑO ---
        if (currentTarget.TryGetComponent(out IDamageable dmg))
        {
            dmg.TakeDamage(damagePerHit);
            // Aquí podrías instanciar un efecto de chispas o sangre
        }

        if (playerAudioSource && furiosoSound) 
            playerAudioSource.PlayOneShot(furiosoSound);

        // Espera muy corta para que se vea la animación antes del siguiente salto
        // Ajusta este tiempo según la duración de tus clips
        yield return new WaitForSeconds(timeBetweenHits);
    }

    // 3. FINALIZACIÓN
    playerSprite.sortingOrder = originalOrder;
    isExecutingFurioso = false;
    yield return new WaitForSeconds(0.1f); // Pequeño respiro tras el último golpe
    
    
    
    transform.position = positionBeforeFurioso; 
    rb.simulated = true;
    isExecutingFurioso = false;
    
    Debug.Log("Combo Furioso completado.");
}
    

    private void OnDashPerformed(InputAction.CallbackContext ctx)
    {
        if (canDash && !isDashing || isExecutingFurioso || PlayerInput.currentActionMap.name == "PlayerSecond")
        {
                StartCoroutine(DashLogic());
        }
    }
    private IEnumerator DashLogic()
    {
        canDash = false;
        isDashing = true;
        clickDuringDash = false;

        rb.gravityScale = 0f; // Usamos la global
        rb.linearVelocity = Vector2.zero; 

        float dashDirection = transform.localScale.x > 0 ? 1f : -1f;
        float speed = dashDistance / dashDuration;

        if (anim != null) anim.SetTrigger("Dash2");

        float timer = 0f;
        while (timer < dashDuration)
        {
            rb.MovePosition(rb.position + new Vector2(dashDirection * speed * Time.fixedDeltaTime, 0f));

            if (clickDuringDash)
            {
                StartCoroutine(ConvertIntoSlashDash(dashDirection, speed)); 
                yield break; 
            }

            timer += Time.deltaTime;
            yield return new WaitForFixedUpdate();
        }

        FinalizeDash();
    }

    private void FinalizeDash()
    {
        rb.linearVelocity = Vector2.zero;
        rb.gravityScale = originalGravity; 
        isDashing = false;
        StartCoroutine(DashCooldown());
    }

    private IEnumerator ConvertIntoSlashDash(float direction, float speed)
    {
        isSlashDashing = true;
        if (anim != null)
        {
            anim.ResetTrigger("Dash2"); 
            
            anim.SetTrigger("SlashDash2"); 
        }
        if (playerAudioSource != null && slashDashSound != null)
        {
            playerAudioSource.PlayOneShot(slashDashSound);
        }
       
        
        isSlashDashing = true;
        if (anim != null) anim.SetTrigger("SlashDash2");

        float attackSpeed = speed * 1.3f; 
        float timer = 0f;

        while (timer < dashDuration)
        {
            rb.MovePosition(rb.position + new Vector2(direction * attackSpeed * Time.fixedDeltaTime, 0f));
            if (SlashDash) CheckForDamageDash();
        
            timer += Time.deltaTime;
            yield return new WaitForFixedUpdate();
        }

        rb.linearVelocity = Vector2.zero;
        rb.gravityScale = originalGravity; // Acceso directo y seguro
        isDashing = false;
        isSlashDashing = false;
        SlashDash = false;
        alreadyDamaged.Clear();

        StartCoroutine(DashCooldown());
    }
    
    private IEnumerator DashCooldown()
    {
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
       
    }
    

    private void GroundCheck()
    {
        
        isGrounded = Physics2D.OverlapCircle(feet.position, detectionRadius, whatIsGround);
    }

    private void FlipSprite()
    {
        if (inputVector.x > 0.1f)
        {
            
            transform.localScale = initialScale;
        }
        else if (inputVector.x < -0.1f)
        {
            
            transform.localScale = new Vector3(-initialScale.x, initialScale.y, initialScale.z);
        }
    }

    private void OnDrawGizmos()
    {
        if (feet != null)
        {
            Gizmos.color = isGrounded ? Color.green : Color.red;
            Gizmos.DrawWireSphere(feet.position, detectionRadius);
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(transform.position, furiosoSearchRadius);
            
        }
    }
    private void FlipTowards(Vector3 targetPos)
    {
        if (targetPos.x > transform.position.x)
            transform.localScale = new Vector3(Mathf.Abs(initialScale.x), initialScale.y, initialScale.z);
        else
            transform.localScale = new Vector3(-Mathf.Abs(initialScale.x), initialScale.y, initialScale.z);
    }
    
    private void CheckForDamageDash()
    {
        if (attackPointDash == null) return;

        // 1. Obtenemos el nivel de Sorrow (0 a 1)
        float sorrowPerc = SorrownessManager.Instance.GetSorrowLevel();
    
        // 2. Cálculo de Daño: Daño base + un extra proporcional al Sorrow (ej. +100% al máximo)
        float currentDamage = baseDamageDash + (baseDamageDash * sorrowPerc);
    
        // 3. Cálculo de Rango: Radio base + un 50% extra basado en el Sorrow
        float currentRadius = baseAttackRadiusDash + (baseAttackRadiusDash * sorrowPerc * 0.5f);
    
        // 4. Detección circular
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPointDash.position, currentRadius, whatIsDamageable);

        foreach (Collider2D enemy in hitEnemies)
        {
            if (enemy.TryGetComponent(out IDamageable damageable) && !alreadyDamaged.Contains(damageable))
            {
                damageable.TakeDamage(currentDamage);
                alreadyDamaged.Add(damageable);

                // --- AÑADE ESTA LÍNEA ---
                if(SorrownessManager.Instance != null) 
                    SorrownessManager.Instance.PlayerDealtDamage();
            }
        }
    }
   
    
    public void OpenSlashDashAttackWindow()
    {
        SlashDash = true;
    }

    public void CloseSlashDashAttackWindow()
    {
        SlashDash = false;
        alreadyDamaged.Clear(); 
    }
    private void OnDrawGizmosSelected()
    {
        if (attackPointDash == null) return;
        Gizmos.color = Color.red;

       
        float visualRadius = baseAttackRadiusDash;
        
       
        if (Application.isPlaying && SorrownessManager.Instance != null)
        {
            float sorrowPerc = SorrownessManager.Instance.GetSorrowLevel();
            visualRadius = baseAttackRadiusDash + (baseAttackRadiusDash * sorrowPerc * 0.5f);
        }

        Gizmos.DrawWireSphere(attackPointDash.position, visualRadius);
    }
    
}


