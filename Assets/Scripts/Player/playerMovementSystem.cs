    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
using UnityEngine.InputSystem;

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
    [SerializeField] private float dashForce = 20f;
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private float dashCooldown = 4f;
    private bool canDash = true;
    private bool isDashing;
    
    [Header("SlashDash Settings")]
    [SerializeField] private LayerMask whatIsDamageable;
    [SerializeField] private Transform attackPointDash; 
    [SerializeField] private float baseAttackRadiusDash = 4;
    [SerializeField] private float baseDamageDash = 60f;
    private bool canSlashDash = true;
    private bool isSlashDashing;
    private bool SlashDash;

    private Rigidbody2D rb;
    private Animator anim;
    private bool isGrounded;
    private Vector2 inputVector;
    private Vector3 initialScale;
    private List<IDamageable> alreadyDamaged = new();

    public PlayerInput PlayerInput { get; private set; }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        PlayerInput = GetComponent<PlayerInput>();
        initialScale = transform.localScale;
        anim = GetComponent<Animator>();
        rb.freezeRotation = true;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
    }

    private void OnEnable()
    {
        PlayerInput.actions["Move"].performed += UpdateMovement;
        PlayerInput.actions["Move"].canceled += UpdateMovement;
        PlayerInput.actions["Jump"].started += Jump;
        PlayerInput.actions["Dash"].started += OnDashPerformed;
        PlayerInput.actions["SlashDash"].started += OnDashPerformed;
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
            // Si no estamos en el mapa "Player", reseteamos el movimiento
            inputVector = Vector2.zero;

            // Si tienes animaciones, forzamos el Idle para que no se quede corriendo
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
        Move();
    }

    private void Move()
    {
        float posPerc = EmotionManager.Instance.positiveBar.FillPercentage;
        float negPerc = EmotionManager.Instance.negativeBar.FillPercentage;
        float finalSpeed = movementSpeed + (movementSpeed * posPerc * 0.5f) - (movementSpeed * negPerc * 0.3f);
        rb.linearVelocity = new Vector2(inputVector.x * finalSpeed, rb.linearVelocity.y);
    }

    private void OnDashPerformed(InputAction.CallbackContext ctx)
    {
        float posPerc = EmotionManager.Instance.positiveBar.FillPercentage;
        float negPerc = EmotionManager.Instance.negativeBar.FillPercentage;
        
        
        if (canDash && !isDashing && PlayerInput.currentActionMap.name == "Player")
        {
            if (posPerc >= 0.5f && negPerc <= 0.7f)
            {
                StartCoroutine(SlashDashSec());
            }
            else if (posPerc >= 0.5f)
            {
                StartCoroutine(Dash());
            }
           
            
        }
    }

    private IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;


        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;


        float posPerc = EmotionManager.Instance.positiveBar.FillPercentage;

        float currentDashForce = dashForce + (dashForce * posPerc * 0.5f);


        float dashDirection = inputVector.x != 0 ? Mathf.Sign(inputVector.x) : transform.localScale.x;

        rb.linearVelocity = new Vector2(dashDirection * currentDashForce, 0f);


        if (anim != null) anim.SetTrigger("Dash");

        yield return new WaitForSeconds(dashDuration);

        
        rb.gravityScale = originalGravity;
        isDashing = false;

        yield return new WaitForSeconds(dashCooldown);
        canDash = true;


    }
    private IEnumerator SlashDashSec()
    {
        canSlashDash = false;
        isSlashDashing = true;


        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;


        float posPerc = EmotionManager.Instance.positiveBar.FillPercentage;

        float currentDashForce = dashForce + (dashForce * posPerc * 0.5f);


        float dashDirection = inputVector.x != 0 ? Mathf.Sign(inputVector.x) : transform.localScale.x;

        rb.linearVelocity = new Vector2(dashDirection * currentDashForce, 0f);
        if (SlashDash)
        {
            CheckForDamageDash();
        }


        if (anim != null) anim.SetTrigger("SlashDash");

        yield return new WaitForSeconds(dashDuration);

        
        rb.gravityScale = originalGravity;
        isSlashDashing = false;

        yield return new WaitForSeconds(dashCooldown);
        canSlashDash = true;


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
    
    
}

