using UnityEngine;
using System.Collections;
using Player; // Asegúrate de que el namespace sea correcto

public class PlayerStaggerSystem : MonoBehaviour
{
    [Header("Ref")]
    [SerializeField] private PlayerHealthSystem playerHealth;
    [SerializeField] private MonoBehaviour playerMovementScript; 
    [SerializeField] private Animator anim; 

    [Header("Configuration")]
    [SerializeField] private float freezeDuration = 4.0f;
    [SerializeField] private string staggerParameterName = "Stagger"; 

    private void OnEnable()
    {
        if (playerHealth != null) playerHealth.OnStagger += HandlePlayerStagger;
    }

    private void OnDisable()
    {
        if (playerHealth != null) playerHealth.OnStagger -= HandlePlayerStagger;
    }

    private void HandlePlayerStagger()
    {
        // 1. ACTIVAR LA ANIMACIÓN
        if (anim != null)
        {
            anim.SetTrigger(staggerParameterName);
        }
        else
        {
            Debug.LogWarning("¡No hay Animator asignado en PlayerStaggerResponse!");
        }

        StartCoroutine(StaggerSequence());
    }

    private IEnumerator StaggerSequence()
    {
        // 2. BLOQUEAR MOVIMIENTO
        if (playerMovementScript != null) playerMovementScript.enabled = false;
        
        yield return new WaitForSeconds(freezeDuration);

        // 3. RESTAURAR MOVIMIENTO
        if (playerMovementScript != null) playerMovementScript.enabled = true;
        
        // Opcional: Podrías forzar la vuelta a Idle si se queda trabado
        // anim.Play("Idle"); 
    }
}