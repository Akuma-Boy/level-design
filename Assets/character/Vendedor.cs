using UnityEngine;
using UnityEngine.Events;

public class Vendedor : MonoBehaviour
{
    [Header("Configurações de Interação")]
    [SerializeField] private float interactionRange = 3f;
    [SerializeField] private KeyCode interactionKey = KeyCode.E;
    [SerializeField] private UnityEvent onInteract;

    [Header("Efeito de Respiração")]
    [SerializeField] private float breathingSpeed = 1f;
    [SerializeField] private float breathingAmount = 0.1f;
    [SerializeField] private Transform bodyToAnimate;

    [Header("Feedback Visual")]
    [SerializeField] private GameObject interactionPrompt; // UI "Pressione E"
    [SerializeField] private Material highlightMaterial; // Material quando jogador está perto
    [SerializeField] private float highlightIntensity = 1.5f;
    
    private Transform player;
    private Vector3 originalScale;
    private Material originalMaterial;
    private Renderer objectRenderer;
    private bool playerInRange = false;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        
        if (bodyToAnimate == null) bodyToAnimate = transform;
        originalScale = bodyToAnimate.localScale;

        // Configuração do sistema de highlight
        objectRenderer = GetComponent<Renderer>();
        if (objectRenderer != null)
        {
            originalMaterial = objectRenderer.material;
        }

        // Desativa o prompt inicialmente
        if (interactionPrompt != null)
        {
            interactionPrompt.SetActive(false);
        }
    }

    private void Update()
    {
        // Verifica distância do jogador
        float distance = Vector3.Distance(transform.position, player.position);
        bool wasInRange = playerInRange;
        playerInRange = distance <= interactionRange;

        // Atualiza feedback visual se o estado mudou
        if (playerInRange != wasInRange)
        {
            UpdateVisualFeedback();
        }

        // Efeito de respiração
        float breathingEffect = Mathf.Sin(Time.time * breathingSpeed) * breathingAmount;
        bodyToAnimate.localScale = originalScale + (Vector3.one * breathingEffect);

        // Verifica interação
        if (playerInRange && Input.GetKeyDown(interactionKey))
        {
            onInteract.Invoke();
        }
    }

    private void UpdateVisualFeedback()
    {
        // Ativa/desativa o prompt
        if (interactionPrompt != null)
        {
            interactionPrompt.SetActive(playerInRange);
        }

        // Aplica efeito de highlight
        if (objectRenderer != null)
        {
            if (playerInRange)
            {
                if (highlightMaterial != null)
                {
                    objectRenderer.material = highlightMaterial;
                }
                else
                {
                    // Efeito de highlight simples se não tiver material específico
                    objectRenderer.material.color = originalMaterial.color * highlightIntensity;
                }
            }
            else
            {
                objectRenderer.material = originalMaterial;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }
}