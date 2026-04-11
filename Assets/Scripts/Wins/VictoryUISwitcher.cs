
using UnityEngine;
using UnityEngine.InputSystem; 
using System.Collections;


public class VictoryUISwitcher : MonoBehaviour
{
    [Header("Canvas")]
    [SerializeField] private GameObject gameHUD;      
    [SerializeField] private GameObject victoryCanvas; 

    [Header("Player References")]
    [SerializeField] private PlayerInput playerInput; 
    [SerializeField] private PlayerAttackSystem playerAttack; 

    private void OnEnable()
    {
        StartCoroutine(WaitAndSubscribe());
    }

    private IEnumerator WaitAndSubscribe()
    {
        while (EventManager.Instance == null) yield return null;
        EventManager.Instance.OnVictory += HandleVictoryUI;
    }

    private void OnDisable()
    {
        if (EventManager.Instance != null) 
            EventManager.Instance.OnVictory -= HandleVictoryUI;
    }

    private void HandleVictoryUI()
    {
        StartCoroutine(ExecuteSwitch());
    }

    private IEnumerator ExecuteSwitch()
    {
        
        yield return new WaitForSecondsRealtime(0.2f);

        
        if (playerAttack != null)
        {
            playerAttack.enabled = false;
        }

        
        if (playerInput != null)
        {
            playerInput.SwitchCurrentActionMap("UI");
            
            
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            
        }
        
        if (victoryCanvas != null) 
        {
            victoryCanvas.SetActive(true);
        }
        
        if (gameHUD != null) 
        {
            gameHUD.SetActive(false);
        }
    }
}