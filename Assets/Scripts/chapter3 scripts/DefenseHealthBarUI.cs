using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DefenseHealthBarUI : MonoBehaviour
{
    [Header("Referencias Base")]
    [SerializeField] private DefenseTarget targetDefense;
    [SerializeField] private Image healthBarFill; 

    [Header("Ajustes del Efecto de Daño")]
    [SerializeField] private Color flashColor = Color.white; 
    [SerializeField] private float flashDuration = 0.2f;     
    [SerializeField] private float shakeDuration = 0.3f;    
    [SerializeField] private float shakeMagnitude = 5f;    

    private Color originalColor;       
    private Vector3 originalPosition;  
    private Coroutine flashCoroutine;  
    private Coroutine shakeCoroutine; 
    private RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();

        

        if (healthBarFill != null)
        {
            originalColor = healthBarFill.color; 
        }
        
        if (rectTransform != null)
        {
            originalPosition = rectTransform.localPosition; 
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

   
    private void OnDefenseDamaged()
    {
       
        if (targetDefense != null && healthBarFill != null)
        {
            healthBarFill.fillAmount = targetDefense.GetHealthNormalized();
        }

       
        TriggerFlashEffect();
        TriggerShakeEffect();
    }

    private void TriggerFlashEffect()
    {
        if (healthBarFill == null) return;
        
        if (flashCoroutine != null) StopCoroutine(flashCoroutine);
        flashCoroutine = StartCoroutine(FlashCoroutine());
    }

    private IEnumerator FlashCoroutine()
    {
       
        healthBarFill.color = flashColor;

       
        yield return new WaitForSeconds(flashDuration);

       
        
        
        
        float elapsed = 0f;
        float fadeSpeed = 5f; 
        while (elapsed < 1f)
        {
            elapsed += Time.deltaTime * fadeSpeed;
            healthBarFill.color = Color.Lerp(flashColor, originalColor, elapsed);
            yield return null;
        }
        healthBarFill.color = originalColor; 
    }

    private void TriggerShakeEffect()
    {
        if (rectTransform == null) return;

        
        if (shakeCoroutine != null) StopCoroutine(shakeCoroutine);
        shakeCoroutine = StartCoroutine(ShakeCoroutine());
    }

    private IEnumerator ShakeCoroutine()
    {
        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            
            Vector2 randomPos = Random.insideUnitCircle * shakeMagnitude;
            
            
            rectTransform.localPosition = new Vector3(originalPosition.x + randomPos.x, originalPosition.y + randomPos.y, originalPosition.z);

            elapsed += Time.deltaTime;
            yield return null; 
        }

        
        rectTransform.localPosition = originalPosition;
    }
}