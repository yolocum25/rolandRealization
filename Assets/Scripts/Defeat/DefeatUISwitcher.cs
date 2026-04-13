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
        // Determinamos la causa de la derrota
        if (LevelTimer.Instance != null && LevelTimer.Instance.IsTimeUp())
        {
            defeatReason = "Out of Time";
        }
        else
        {
            defeatReason = "Mutilated by Anomalis";
        }


        // Iniciamos la secuencia visual
        StartCoroutine(ExecuteDefeatSequence());
    }

    private IEnumerator ExecuteDefeatSequence()
    {
        // 1. Espera inicial (Cámara lenta o efecto de muerte)
        yield return new WaitForSecondsRealtime(0.5f);

        // 2. ACTIVAR EL CANVAS PRIMERO (Vital para el Audio y la UI)
        // Siempre activa el objeto ANTES de intentar acceder a sus componentes
        if (defeatCanvas != null)
        {
            defeatCanvas.SetActive(true);
        }

        // 3. Configurar la pantalla (Ahora que el objeto está activo, el audio sonará)
        DefeatScreen dScreen = defeatCanvas.GetComponent<DefeatScreen>();
        if (dScreen != null)
        {
            dScreen.SetupDefeat(defeatReason);
        }

        // 4. Gestión de Scripts y HUD
        if (playerAttack != null) playerAttack.enabled = false;
        if (gameHUD != null) gameHUD.SetActive(false);

        // 5. Configuración del Input (Aseguramos que esté encendido antes del cambio)
        if (playerInput != null)
        {
            playerInput.enabled = true;
            playerInput.SwitchCurrentActionMap("UI");

            // Liberar el ratón para los botones
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        // 6. Detener el tiempo de juego (opcional, si quieres que todo se congele tras la muerte)
        // Time.timeScale = 0f; 
    }
}
