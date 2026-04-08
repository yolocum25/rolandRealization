using UnityEngine;
using UnityEngine.UI;

public class EmotionBar : MonoBehaviour
{
    [SerializeField] private Image fillImage; 
    public float maxValue = 100f;
    private float currentValue = 0f;
    
    [Header("Drain Settings")]
    [SerializeField] private float drainSpeed = 2f;    
    [SerializeField] private float idleDelay = 10f;
    private float lastIncreaseTime;
    
    [SerializeField] private float lerpSpeed = 5f;
    
    [Header("Audio Feedback")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip halfFullClip;
    private bool playedHalfwaySound = false;
   
    public float FillPercentage => currentValue / maxValue;

    void Update()
    {
        if (Time.time > lastIncreaseTime + idleDelay)
        {
            currentValue = Mathf.MoveTowards(currentValue, 0, drainSpeed * Time.deltaTime);
            
        }
        if (fillImage != null)
        {
            fillImage.fillAmount = Mathf.Lerp(fillImage.fillAmount, FillPercentage, Time.deltaTime * lerpSpeed);
            if (FillPercentage >= 0.5f)
            {
                if (!playedHalfwaySound)
                {
                    PlayHalfwaySound();
                    playedHalfwaySound = true;
                }
            }
            else
            {
               
                playedHalfwaySound = false;
            }
        }
       
        if (Time.time > lastIncreaseTime + idleDelay && currentValue > 0)
        {
            
            fillImage.color = new Color(fillImage.color.r, fillImage.color.g, fillImage.color.b, 0.7f + Mathf.PingPong(Time.time, 0.3f));
        }
        else
        {
            fillImage.color = new Color(fillImage.color.r, fillImage.color.g, fillImage.color.b, 1f);
        }
        
        
    }

    public void AddValue(float amount)
    {
        currentValue = Mathf.Clamp(currentValue + amount, 0, maxValue);
        lastIncreaseTime = Time.time;
    }
    
    
    private void PlayHalfwaySound()
    {
        if (audioSource != null && halfFullClip != null)
        {
            audioSource.PlayOneShot(halfFullClip);
        }
    }
}