using Player;
using UnityEngine;
using UnityEngine.UI; 

public class PlayerHealthBarUI : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private CharactersHealthSystem targetHealthSystem;
    [SerializeField] private Image healthBarFill; 

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
    private void Awake()
    {
        
        if (targetHealthSystem == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                targetHealthSystem = player.GetComponent<CharactersHealthSystem>();
            }
        }
    }
    
}