using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class DefeatUISwitcher : MonoBehaviour
{
    [Header("Canvas")] [SerializeField] private GameObject gameHUD;
    [SerializeField] private GameObject defeatCanvas;

    [Header("Player References")] [SerializeField]
    private PlayerInput playerInput;

    [SerializeField] private PlayerAttackSystem playerAttack;


    private string defeatReason = "Died by";


    private void OnEnable()
    {
        StartCoroutine(WaitAndSubscribe());
    }

    private IEnumerator WaitAndSubscribe()
    {
        while (EventManager.Instance == null) yield return null;
        EventManager.Instance.OnGameOver += HandleDefeatUI;

    }

    private void OnDisable()
    {
        if (EventManager.Instance != null) EventManager.Instance.OnGameOver -= HandleDefeatUI;
    }

    private void HandleDefeatUI()
    {
        
        if (LevelTimer.Instance != null && LevelTimer.Instance.IsTimeUp())
        {
            defeatReason = "Out of Time";
        }
        else
        {
            defeatReason = "Mutilated by Anomalis";
        }


        
        StartCoroutine(ExecuteDefeatSequence());
    }

    private IEnumerator ExecuteDefeatSequence()
    {
        
        yield return new WaitForSecondsRealtime(0.5f);

        
        if (defeatCanvas != null)
        {
            defeatCanvas.SetActive(true);
        }

       
        DefeatScreen dScreen = defeatCanvas.GetComponent<DefeatScreen>();
        if (dScreen != null)
        {
            dScreen.SetupDefeat(defeatReason);
        }

       
        if (playerAttack != null) playerAttack.enabled = false;
        if (gameHUD != null) gameHUD.SetActive(false);

      
        if (playerInput != null)
        {
            playerInput.enabled = true;
            playerInput.SwitchCurrentActionMap("UI");

           
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        
    }
}
