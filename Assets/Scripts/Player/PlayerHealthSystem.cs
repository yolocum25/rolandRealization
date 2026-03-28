using UnityEngine.TextCore.Text;

namespace Player

{
    public class PlayerHealthSystem : charactersHealthSystem
    {
        public override void TakeDamage(float damage)
        {
            base.TakeDamage(damage);
            //avisa al EventManager
            EventManager.Instance.PlayerDamage(currentHealth, maxhealth);
        } 
    }
}
