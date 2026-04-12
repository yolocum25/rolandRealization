using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float lifeTime = 2f;

    private void OnEnable()
    {
        // Al activarse, se apagará sola después de X segundos
        Invoke("Deactivate", lifeTime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Si choca contra algo, se apaga inmediatamente
        Deactivate();
    }

    void Deactivate()
    {
        CancelInvoke(); // Limpiamos el timer para que no se raye
        gameObject.SetActive(false); // Vuelve a la "caja"
    }

    private void OnDisable()
    {
        // Nos aseguramos de limpiar el timer si se apaga por cualquier motivo
        CancelInvoke();
    }
}