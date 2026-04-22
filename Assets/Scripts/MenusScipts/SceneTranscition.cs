
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class SceneTransction : MonoBehaviour
{
    [Header("start background")]
    public Image blackbackground; 

    [Header("Texts")]
    public TextMeshProUGUI text1_P1; public TextMeshProUGUI text2_P1;
    public TextMeshProUGUI text1_P2; public TextMeshProUGUI text2_P2;

    [Header("Menu Elements")]
    public RawImage backgroundMenu;
    public TextMeshProUGUI GameTittle;
    public List<Image> buttonImage;
    public List<TextMeshProUGUI> buttonText;

    [Header("Configuración")]
    public float fadeSpeed = 1.0f;
    public float waitToLecture = 2.0f;

    void Start()
    {
         
        PrepareCanvas();

        
        bool yaSeVio = false;
        if (SceneSkipManager.Instance != null && SceneSkipManager.Instance.introAlreadyPlayed)
        {
            yaSeVio = true;
        }

      
        if (yaSeVio)
        {
            
            SkipIntroAndShowMenu();
        }
        else
        {
            
            StartCoroutine(FullSequence());
        }
    }

    IEnumerator FullSequence()
    {
        
        yield return StartCoroutine(introScene());
        
       
        if (SceneSkipManager.Instance != null) SceneSkipManager.Instance.introAlreadyPlayed = true;

       
        yield return StartCoroutine(MenuScene());
    }

    void SkipIntroAndShowMenu()
    {
       
        blackbackground.gameObject.SetActive(false);
        SetAlpha(text1_P1, 0); SetAlpha(text2_P1, 0);
        SetAlpha(text1_P2, 0); SetAlpha(text2_P2, 0);

       
        backgroundMenu.gameObject.SetActive(true);
        SetAlpha(backgroundMenu, 1);

        
        StartCoroutine(MenuScene());
    }

    IEnumerator introScene()
    {
        
        blackbackground.gameObject.SetActive(true);
        yield return StartCoroutine(Fade(blackbackground, 0, 1)); 
        yield return StartCoroutine(Fade(text1_P1, 0, 1));
        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(Fade(text2_P1, 0, 1));
        yield return new WaitForSeconds(waitToLecture);

        StartCoroutine(Fade(text1_P1, 1, 0));
        yield return StartCoroutine(Fade(text2_P1, 1, 0));

        yield return StartCoroutine(Fade(text1_P2, 0, 1));
        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(Fade(text2_P2, 0, 1));
        yield return new WaitForSeconds(waitToLecture);

        StartCoroutine(Fade(text1_P2, 1, 0));
        yield return StartCoroutine(Fade(text2_P2, 1, 0));
        
        blackbackground.gameObject.SetActive(false);
    }

    IEnumerator MenuScene()
    {
        
        SetAlpha(backgroundMenu, 1);
        yield return StartCoroutine(Fade(GameTittle, 0, 1));

        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime * fadeSpeed;
            foreach (Image img in buttonImage) SetAlpha(img, t);
            foreach (TextMeshProUGUI txt in buttonText) SetAlpha(txt, t);
            yield return null;
        }

        foreach (Image img in buttonImage) img.raycastTarget = true;
    }
    
    void PrepareCanvas()
    {
        SetAlpha(blackbackground, 0);
        SetAlpha(text1_P1, 0); SetAlpha(text2_P1, 0);
        SetAlpha(text1_P2, 0); SetAlpha(text2_P2, 0);
        SetAlpha(backgroundMenu, 0);
        SetAlpha(GameTittle, 0);
        foreach (Image img in buttonImage) { SetAlpha(img, 0); img.raycastTarget = false; }
        foreach (TextMeshProUGUI txt in buttonText) SetAlpha(txt, 0);
    }

    IEnumerator Fade(Graphic Element, float start, float fin)
    {
        if (Element == null) yield break;
        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime * fadeSpeed;
            SetAlpha(Element, Mathf.Lerp(start, fin, t));
            yield return null;
        }
        SetAlpha(Element, fin);
    }

    void SetAlpha(Graphic g, float a)
    {
        if (g == null) return;
        Color c = g.color;
        g.color = new Color(c.r, c.g, c.b, a);
    }
}