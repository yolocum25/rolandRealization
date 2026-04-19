using UnityEngine;

public class PlayerMain : MonoBehaviour
{
    [field: SerializeField] public Transform InteractPoint { get; private set; }
    [field: SerializeField] public float InteractionRadius { get; private set; }
    [SerializeField] private LayerMask interactionLayer;

    public Rigidbody2D Rb { get; private set; }
    public Animator Anim { get; private set; }

    private void Awake()
    {
        Rb = GetComponent<Rigidbody2D>();
        Anim = GetComponent<Animator>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            PerformInteraction();
        }
    }

    private void PerformInteraction()
    {
       
        Collider2D[] colliders = Physics2D.OverlapCircleAll(InteractPoint.position, InteractionRadius, interactionLayer);

        foreach (var col in colliders)
        {
            
            IInteractable interactable = col.GetComponent<IInteractable>();
            
            if (interactable != null)
            {
                interactable.GenerateInteraction();
                break; 
            }
        }
    }

    
    private void OnDrawGizmosSelected()
    {
        if (InteractPoint != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(InteractPoint.position, InteractionRadius);
        }
    }
}