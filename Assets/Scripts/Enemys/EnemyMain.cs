using System;
using UnityEngine;

public class EnemyMain : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.TryGetComponent(out IDamageable damageable))
        {
            damageable.TakeDamage(20);
        }
    }
}