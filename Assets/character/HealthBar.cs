using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [Header("Referências")]
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private Image healthBarFill;
    [SerializeField] private Image healthBarDamageEffect; // Opcional: efeito de dano atrasado
    [SerializeField] private Text healthText; // Opcional: texto com números

    [Header("Configurações")]
    [SerializeField] private float damageEffectSpeed = 1f; // Velocidade do efeito de dano atrasado
    [SerializeField] private Color normalColor = Color.green;
    [SerializeField] private Color lowHealthColor = Color.red;
    [SerializeField] private float lowHealthThreshold = 0.3f; // 30% de vida

    private float currentDisplayHealth = 1f;
    private float targetHealth = 1f;

    void Start()
    {
        if (playerHealth == null)
        {
            playerHealth = FindAnyObjectByType<PlayerHealth>();
        }

        if (playerHealth != null)
        {
            // Registrar para receber atualizações de vida
            playerHealth.OnHealthChanged.AddListener(UpdateHealthBar);
            currentDisplayHealth = playerHealth.GetHealthPercentage();
            targetHealth = currentDisplayHealth;
            UpdateVisuals();
        }
        else
        {
            Debug.LogError("HealthBar: PlayerHealth não encontrado!");
        }
    }

    void Update()
    {
        // Atualiza o efeito de dano atrasado (se existir)
        if (healthBarDamageEffect != null)
        {
            if (currentDisplayHealth != targetHealth)
            {
                currentDisplayHealth = Mathf.MoveTowards(currentDisplayHealth, targetHealth, damageEffectSpeed * Time.deltaTime);
                healthBarDamageEffect.fillAmount = currentDisplayHealth;
            }
        }
    }

    private void UpdateHealthBar()
    {
        if (playerHealth != null)
        {
            targetHealth = playerHealth.GetHealthPercentage();
            healthBarFill.fillAmount = targetHealth; // Atualiza imediatamente a barra principal

            // Atualiza cores baseado na vida atual
            UpdateVisuals();
        }
    }

    private void UpdateVisuals()
    {
        // Muda a cor quando a vida está baixa
        if (healthBarFill != null)
        {
            healthBarFill.color = targetHealth <= lowHealthThreshold ? lowHealthColor : normalColor;
        }

        // Atualiza texto se existir
        if (healthText != null && playerHealth != null)
        {
            healthText.text = $"{playerHealth.currentHealth}/{playerHealth.maxHealth}";
        }
    }

    // Opcional: reset quando o jogador é curado ou revive
    public void ResetHealthBar()
    {
        if (playerHealth != null)
        {
            targetHealth = playerHealth.GetHealthPercentage();
            currentDisplayHealth = targetHealth;
            healthBarFill.fillAmount = targetHealth;

            if (healthBarDamageEffect != null)
            {
                healthBarDamageEffect.fillAmount = targetHealth;
            }

            UpdateVisuals();
        }
    }
}