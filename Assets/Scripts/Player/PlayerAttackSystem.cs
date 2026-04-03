using UnityEngine;
using System.Collections.Generic;
public class PlayerAttackSystem : PlayerSystem
{
    #region AnimParameters

    private static readonly int Attack = Animator.StringToHash("attack");
    

    #endregion

    [SerializeField] private LayerMask whatIsDamageable;
    private float damage = 20;
    private bool attacking;
    private List<IDamageable> alreadyDamaged = new();
    [SerializeField] private float what_is_damageble;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
   
   
   
    void Update()
    {
        AttackAnim();
        if (attacking)
        {
            Collider2D result = Physics2D.OverlapCircle(main.InteractPoint.position, main.InteractionRadius,whatIsDamageable);

            if (result != null)
            {
                if (result.TryGetComponent(out IDamageable damageableElement)&& !alreadyDamaged.Contains(damageableElement))
                { 
                    damageableElement.TakeDamage(damage);
                    alreadyDamaged.Add(damageableElement);
                }
            }
        }
    }
    private void AttackAnim()
    {
        if (Input.GetMouseButtonDown(0))
        {
            main.Anim.SetTrigger((int)Attack);
            
            
                
            
            //1. Se lanza la animación
            //2. Lanzar Overlap en "Interaction point"
            //3. Preguntar si aquello detectado implementa la interfaz dañable
            //4. Hacer daño --> Implementa un nuevo método TakeDamage(float damage) en la interfaz
        }
    }
    //se ejecuta cuando el ataque llegue a su punto de impacto
   

    private void OpenAttackWindow()
    {
        attacking = true;
    }

    private void CloseAttackWindow()
    {
        attacking = false;
        alreadyDamaged.Clear();
    }
}