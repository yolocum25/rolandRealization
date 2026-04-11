using UnityEngine;
using TMPro;

public class DefeatScreen : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI reasonText;
    [SerializeField] private AudioSource defeatSound;

    public void SetupDefeat(string reason)
    {
        if (reasonText != null) reasonText.text = reason;
        if (defeatSound != null) defeatSound.Play();
    }
}