using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class IntroFadeDirect : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private RawImage introImage;

    [Header("Settings")]
    [SerializeField] private float displayDuration = 5f; 
    [SerializeField] private float fadeDuration = 2f;

    void Start()
    {
        if (introImage != null)
        {
           
            Color c = introImage.color;
            c.a = 1f;
            introImage.color = c;

            StartCoroutine(HandleIntro());
        }
    }

    private IEnumerator HandleIntro()
    {
        
        yield return new WaitForSeconds(displayDuration);

       
        float elapsedTime = 0f;
        Color tempColor = introImage.color;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            
            
            float newAlpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
            
            
            tempColor.a = newAlpha;
            introImage.color = tempColor;

            yield return null;
        }

       
        introImage.gameObject.SetActive(false);
    }
}