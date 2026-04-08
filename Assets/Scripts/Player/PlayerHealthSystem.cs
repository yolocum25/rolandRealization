using UnityEngine.TextCore.Text;

namespace Player

{
    public class PlayerHealthSystem : charactersHealthSystem
    {
        public override void TakeDamage(float damage)
        {
            base.TakeDamage(damage);

            EventManager.Instance.PlayerDamage(currentHealth, maxhealth);


            if (EmotionManager.Instance != null)
            {
                EmotionManager.Instance.GainNegative(damage * 0.5f);

            }
        }


        protected override void Awake()
        {
            base.Awake();
            OnStagger += HandleStaggerEmotion;
        }

        
        private void HandleStaggerEmotion()
        {
           
            if (EmotionManager.Instance != null)
            {
                EmotionManager.Instance.GainNegative(25f); 
            }
        }
        
        private void OnDestroy()
        {
            
            OnStagger -= HandleStaggerEmotion;
        }
    }
}
