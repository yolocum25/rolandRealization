
using UnityEngine;

public class SceneSkipManager : MonoBehaviour
{
    public static SceneSkipManager Instance;
    public bool introAlreadyPlayed = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); 
        }
        else
        {
            Destroy(gameObject);
        }
    }
}