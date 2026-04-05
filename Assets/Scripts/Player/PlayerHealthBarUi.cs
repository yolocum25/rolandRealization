using UnityEngine;
using UnityEngine.UI; 

public class PlayerHealthBarUI : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private charactersHealthSystem targetHealthSystem;
    [SerializeField] private Image healthBarFill; // La imagen de la barra (tipo Filled)

    private void OnEnable()
    {
      
        if (targetHealthSystem != null)
        {
            targetHealthSystem.OnDamaged += UpdateHealthBar;
        }
    }

    private void OnDisable()
    {
        
        if (targetHealthSystem != null)
        {
            targetHealthSystem.OnDamaged -= UpdateHealthBar;
        }
    }

    private void UpdateHealthBar()
    {
        
        float current = targetHealthSystem.GetCurrentHealth();
        float max = targetHealthSystem.GetMaxHealth();

        
        healthBarFill.fillAmount = current / max;
    }
}