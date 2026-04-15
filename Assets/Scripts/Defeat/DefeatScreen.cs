using UnityEngine;
using TMPro;

public class DefeatScreen : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI reasonText;
    [SerializeField] private AudioSource defeatSound;

    public void SetupDefeat(string reason)
    {
        if (!gameObject.activeSelf) 
        {
            gameObject.SetActive(true);
        }

        if (reasonText != null) 
        {
            reasonText.text = reason;
        }

       
        if (defeatSound != null) 
        {
           
            if (!defeatSound.enabled) defeatSound.enabled = true;
            
            defeatSound.Play();
        }
    }
}