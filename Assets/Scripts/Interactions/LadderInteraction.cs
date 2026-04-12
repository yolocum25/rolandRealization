using UnityEngine;

public class LadderInteraction : MonoBehaviour, IInteractable
{
    [Header("tp POINTS")]
    [SerializeField] private Transform topPoint;    // Arrastra el objeto de arriba
    [SerializeField] private Transform bottomPoint; // Arrastra el objeto de abajo

    [Header("Confi")]
    [SerializeField] private bool isAtBottom = true; // Controla hacia dónde va el jugador
    [SerializeField] private AudioSource interactSound;

    public void GenerateInteraction()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null && topPoint != null && bottomPoint != null)
        {
           
            if (interactSound != null) interactSound.Play();

            
            if (isAtBottom)
            {
               
                player.transform.position = topPoint.position;
                isAtBottom = false; 
               
            }
            else
            {
                
                player.transform.position = bottomPoint.position;
                isAtBottom = true; 
               
            }

            
            if (player.TryGetComponent(out Rigidbody2D rb))
            {
                rb.linearVelocity = Vector2.zero;
            }
        }
    }
}