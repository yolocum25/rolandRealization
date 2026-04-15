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
            // Guardamos el índice actual para el click del botón
            int index = i;

            if (i == 0)
            {
                // El primer nivel de la lista siempre debe estar disponible
                chapters[i].isLocked = false;
            }
            else
            {
                // 1. Sacamos el nombre del nivel anterior de tu lista del Inspector
                string previousLevelName = chapters[i - 1].sceneName;

                // 2. Le preguntamos al Manager si ese nivel anterior está completado
                // Esta es la variable que "creamos" para guardar la respuesta (true/false)
                bool isPreviousCompleted = LevelCheckerManager.IsLevelUnlocked(previousLevelName);


                chapters[i].isLocked = !isPreviousCompleted;
            }

            // Aplicamos los cambios visuales (transparencias de candado, etc.)
            UpdateVisuals(chapters[i]);

            // Configuramos el botón para que cargue su escena al hacer click
           
            chapters[i].button.onClick.AddListener(() => LoadLevel(chapters[index]));
        }
    }

    void UpdateVisuals(LevelButton level)
    {
        // Si está bloqueado, desactivamos el objeto o el componente por completo
        level.button.interactable = !level.isLocked;
    
        if (level.isLocked)
        {
            level.lockVideo.canvasRenderer.SetAlpha(1.0f);
            level.unlockedImage.canvasRenderer.SetAlpha(0.0f);
            // Esto desactiva el componente para que no detecte ni el ratón
            level.button.enabled = false; 
        }
        else
        {
            level.lockVideo.canvasRenderer.SetAlpha(0.0f);
            level.unlockedImage.canvasRenderer.SetAlpha(1.0f);
            level.button.enabled = true;
        }
    }


    public IEnumerator UnlockSequence(int levelIndex)
    {
        if (levelIndex < 0 || levelIndex >= chapters.Length) yield break;

        LevelButton level = chapters[levelIndex];
        float timer = 0;

        level.isLocked = false;
        level.button.interactable = true;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float alpha = timer / fadeDuration;

            level.lockVideo.canvasRenderer.SetAlpha(1.0f - alpha);
            level.unlockedImage.canvasRenderer.SetAlpha(alpha);

            yield return null;
        }
    }







    public void LoadLevel(LevelButton level)
    {
        // LOG DE SEGURIDAD: Esto te dirá en la consola qué está pasando realmente
        Debug.Log($"Intentando cargar: {level.sceneName}. ¿Está bloqueado?: {level.isLocked}");

        // Si la variable dice que está bloqueado, CORTAMOS la ejecución aquí mismo
        if (level.isLocked) 
        {
            Debug.LogWarning("BLOQUEO ACTIVO: No se carga la escena.");
            return; 
        }

        SceneManager.LoadScene(level.sceneName);
    }
}