
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public bool introAlreadyPlayed = false; // Aquí guardamos si ya se vio la intro

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Esto hace que el objeto sobreviva entre escenas
        }
        else
        {
            Destroy(gameObject);
        }
    }
}