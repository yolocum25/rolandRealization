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
            // Nos aseguramos de que empiece con el alpha al máximo
            Color c = introImage.color;
            c.a = 1f;
            introImage.color = c;

            StartCoroutine(HandleIntro());
        }
    }

    private IEnumerator HandleIntro()
    {
        // 1. Espera activa de 5 segundos
        yield return new WaitForSeconds(displayDuration);

        // 2. Bucle de Fade Out
        float elapsedTime = 0f;
        Color tempColor = introImage.color;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            
            // Calculamos el nuevo alpha
            float newAlpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
            
            // Aplicamos solo el alpha manteniendo el color original
            tempColor.a = newAlpha;
            introImage.color = tempColor;

            yield return null;
        }

        // 3. Finalización: Desactivamos el objeto para que no estorbe
        introImage.gameObject.SetActive(false);
    }
}