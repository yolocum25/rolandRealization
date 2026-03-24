using UnityEngine;
using TMPro;

public class CursedText : MonoBehaviour
{
    [Header("Shake configuration")]
    public float intensity = 5.0f; 
    public float speed = 20.0f; 

    private Vector3 originalPostion;

    void OnEnable()
    {
        
        originalPostion = transform.localPosition;
    }

    void Update()
    {
       
        if (GetComponent<TextMeshProUGUI>().color.a > 0.1f)
        {
            float x = Random.Range(-1f, 1f) * intensity;
            float y = Random.Range(-1f, 1f) * intensity;

            
            transform.localPosition = originalPostion + new Vector3(x, y, 0);
        }
        else
        {
            
            transform.localPosition = originalPostion;
        }
    }
}