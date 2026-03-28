using System;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [field: SerializeField] private static Vector3 startPosition = new Vector3(4.03f,-0.98f,0);
    public Vector3 SavedPosition { get; private set; } = startPosition;
    public Vector3 SavedOrientation { get; private set; }
    public PlayerData SavedData { get; private set; }
    public static GameManager Instance { get; private set; }

    private void Awake()
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

    public void LoadNewScene(Vector3 targetPosition, Vector3 targetOrientation, int targetSceneIndex)
    {
        SavedPosition = targetPosition;
        SavedOrientation = targetOrientation;
        SceneManager.LoadScene(targetSceneIndex);
       
    }
}