using UnityEngine;

public class EmotionManager : MonoBehaviour
{
    public static EmotionManager Instance;

    public EmotionBar positiveBar;
    public EmotionBar negativeBar;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void GainPositive(float amt) => positiveBar.AddValue(amt);
    public void GainNegative(float amt) => negativeBar.AddValue(amt);
}