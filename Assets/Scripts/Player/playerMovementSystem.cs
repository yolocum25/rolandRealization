using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement2D : MonoBehaviour
{
    [Header("Configuración de Movimiento")]
    [SerializeField] private float speed = 8f;
    [SerializeField] private float jumpForce = 12f;

    private Rigidbody2D rb;
    private Vector2 moveInput;
    private bool isGrounded;

    [Header("Detección de Suelo")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Este método se vincula al evento "Performed" y "Canceled" del Player Input
    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    // Este método se vincula al evento del botón de Salto
    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.started && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
    }

    private void FixedUpdate()
    {
        // Aplicamos el movimiento horizontal
        rb.linearVelocity = new Vector2(moveInput.x * speed, rb.linearVelocity.y);

        // Verificamos si tocamos el suelo
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        
        // Opcional: Voltear el sprite según la dirección
        FlipSprite();
    }

    private void FlipSprite()
    {
        if (moveInput.x > 0) transform.localScale = new Vector3(1, 1, 1);
        else if (moveInput.x < 0) transform.localScale = new Vector3(-1, 1, 1);
    }
}