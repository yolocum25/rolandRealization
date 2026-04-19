using UnityEngine;
using System.Collections;
using Player; 
using UnityEngine.InputSystem;

public class PlayerStaggerSystem : MonoBehaviour
{
    [Header("Ref")]
    [SerializeField] private PlayerHealthSystem playerHealth;
    [SerializeField] private MonoBehaviour playerMovementScript; 
    [SerializeField] private PlayerAttackSystem playerAttackScript;
    [SerializeField] private Animator anim; 
    [SerializeField] private PlayerInput pInput;

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
       
        if (anim != null)
        {
            anim.SetTrigger(staggerParameterName);
        }
      

        StartCoroutine(StaggerSequence());
    }

    private IEnumerator StaggerSequence()
    {
       
        if (playerMovementScript != null) playerMovementScript.enabled = false;
        if (playerAttackScript != null) playerAttackScript.enabled = false;
        PlayerInput pInput = GetComponent<PlayerInput>();
        pInput.SwitchCurrentActionMap("Stagger");
        if (!pInput.enabled) pInput.enabled = true;
        
        if (anim != null) 
        {
            anim.SetFloat("Speed", 0f); 
        }
        if (anim != null) anim.Play("roland_Stagger", 0, 0f);
        
        
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        }

        yield return new WaitForSeconds(4f); 

       
        pInput.SwitchCurrentActionMap("Player");
        if (playerMovementScript != null) playerMovementScript.enabled = true;
        if (playerAttackScript != null) playerAttackScript.enabled = true;
    }
}