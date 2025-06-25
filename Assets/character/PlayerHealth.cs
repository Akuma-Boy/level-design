using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.SceneManagement; // Adicionado para gerenciamento de cenas

public class PlayerHealth : MonoBehaviour
{
    [Header("Configurações de Vida")]
    public int maxHealth = 100;
    public int currentHealth;
    public bool isInvincible = false;
    public float invincibilityTime = 1f;

    [Header("Eventos")]
    public UnityEvent OnDamageTaken;
    public UnityEvent OnDeath;
    public UnityEvent OnHealthChanged;

    [Header("Feedback de Dano")]
    public Image damageScreenEffect;
    public Color damageColor = new Color(1f, 0f, 0f, 0.5f);
    public float flashDuration = 0.2f;
    public CameraShake cameraShakeEffect;
    public float shakeIntensity = 0.5f;
    public float shakeDuration = 0.3f;

    [Header("Efeitos Sonoros")]
    public AudioClip damageSound;
    public AudioClip deathSound;
    [Range(0, 1)] public float volume = 0.7f;

    [Header("Configurações de Morte")]
    public bool resetPositionOnDeath = true;
    public Transform respawnPoint; // Ponto de respawn se resetPositionOnDeath for true
    public bool reloadSceneOnDeath = false;
    public float delayBeforeReset = 2f; // Tempo antes de reiniciar

    private float invincibilityTimer = 0f;
    private Color originalEffectColor;
    private AudioSource audioSource;
    private Vector3 initialPosition; // Posição inicial do jogador

    void Start()
    {
        currentHealth = maxHealth;
        initialPosition = transform.position; // Guarda a posição inicial
        OnHealthChanged.Invoke();

        if (damageScreenEffect != null)
        {
            originalEffectColor = damageScreenEffect.color;
            damageScreenEffect.color = Color.clear;
        }

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
        if (damageScreenEffect != null)
        {
            damageScreenEffect.color = damageColor;
            Invoke("ResetDamageEffect", flashDuration);
        }

        if (cameraShakeEffect != null)
        {
            cameraShakeEffect.ShakeCamera(shakeIntensity, shakeDuration);
        }

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

        if (deathSound != null)
        {
            audioSource.PlayOneShot(deathSound, volume);
        }

        if (damageScreenEffect != null)
        {
            damageScreenEffect.color = damageColor;
        }

        Debug.Log("Player morreu!");

        // Chama o respawn após o delay
        Invoke("Respawn", delayBeforeReset);
    }

    private void Respawn()
    {
        if (reloadSceneOnDeath)
        {
            // Recarrega a cena atual
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        else if (resetPositionOnDeath)
        {
            // Reseta a posição do jogador
            if (respawnPoint != null)
            {
                transform.position = respawnPoint.position;
            }
            else
            {
                transform.position = initialPosition;
            }

            // Reseta a vida
            currentHealth = maxHealth;
            OnHealthChanged.Invoke();

            // Reseta efeitos visuais
            if (damageScreenEffect != null)
            {
                damageScreenEffect.color = Color.clear;
            }
        }
    }

    public float GetHealthPercentage()
    {
        return (float)currentHealth / maxHealth;
    }
}