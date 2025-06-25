using UnityEngine;
using System.Collections;

public class BossMageAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    [SerializeField] private GameObject magicProjectilePrefab;
    [SerializeField] private Transform[] projectileSpawnPoints;
    [SerializeField] private float attackCooldown = 2f;
    [SerializeField] private float projectileSpeed = 10f;
    
    [Header("Special Attack Settings")]
    [SerializeField] private GameObject lightningLinePrefab;
    [SerializeField] private float specialCooldown = 5f;
    [SerializeField] private int maxLines = 3;
    [SerializeField] private float lineSpawnRadius = 8f;
    [SerializeField] private float lineHeight = 20f;
    [SerializeField] private float lineDuration = 1.5f;
    
    private Animator animator;
    private Transform player;
    private bool canAttack = true;
    private bool canUseSpecial = true;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        
        if (animator == null)
        {
            Debug.LogError("Animator component not found on the boss!");
        }
    }

    private void Start()
    {
        FindPlayer();
    }

    private void FindPlayer()
    {
        // Busca o player com a tag correta "Player" (P maiúsculo)
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
        else
        {
            Debug.LogError("Player not found! Make sure your player has the tag 'Player' (with capital P)");
            // Tenta encontrar novamente após 1 segundo
            Invoke("FindPlayer", 1f);
        }
    }

    // Chamado pelo trigger "Attack" no Animator
    public void TriggerMagicAttack()
    {
        if (canAttack && player != null)
        {
            StartCoroutine(PerformMagicAttack());
        }
        else if (!canAttack)
        {
            Debug.Log("Attack on cooldown");
        }
        else if (player == null)
        {
            Debug.LogWarning("Player reference is null, trying to find player again");
            FindPlayer();
        }
    }

    // Chamado pelo trigger "Especial" no Animator
    public void TriggerSpecialAttack()
    {
        if (canUseSpecial && player != null)
        {
            StartCoroutine(PerformSpecialAttack());
        }
    }

    private IEnumerator PerformMagicAttack()
    {
        canAttack = false;
        
        // Dispara animação
        animator.SetTrigger("Attack");
        
        // Espera um pequeno delay para sincronizar com a animação
        yield return new WaitForSeconds(0.3f);
        
        // Verifica novamente se o player existe
        if (player == null) yield break;
        
        // Instancia projéteis em todos os pontos de spawn
        foreach (Transform spawnPoint in projectileSpawnPoints)
        {
            if (spawnPoint == null) continue;
            
            GameObject projectile = Instantiate(magicProjectilePrefab, spawnPoint.position, spawnPoint.rotation);
            Rigidbody rb = projectile.GetComponent<Rigidbody>();
            
            if (rb != null)
            {
                Vector3 direction = (player.position - spawnPoint.position).normalized;
                rb.linearVelocity = direction * projectileSpeed;
            }
            
            // Destrói o projétil após 5 segundos (caso não colida com nada)
            Destroy(projectile, 5f);
        }
        
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

    private IEnumerator PerformSpecialAttack()
    {
        canUseSpecial = false;
        
        // Dispara animação
        animator.SetTrigger("Especial");
        
        // Espera um pequeno delay para sincronizar com a animação
        yield return new WaitForSeconds(0.5f);
        
        // Verifica novamente se o player existe
        if (player == null) yield break;
        
        // Cria várias linhas de raio próximas ao jogador
        for (int i = 0; i < maxLines; i++)
        {
            Vector3 randomOffset = Random.insideUnitCircle * lineSpawnRadius;
            Vector3 spawnPosition = player.position + new Vector3(randomOffset.x, lineHeight, randomOffset.y);
            
            // Cria o raio
            GameObject lightningLine = Instantiate(lightningLinePrefab, spawnPosition, Quaternion.identity);
            
            // Configura o Rigidbody para cair
            Rigidbody rb = lightningLine.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.useGravity = true;
                // Adiciona força para baixo para melhor efeito
                rb.AddForce(Vector3.down * 10f, ForceMode.Impulse);
            }
            
            // Configura o sistema de partículas
            ParticleSystem ps = lightningLine.GetComponent<ParticleSystem>();
            if (ps != null)
            {
                ps.Play();
            }
            
            // Destrói após o tempo de duração
            Destroy(lightningLine, lineDuration);
            
            // Pequeno delay entre cada raio
            yield return new WaitForSeconds(0.3f);
        }
        
        yield return new WaitForSeconds(specialCooldown);
        canUseSpecial = true;
    }

    // Método para debug - mostra o alcance do ataque especial
    private void OnDrawGizmosSelected()
    {
        if (player != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(player.position, lineSpawnRadius);
        }
    }
}