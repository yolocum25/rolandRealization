using System.Collections.Generic;
using UnityEngine;

public class PlayerSlashDash : MonoBehaviour
{
    #region AnimParameters
    private static readonly int AttackTrigger = Animator.StringToHash("SlashDash");
    #endregion
    
    [Header("Settings")]
    [SerializeField] private LayerMask whatIsDamageable;
    [SerializeField] private Transform attackPoint; 
    [SerializeField] private float baseAttackRadius = 4;
    [SerializeField] private float baseDamage = 60f;
    
    private Animator anim;
    private AudioSource audioSource;
    
    private List<IDamageable> alreadyDamaged = new();
    
    void Awake()
    {
        anim = GetComponent<Animator>();
    }
    
    
}
