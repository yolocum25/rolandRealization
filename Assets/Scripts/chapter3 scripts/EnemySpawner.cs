using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    [Header("Confi Spawn")]
    [SerializeField] private GameObject enemyPrefab;    
    [SerializeField] private float spawnInterval = 10f;  
    [SerializeField] private Transform[] spawnPoints;   
    [SerializeField] private SurvivalTimer survivalTimer;

    [Header("Limit")]
    [SerializeField] private int maxEnemiesAtOnce = 4; 
    
    private bool canSpawn = true;

    private void Start()
    {
        
        StartCoroutine(SpawnRoutine());
    }

    private IEnumerator SpawnRoutine()
    {
      
        while (survivalTimer == null || !survivalTimer.IsRunning())
        {
            yield return null; 
        }
        
        
        while (canSpawn) 
        {
            if (CountActiveEnemies() < maxEnemiesAtOnce)
            {
                SpawnEnemy();
            }
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void SpawnEnemy()
    {
        if (spawnPoints.Length == 0) return;

       
        int randomIndex = Random.Range(0, spawnPoints.Length);
        Transform selectedPoint = spawnPoints[randomIndex];

        
        Instantiate(enemyPrefab, selectedPoint.position, Quaternion.identity);
        
    }

    private int CountActiveEnemies()
    {
        return GameObject.FindGameObjectsWithTag("Enemy").Length;
    }

  
    public void StopSpawning()
    {
        canSpawn = false;
    }
}