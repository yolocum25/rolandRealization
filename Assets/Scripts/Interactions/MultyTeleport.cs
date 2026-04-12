using UnityEngine;

public class MultiTeleport : MonoBehaviour, IInteractable
{
    [Header("Puntos de Teletransporte")]
    [SerializeField] private Transform[] teleportPoints; 
    
    [Header("Estado Actual")]
    [SerializeField] private int currentPointIndex = 0; 

    [Header("Configuración")]
    [SerializeField] private AudioSource interactSound;

    public void GenerateInteraction()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null && teleportPoints.Length > 0)
        {
            // 1. Calculamos el siguiente índice
            // El símbolo '%' (módulo) hace que al llegar al final vuelva al 0
            currentPointIndex = (currentPointIndex + 1) % teleportPoints.Length;

            // 2. Referencia al punto destino
            Transform targetPoint = teleportPoints[currentPointIndex];

            if (targetPoint != null)
            {
                
                if (interactSound != null) interactSound.Play();

                
                player.transform.position = targetPoint.position;

                
                if (player.TryGetComponent(out Rigidbody2D rb))
                {
                    rb.linearVelocity = Vector2.zero;
                }
            }
        }
    }
}