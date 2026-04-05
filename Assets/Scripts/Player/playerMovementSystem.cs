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

    private Rigidbody2D rb;
    private Animator anim;
    private bool isGrounded;
    private Vector2 inputVector;
    private Vector3 initialScale;

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
    }

    private void OnDisable()
    {
        PlayerInput.actions["Move"].performed -= UpdateMovement;
        PlayerInput.actions["Move"].canceled -= UpdateMovement;
        PlayerInput.actions["Jump"].started -= Jump;
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
            float gravity = Physics2D.gravity.y * rb.gravityScale;
            float jumpVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpVelocity);
            isGrounded = false; 
        }
    }

    private void Update()
    {
        if (!enabled) return;
        GroundCheck();
        FlipSprite();
        float horizontalSpeed = Mathf.Abs(inputVector.x);
        if (anim != null) 
        {
            anim.SetFloat("Speed", horizontalSpeed);
        }
    }

    private void FixedUpdate()
    {
        if (!enabled) 
        {
            
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            return;
        }
        Move();
    }

    private void Move()
    {
       
        rb.linearVelocity = new Vector2(inputVector.x * movementSpeed, rb.linearVelocity.y);
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
    
}

