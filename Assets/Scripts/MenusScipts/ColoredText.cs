using UnityEngine;
using TMPro; 
using UnityEngine.EventSystems; 

public class TextHoverColor : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private TextMeshProUGUI textMesh;
    private Color originalColor;

    [Header("ColorConfiguration")]
    public Color hoverColor = Color.red;
    public float transitionSpeed = 5f; 

    void Awake()
    {
        textMesh = GetComponent<TextMeshProUGUI>();
        originalColor = textMesh.color;
    }

   
    public void OnPointerEnter(PointerEventData eventData)
    {
        StopAllCoroutines();
        StartCoroutine(ChangeColor(hoverColor));
    }

  
    public void OnPointerExit(PointerEventData eventData)
    {
        StopAllCoroutines();
        StartCoroutine(ChangeColor(originalColor));
    }

    
    private System.Collections.IEnumerator ChangeColor(Color targetColor) 
    {
        Color currentColor = textMesh.color;
        float tick = 0f;

        while (tick < 1f)
        {
            tick += Time.deltaTime * transitionSpeed;
            textMesh.color = Color.Lerp(currentColor, targetColor, tick);
            yield return null;
        }
        
        textMesh.color = targetColor;
    }
}