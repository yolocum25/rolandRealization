using System.Collections.Generic;
using UnityEngine;

public class BulletPoolManager : MonoBehaviour
{
    public static BulletPoolManager Instance; // Para que el enemigo lo encuentre fácil

    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private int poolSize = 10; // Cuántas balas quieres tener listas
    private List<GameObject> pooledBullets;

    private void Awake()
    {
        Instance = this;
        // Llenamos la caja de balas al empezar
        pooledBullets = new List<GameObject>();
        for (int i = 0; i < poolSize; i++)
        {
            GameObject bullet = Instantiate(bulletPrefab);
            bullet.SetActive(false); // Las guardamos apagadas
            pooledBullets.Add(bullet);
        }
    }

    public GameObject GetBullet()
    {
        // Buscamos una bala que no se esté usando
        for (int i = 0; i < pooledBullets.Count; i++)
        {
            if (!pooledBullets[i].activeInHierarchy)
            {
                return pooledBullets[i];
            }
        }

        // Opcional: Si se acaban, creamos una nueva para no quedarnos cortos
        GameObject newBullet = Instantiate(bulletPrefab);
        newBullet.SetActive(false);
        pooledBullets.Add(newBullet);
        return newBullet;
    }
}