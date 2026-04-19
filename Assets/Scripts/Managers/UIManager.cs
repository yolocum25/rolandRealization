using System;
using UnityEngine;
using Image = UnityEngine.UI.Image;
public class UIManager : MonoBehaviour
{
    [SerializeField] private Image healthBar;
   
    
    private void OnEnable()
    {
        EventManager.Instance.OnPlayerDamaged += UpdateHealthBar;
    }

    private void OnDisable()
    {
        EventManager.Instance.OnPlayerDamaged -= UpdateHealthBar;
    }
    
    public void UpdateHealthBar(float currentHealth, float maxhealth)
    {
        healthBar.fillAmount = currentHealth / maxhealth;
        healthBar.color = Color.Lerp(Color.red, Color.green, currentHealth/maxhealth);
        
    }
}