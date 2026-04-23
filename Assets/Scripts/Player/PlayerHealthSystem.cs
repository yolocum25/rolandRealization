using UnityEngine.TextCore.Text;

namespace Player

{
    public class PlayerHealthSystem : CharactersHealthSystem
    {
        public override void TakeDamage(float damage)
        {
            if (isInvulnerable) return;
            base.TakeDamage(damage);

            EventManager.Instance.PlayerDamage(currentHealth, maxhealth);


            if (EmotionManager.Instance != null)
            {
                EmotionManager.Instance.GainNegative(damage * 0.5f);

            }
        }
        protected override void Die()
        {
           
            if (EventManager.Instance != null)
            {
                EventManager.Instance.CharacterDead(this.gameObject);
            }

            
            if (gameObject.CompareTag("Player"))
            {
                EventManager.Instance.GameOver();
            }
            
            CharacterDeathVisuals visuals = GetComponent<CharacterDeathVisuals>();
            if (visuals != null)
            {
                visuals.HandleDeath(this.gameObject);
            }
            
            
            if (TryGetComponent(out PlayerAttackSystem attack))
            {
                attack.enabled = false;
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
