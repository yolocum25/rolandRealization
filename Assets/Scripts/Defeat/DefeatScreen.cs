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

        // 2. Comprobamos que el AudioSource no sea nulo y esté habilitado
        if (defeatSound != null) 
        {
            // Si el componente AudioSource mismo está desmarcado, lo habilitamos
            if (!defeatSound.enabled) defeatSound.enabled = true;
            
            defeatSound.Play();
        }
    }
}