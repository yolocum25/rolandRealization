using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class LevelSelector : MonoBehaviour
{
    [System.Serializable]
    public class LevelButton
    {
        public string sceneName;
        public Button button;
        public RawImage lockVideo; 
        public RawImage unlockedImage; 
        public bool isLocked = true;
    }

    public LevelButton[] chapters; 
    public float fadeDuration = 1.0f;

    void Start()
    {
        SetupButtons();
    }

    void SetupButtons()
    {
        for (int i = 0; i < chapters.Length; i++)
        {
            int index = i; 
            
            if (i == 0) 
            {
                chapters[i].isLocked = false;
            }
            else
            {
                string previousLevelName = chapters[i-1].sceneName;
                chapters[i].isLocked = !LevelCheckerManager.IsLevelUnlocked(previousLevelName);
            }
            

            UpdateVisuals(chapters[i]);
            
            chapters[i].button.onClick.AddListener(() => LoadLevel(chapters[index]));
        }
    }

    void UpdateVisuals(LevelButton level)
    {
        if (level.isLocked)
        {
            level.lockVideo.canvasRenderer.SetAlpha(1.0f);
            level.unlockedImage.canvasRenderer.SetAlpha(0.0f);
            level.button.interactable = false;
        }
        else
        {
            level.lockVideo.canvasRenderer.SetAlpha(0.0f);
            level.unlockedImage.canvasRenderer.SetAlpha(1.0f);
            level.button.interactable = true;
        }
    }

   
    public IEnumerator UnlockSequence(int levelIndex)
    {
        LevelButton level = chapters[levelIndex];
        float timer = 0;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float alpha = timer / fadeDuration;

            
            level.lockVideo.canvasRenderer.SetAlpha(1.0f - alpha);
            level.unlockedImage.canvasRenderer.SetAlpha(alpha);

            yield return null;
        }

        level.isLocked = false;
        level.button.interactable = true;
    }
    
    
    
    
    
    

    public void LoadLevel(LevelButton level)
    {
        if (!level.isLocked)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(level.sceneName);
        }
    }
}
