using UnityEngine;
using System.Collections;

public class EnemySpawnerV2 : MonoBehaviour
{
    [Header("Confi Spawn")] [SerializeField]
    private GameObject enemyPrefab;

    [SerializeField] private float spawnInterval = 10f;
    [SerializeField] private Transform[] spawnPoints;


    [Header("Intro Settings")] [SerializeField]
    private float delayAfterIntro = 5f; 

    [Header("Limit")] [SerializeField] private int maxEnemiesAtOnce = 4;

    private bool canSpawn = true;

    private void Start()
    {
        
        StartCoroutine(SpawnRoutine());
    }

    private IEnumerator SpawnRoutine()
    {
       
       
        yield return new WaitForSeconds(delayAfterIntro);


      
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
        // Si canSpawn es false o el prefab es nulo, salimos antes de intentar el Instantiate
        if (!canSpawn || enemyPrefab == null) return; 

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
        StopAllCoroutines();
    }

   
}
