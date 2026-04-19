using UnityEngine;

public class SorrownessManager : MonoBehaviour
{
    public static SorrownessManager Instance;

    [Header("UI Reference")]
    public SorrownessBar sorrowBar; 

    [Header("Balance Settings")]
    [SerializeField] private float passiveGainRate = 1f;    
    [SerializeField] private float gainOnHitGiven = 2f;    
    [SerializeField] private float gainOnHitReceived = 5f; 

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Update()
    {
        
        if (sorrowBar != null && sorrowBar.CurrentValue < sorrowBar.MaxValue)
        {
            sorrowBar.AddValue(passiveGainRate * Time.deltaTime);
        }
    }

    
    public void PlayerDealtDamage() => sorrowBar.AddValue(gainOnHitGiven);
    public void PlayerTookDamage() => sorrowBar.AddValue(gainOnHitReceived);
    
    public float GetSorrowLevel() => sorrowBar != null ? sorrowBar.FillPercentage : 0f;

    public void ResetSorrow() => sorrowBar.SubtractValue(sorrowBar.CurrentValue);
}