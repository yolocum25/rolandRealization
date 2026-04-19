using UnityEngine;
using System.Collections;

public class CharacterDeathVisuals : MonoBehaviour
{
    [Header("Referencias")]
    public Animator anim;
    public SpriteRenderer spriteRenderer;
    public ParticleSystem deathParticles;

    [Header("confi")]
    public float delayBeforeFade = 1.5f;
    public float fadeDuration = 1.0f;
    public string deathTriggerName = "Dead";
    
    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip deathSound;
    [SerializeField] private float volume = 1f;

    private bool isDead = false;

    void Awake()
    {
        if (anim == null) anim = GetComponent<Animator>();
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        
        if (EventManager.Instance != null)
        {
            EventManager.Instance.OnCharacterDead += HandleDeath;
        }
    }

    private void OnDestroy()
    {
      
        if (EventManager.Instance != null)
        {
            EventManager.Instance.OnCharacterDead -= HandleDeath;
        }
    }

   
    public void HandleDeath(GameObject deadCharacter)
    {
        if (deadCharacter != this.gameObject || isDead) return;

        isDead = true;
        StartCoroutine(DeathSequence());
    }

    private IEnumerator DeathSequence()
    {
        DisablePhysicsAndLogic();
        
        if (audioSource != null && deathSound != null)
        {
            audioSource.PlayOneShot(deathSound, volume);
        }
        
        if (anim != null)
        {
            anim.SetTrigger(deathTriggerName);
        }
        
        if (deathParticles != null)
        {
           
           
            Vector3 spawnPos = transform.position + new Vector3(0, 0, -1);
            ParticleSystem effect = Instantiate(deathParticles, spawnPos, Quaternion.identity);
        
           
            var pRenderer = effect.GetComponent<ParticleSystemRenderer>();
            if (pRenderer != null && spriteRenderer != null)
            {
                pRenderer.sortingLayerID = spriteRenderer.sortingLayerID;
                pRenderer.sortingOrder = spriteRenderer.sortingOrder + 1;
            }

            effect.Play();
        
            
            Destroy(effect.gameObject, 2f); 
        }
        yield return new WaitForSeconds(delayBeforeFade);
      
        if (spriteRenderer != null)
        {
            float timer = 0;
            Color startColor = spriteRenderer.color;

            while (timer < fadeDuration)
            {
                timer += Time.deltaTime;
                float alpha = Mathf.Lerp(1f, 0f, timer / fadeDuration);
                spriteRenderer.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
                yield return null;
            }
        }

        Destroy(gameObject);
    }

    private void DisablePhysicsAndLogic()
    {
       
        Collider2D[] colls = GetComponentsInChildren<Collider2D>();
        foreach (var c in colls) c.enabled = false;

       
        if (TryGetComponent(out Rigidbody2D rb))
        {
            rb.linearVelocity = Vector2.zero;
            rb.gravityScale = 0;
            rb.constraints = RigidbodyConstraints2D.FreezeAll;
        }

        
        MonoBehaviour[] scripts = GetComponents<MonoBehaviour>();
        foreach (var s in scripts)
        {
            
            if (s != this && !(s is SpriteRenderer) && !(s is Animator))
            {
                s.enabled = false;
            }
        }
    }
    
}