using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI; // Para efeitos de UI

public class PlayerHealth : MonoBehaviour
{
    [Header("Configura��es de Vida")]
    public int maxHealth = 100;
    public int currentHealth;
    public bool isInvincible = false;
    public float invincibilityTime = 1f;

    [Header("Eventos")]
    public UnityEvent OnDamageTaken;
    public UnityEvent OnDeath;
    public UnityEvent OnHealthChanged;

    [Header("Feedback de Dano")]
    public Image damageScreenEffect; // Imagem de overlay para efeito de dano
    public Color damageColor = new Color(1f, 0f, 0f, 0.5f); // Cor do efeito (vermelho semi-transparente)
    public float flashDuration = 0.2f; // Dura��o do efeito piscante
    public CameraShake cameraShakeEffect; // Refer�ncia para efeito de tremer c�mera
    public float shakeIntensity = 0.5f;
    public float shakeDuration = 0.3f;

    [Header("Efeitos Sonoros")]
    public AudioClip damageSound;
    public AudioClip deathSound;
    [Range(0, 1)] public float volume = 0.7f;

    private float invincibilityTimer = 0f;
    private Color originalEffectColor;
    private AudioSource audioSource;

    void Start()
    {
        currentHealth = maxHealth;
        OnHealthChanged.Invoke();

        // Configura efeitos visuais
        if (damageScreenEffect != null)
        {
            originalEffectColor = damageScreenEffect.color;
            damageScreenEffect.color = Color.clear;
        }

        // Configura �udio
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }
    }

    void Update()
    {
        if (isInvincible)
        {
            invincibilityTimer += Time.deltaTime;
            if (invincibilityTimer >= invincibilityTime)
            {
                isInvincible = false;
                invincibilityTimer = 0f;

                // Resetar efeito visual se necess�rio
                if (damageScreenEffect != null)
                {
                    damageScreenEffect.color = Color.clear;
                }
            }
        }
    }

    public void TakeDamage(int damageAmount)
    {
        if (isInvincible || currentHealth <= 0) return;

        currentHealth -= damageAmount;
        OnHealthChanged.Invoke();
        OnDamageTaken.Invoke();

        // Efeitos de feedback
        PlayDamageEffects();

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            isInvincible = true;
        }
    }

    private void PlayDamageEffects()
    {
        // Efeito visual piscante
        if (damageScreenEffect != null)
        {
            damageScreenEffect.color = damageColor;
            Invoke("ResetDamageEffect", flashDuration);
        }

        // Tremer c�mera
        if (cameraShakeEffect != null)
        {
            cameraShakeEffect.ShakeCamera(shakeIntensity, shakeDuration);
        }

        // Som de dano
        if (damageSound != null)
        {
            audioSource.PlayOneShot(damageSound, volume);
        }
    }

    private void ResetDamageEffect()
    {
        if (damageScreenEffect != null)
        {
            damageScreenEffect.color = Color.Lerp(damageScreenEffect.color, Color.clear, 0.5f);
        }
    }

    public void Heal(int healAmount)
    {
        currentHealth = Mathf.Min(currentHealth + healAmount, maxHealth);
        OnHealthChanged.Invoke();
    }

    private void Die()
    {
        currentHealth = 0;
        OnDeath.Invoke();

        // Efeitos de morte
        if (deathSound != null)
        {
            audioSource.PlayOneShot(deathSound, volume);
        }

        if (damageScreenEffect != null)
        {
            damageScreenEffect.color = damageColor;
        }

        Debug.Log("Player morreu!");
        // Adicione aqui outras l�gicas de game over
    }

    public float GetHealthPercentage()
    {
        return (float)currentHealth / maxHealth;
    }
}


