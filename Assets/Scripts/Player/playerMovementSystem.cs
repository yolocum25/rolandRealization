    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement2D : MonoBehaviour
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
    private bool IsPaused;
    
    [Header("SlashDash Settings")]
    [SerializeField] private LayerMask whatIsDamageable;
    [SerializeField] private Transform attackPointDash; 
    [SerializeField] private float baseAttackRadiusDash = 4;
    [SerializeField] private float baseDamageDash = 60f;
    private bool canSlashDash = true;
    private bool isSlashDashing;
    private bool SlashDash;
    private bool clickDuringDash = false;
    
    [Header("Audio SlashDash")]
    [SerializeField] private AudioSource playerAudioSource; 
    [SerializeField] private AudioClip slashDashSound;
    
    private Rigidbody2D rb;
    private Animator anim;
    private bool isGrounded;
    private Vector2 inputVector;
    private Vector3 initialScale;
    private List<IDamageable> alreadyDamaged = new();
    private float originalGravity; 
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private PlayerAttackSystem playerAttack;
    
    [Header("Pause Settings")]
    [SerializeField] private GameObject PauseCanvas; // El Pausa
    [SerializeField] private GameObject gameHUD;       
    public PlayerInput PlayerInput { get; private set; }

    private void Awake()
    {
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
        PlayerInput.actions["Move"].performed += UpdateMovement;
        PlayerInput.actions["Move"].canceled += UpdateMovement;
        PlayerInput.actions["Jump"].started += Jump;
        PlayerInput.actions["Dash"].started += OnDashPerformed;
        PlayerInput.actions["SlashDash"].started += OnSlashInput;
        PlayerInput.actions["Player/Pause"].performed += OnPauseToggle;
        PlayerInput.actions["UI/UnPause"].performed += OnPauseToggle;
    }

    private void OnDisable()
    {
        PlayerInput.actions["Move"].performed -= UpdateMovement;
        PlayerInput.actions["Move"].canceled -= UpdateMovement;
        PlayerInput.actions["Jump"].started -= Jump;
        PlayerInput.actions["Dash"].started -= OnDashPerformed;
        PlayerInput.actions["SlashDash"].started -= OnDashPerformed;
       
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
        if (!enabled) return;

        if (ctx.started && isGrounded)
        {
            float posPerc = EmotionManager.Instance.positiveBar.FillPercentage;
            float extraJump = jumpHeight * (posPerc * 0.4f);
            float finalJumpHeight = jumpHeight + extraJump;
            float gravity = Physics2D.gravity.y * rb.gravityScale;
            float jumpVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpVelocity);
            isGrounded = false; 
        }
    }

    void Update()
    {
        if (PlayerInput == null || PlayerInput.currentActionMap == null)
        {
            return; 
        }

        // 2. CONTROL DE ESTADO: ¿Estamos en modo Diálogo/UI o en modo Juego?
        if (PlayerInput.currentActionMap.name != "Player")
        {
           
            inputVector = Vector2.zero;

            
            if (anim != null) 
            {
                anim.SetFloat("Speed", 0);
            }

            // Salimos del Update aquí. Todo lo que esté debajo no se ejecutará.
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
        
        if (PlayerInput.currentActionMap.name != "Player") 
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
            if (isDashing || isSlashDashing) return; 
            float posPerc = EmotionManager.Instance.positiveBar.FillPercentage;
            float negPerc = EmotionManager.Instance.negativeBar.FillPercentage;
    
            float finalSpeed = movementSpeed + (movementSpeed * posPerc * 0.5f) - (movementSpeed * negPerc * 0.3f);
    
           
            rb.linearVelocity = new Vector2(inputVector.x * finalSpeed, rb.linearVelocity.y);
        }
    

    private void OnSlashInput(InputAction.CallbackContext ctx)
    {
        // Solo registramos el click si el botón se presiona (started)
        // y si estamos en medio de un dash normal.
        if (ctx.started && isDashing && !isSlashDashing)
        {
            clickDuringDash = true;
        }
    }

    private void OnPauseToggle(InputAction.CallbackContext ctx)
    {
        
        IsPaused = !IsPaused;

        if (IsPaused)
        {
            Time.timeScale = 0f;
            playerInput.SwitchCurrentActionMap("UI");
            if (gameHUD != null) gameHUD.SetActive(false);
            if (PauseCanvas != null) PauseCanvas.SetActive(true);
            if (playerInput != null) playerInput.enabled = false;
            if (playerAttack != null) playerAttack.enabled = false;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            
        }
        
        {
            Time.timeScale = 1f;
            playerInput.SwitchCurrentActionMap("Player");
            if (gameHUD != null) gameHUD.SetActive(true);
            if(PauseCanvas != null) PauseCanvas.SetActive(false);
            if (PlayerInput != null) PlayerInput.enabled = true;
            if (playerInput != null) playerInput.enabled = true;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
        
        
    }
    
    private void OnDashPerformed(InputAction.CallbackContext ctx)
    {
        if (canDash && !isDashing && PlayerInput.currentActionMap.name == "Player")
        {
            float posPerc = EmotionManager.Instance.positiveBar.FillPercentage;
            if (posPerc >= 0.5f)
            {
                StartCoroutine(DashLogic());
            }
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

        if (anim != null) anim.SetTrigger("Dash");

        float timer = 0f;
        while (timer < dashDuration)
        {
            rb.MovePosition(rb.position + new Vector2(dashDirection * speed * Time.fixedDeltaTime, 0f));

            if (clickDuringDash && EmotionManager.Instance.negativeBar.FillPercentage <= 0.7f)
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
            anim.ResetTrigger("Dash"); 
            
            anim.SetTrigger("SlashDash"); 
        }
        if (playerAudioSource != null && slashDashSound != null)
        {
            playerAudioSource.PlayOneShot(slashDashSound);
        }
        if (EmotionManager.Instance != null)
        {
            EmotionManager.Instance.LostPositive(20f); 
            EmotionManager.Instance.GainNegative(15f); 
        }
        
        isSlashDashing = true;
        if (anim != null) anim.SetTrigger("SlashDash");

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
    // private IEnumerator Dash()
    // {
    //     canDash = false;
    //     isDashing = true;
    //
    //
    //     float originalGravity = rb.gravityScale;
    //     rb.gravityScale = 0f;
    //
    //
    //     float posPerc = EmotionManager.Instance.positiveBar.FillPercentage;
    //
    //     float currentDashForce = dashForce + (dashForce * posPerc * 0.5f);
    //
    //
    //     float dashDirection = inputVector.x != 0 ? Mathf.Sign(inputVector.x) : transform.localScale.x;
    //
    //     rb.linearVelocity = new Vector2(dashDirection * currentDashForce, 0f);
    //
    //
    //     if (anim != null) anim.SetTrigger("Dash");
    //
    //     yield return new WaitForSeconds(dashDuration);
    //
    //     
    //     rb.gravityScale = originalGravity;
    //     isDashing = false;
    //
    //     yield return new WaitForSeconds(dashCooldown);
    //     canDash = true;
    //
    //
    // }
    // private IEnumerator SlashDashSec()
    // {
    //     // Bloqueo total para evitar doble dash
    //     canDash = false;
    //     isDashing = true;
    //
    //     float originalGravity = rb.gravityScale;
    //     rb.gravityScale = 0f;
    //
    //     // Dirección: si no te mueves, hacia donde mire el sprite
    //     float dashDirection = inputVector.x != 0 ? Mathf.Sign(inputVector.x) : transform.localScale.x;
    //     float currentDashForce = dashForce + (dashForce * EmotionManager.Instance.positiveBar.FillPercentage * 0.5f);
    //
    //     rb.linearVelocity = new Vector2(dashDirection * currentDashForce, 0f);
    //
    //     if (anim != null) anim.SetTrigger("SlashDash");
    //
    //     // VARIABLE LOCAL: Siempre empieza en 0 cada vez que activas el dash
    //     float timer = 0f; 
    //
    //     while (timer < dashDuration)
    //     {
    //         // El daño solo ocurre si SlashDash es true (activado por Animation Event)
    //         if (SlashDash)
    //         {
    //             CheckForDamageDash();
    //         }
    //     
    //         timer += Time.deltaTime;
    //         yield return null;
    //     }
    //
    //     // Finalización del movimiento
    //     rb.gravityScale = originalGravity;
    //     isDashing = false;
    //
    //     // Reset de seguridad de la ventana de daño
    //     SlashDash = false; 
    //     alreadyDamaged.Clear();
    //
    //     // Espera del Cooldown antes de poder usar cualquier dash otra vez
    //     yield return new WaitForSeconds(dashCooldown);
    //     canDash = true;
    // }

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
        }
    }
    
    private void CheckForDamageDash()
    {
        
        if (attackPointDash == null) return;
        float posPerc = EmotionManager.Instance.positiveBar.FillPercentage;
        float negPerc = EmotionManager.Instance.negativeBar.FillPercentage;
        
        float currentDamage = baseDamageDash + (baseDamageDash * negPerc);
        float currentRadius = baseAttackRadiusDash + (baseAttackRadiusDash * posPerc * 0.5f);
        
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPointDash.position, currentRadius, whatIsDamageable);

        foreach (Collider2D enemy in hitEnemies)
        {
            
            if (enemy.TryGetComponent(out IDamageable damageable) && !alreadyDamaged.Contains(damageable))
            {
                damageable.TakeDamage(currentDamage);
                alreadyDamaged.Add(damageable);
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
        
       
        if (Application.isPlaying && EmotionManager.Instance != null)
        {
            float posPerc = EmotionManager.Instance.positiveBar.FillPercentage;
            visualRadius = baseAttackRadiusDash + (baseAttackRadiusDash * posPerc * 0.5f);
        }

        Gizmos.DrawWireSphere(attackPointDash.position, visualRadius);
    }
    
}

