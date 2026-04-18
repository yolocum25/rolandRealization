using UnityEngine;
using UnityEngine.UI;

public class SorrownessBar : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private Image fillImage; 
    
    [Header("Bar Settings")]
    [SerializeField] public float maxValue = 100f; // Ahora es propia de este script
    [SerializeField] private float lerpSpeed = 5f;

    private float currentValue = 0f;
    private float targetValue = 0f;

    // Propiedades para que el Manager lea los datos
    public float CurrentValue => currentValue;
    public float MaxValue => maxValue;
    public float FillPercentage => currentValue / maxValue;

    private void Update()
    {
        // Movimiento suave de la barra
        if (Mathf.Abs(currentValue - targetValue) > 0.01f)
        {
            currentValue = Mathf.Lerp(currentValue, targetValue, Time.deltaTime * lerpSpeed);
            if (fillImage != null) fillImage.fillAmount = currentValue / maxValue;
        }
    }

    public void AddValue(float amount)
    {
        targetValue = Mathf.Clamp(targetValue + amount, 0, maxValue);
    }

    public void SubtractValue(float amount)
    {
        targetValue = Mathf.Clamp(targetValue - amount, 0, maxValue);
    }
}