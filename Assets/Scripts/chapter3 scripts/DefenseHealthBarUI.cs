using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DefenseHealthBarUI : MonoBehaviour
{
    [Header("Referencias Base")]
    [SerializeField] private DefenseTarget targetDefense;
    [SerializeField] private Image healthBarFill; // La imagen con Fill Amount

    [Header("Ajustes del Efecto de Daño")]
    [SerializeField] private Color flashColor = Color.white; // Color al recibir daño
    [SerializeField] private float flashDuration = 0.2f;     // Cuánto dura el color blanco
    [SerializeField] private float shakeDuration = 0.3f;    // Cuánto dura el temblor
    [SerializeField] private float shakeMagnitude = 5f;    // Qué tan fuerte es el temblor

    private Color originalColor;       // Para guardar el color rojo original
    private Vector3 originalPosition;  // Para guardar la posición original de la barra
    private Coroutine flashCoroutine;  // Para controlar que no se solapen los flashes
    private Coroutine shakeCoroutine;  // Para controlar que no se solapen los temblores
    private RectTransform rectTransform; // El componente que maneja la posición en la UI

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();

        

        if (healthBarFill != null)
        {
            originalColor = healthBarFill.color; // Guardamos el color original (rojo)
        }
        
        if (rectTransform != null)
        {
            originalPosition = rectTransform.localPosition; // Guardamos la posición original
        }
    }

    private void OnEnable()
    {
        if (targetDefense != null)
        {
            targetDefense.OnHealthChanged += OnDefenseDamaged;
        }
    }

    private void OnDisable()
    {
        if (targetDefense != null)
        {
            targetDefense.OnHealthChanged -= OnDefenseDamaged;
        }
    }

    // Esta función se ejecuta CADA vez que la defensa recibe daño
    private void OnDefenseDamaged()
    {
        // 1. Actualizamos el Fill Amount (la lógica de antes)
        if (targetDefense != null && healthBarFill != null)
        {
            healthBarFill.fillAmount = targetDefense.GetHealthNormalized();
        }

        // 2. Iniciamos los efectos visuales
        TriggerFlashEffect();
        TriggerShakeEffect();
    }

    private void TriggerFlashEffect()
    {
        if (healthBarFill == null) return;

        // Si ya hay un flash en curso, lo paramos para empezar este nuevo
        if (flashCoroutine != null) StopCoroutine(flashCoroutine);
        flashCoroutine = StartCoroutine(FlashCoroutine());
    }

    private IEnumerator FlashCoroutine()
    {
        // Cambiamos al color blanco instantáneamente
        healthBarFill.color = flashColor;

        // Esperamos la duración establecida
        yield return new WaitForSeconds(flashDuration);

        // Volvemos al color original poco a poco (o instantáneo, como prefieras)
        // Para instantáneo: healthBarFill.color = originalColor;
        
        // Versión suave (Lerp):
        float elapsed = 0f;
        float fadeSpeed = 5f; // Velocidad de retorno
        while (elapsed < 1f)
        {
            elapsed += Time.deltaTime * fadeSpeed;
            healthBarFill.color = Color.Lerp(flashColor, originalColor, elapsed);
            yield return null;
        }
        healthBarFill.color = originalColor; // Aseguramos el color final
    }

    private void TriggerShakeEffect()
    {
        if (rectTransform == null) return;

        // Si ya se está sacudiendo, lo paramos para empezar este nuevo impacto
        if (shakeCoroutine != null) StopCoroutine(shakeCoroutine);
        shakeCoroutine = StartCoroutine(ShakeCoroutine());
    }

    private IEnumerator ShakeCoroutine()
    {
        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            // Calculamos un desplazamiento aleatorio dentro de un círculo
            Vector2 randomPos = Random.insideUnitCircle * shakeMagnitude;
            
            // Aplicamos el desplazamiento a la posición original
            rectTransform.localPosition = new Vector3(originalPosition.x + randomPos.x, originalPosition.y + randomPos.y, originalPosition.z);

            elapsed += Time.deltaTime;
            yield return null; // Esperamos al siguiente frame
        }

        // Al terminar, nos aseguramos de volver a la posición original exacta
        rectTransform.localPosition = originalPosition;
    }
}