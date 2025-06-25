using UnityEngine;

public class LightningDamage : MonoBehaviour
{
    [Header("Configurações de Dano")]
    [SerializeField] private int damageAmount = 15;
    [SerializeField] private float damageCooldown = 0.5f; // Tempo entre danos consecutivos
    [SerializeField] private string playerTag = "Player";
    
    [Header("Efeitos")]
    [SerializeField] private GameObject impactEffect;
    [SerializeField] private AudioClip impactSound;
    [Range(0, 1)] public float volume = 0.7f;
    
    private float lastDamageTime;
    private AudioSource audioSource;

    private void Awake()
    {
        // Configura o AudioSource
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            TryDamagePlayer(other.gameObject);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            TryDamagePlayer(other.gameObject);
        }
    }

    private void TryDamagePlayer(GameObject player)
    {
        // Verifica se já pode causar dano novamente
        if (Time.time - lastDamageTime < damageCooldown) return;
        
        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(damageAmount);
            lastDamageTime = Time.time;
            
            // Play impact effects
            PlayImpactEffects();
        }
    }

    private void PlayImpactEffects()
    {
        // Efeito visual de impacto
        if (impactEffect != null)
        {
            Instantiate(impactEffect, transform.position, Quaternion.identity);
        }
        
        // Som de impacto
        if (impactSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(impactSound, volume);
        }
    }
}