using System.Collections.Generic;
using UnityEngine;

public class BulletPoolManager : MonoBehaviour
{
    public static BulletPoolManager Instance;

    [Header("Configuración del Pool")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private int poolSize = 20;
    
    private List<GameObject> bulletPool;

    void Awake()
    {
        // Singleton para acceder desde cualquier script
        Instance = this;
        
        // Inicializar el pool
        bulletPool = new List<GameObject>();
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(bulletPrefab);
            obj.SetActive(false); // Las balas empiezan "apagadas"
            bulletPool.Add(obj);
        }
    }

    public GameObject GetBullet()
    {
        // Buscamos una bala que no esté activa
        foreach (GameObject bullet in bulletPool)
        {
            if (!bullet.activeInHierarchy)
            {
                return bullet;
            }
        }

        // Opcional: Si se acaban, crear una nueva (expansión dinámica)
        GameObject newObj = Instantiate(bulletPrefab);
        newObj.SetActive(false);
        bulletPool.Add(newObj);
        return newObj;
    }
}