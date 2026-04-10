using UnityEngine;

public class LadderInteraction : MonoBehaviour, IInteractable
{
    [Header("Puntos de Teletransporte")]
    [SerializeField] private Transform topPoint;    // Arrastra el objeto de arriba
    [SerializeField] private Transform bottomPoint; // Arrastra el objeto de abajo

    [Header("Configuración")]
    [SerializeField] private bool isAtBottom = true; // Controla hacia dónde va el jugador
    [SerializeField] private AudioSource interactSound;

    public void GenerateInteraction()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null && topPoint != null && bottomPoint != null)
        {
            // 1. Sonido
            if (interactSound != null) interactSound.Play();

            // 2. Determinar destino y actualizar estado
            if (isAtBottom)
            {
                // Si estamos abajo, teletransportamos arriba
                player.transform.position = topPoint.position;
                isAtBottom = false; // La próxima vez irá hacia abajo
                Debug.Log("Subiendo...");
            }
            else
            {
                // Si estamos arriba, teletransportamos abajo
                player.transform.position = bottomPoint.position;
                isAtBottom = true; // La próxima vez irá hacia arriba
                Debug.Log("Bajando...");
            }

            // 3. Limpiar velocidad para evitar bugs físicos
            if (player.TryGetComponent(out Rigidbody2D rb))
            {
                rb.linearVelocity = Vector2.zero;
            }
        }
    }
}