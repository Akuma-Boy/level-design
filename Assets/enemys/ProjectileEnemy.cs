using UnityEngine;

public class ProjectileEnemy : MonoBehaviour
{
    [Header("Configurações de Tiro")]
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float projectileSpeed = 10f;
    public float fireRate = 1f;
    public float detectionRange = 10f;

    [Header("Referências")]
    public Animator enemyAnimator;

    private Transform player;
    private float nextFireTime = 0f;
    private static readonly int Attack = Animator.StringToHash("attack");
    private bool hasLineOfSight = false;

    void Start()
    {
        // Busca imediata do player
        TryAssignPlayer();

        if (enemyAnimator == null)
            enemyAnimator = GetComponent<Animator>();
    }

    void Update()
    {
        // Atualização mais robusta da referência ao player
        if (player == null || !player.gameObject.activeInHierarchy)
        {
            TryAssignPlayer();
            if (player == null) return;
        }

        // Verificação de linha de visada
        CheckLineOfSight();

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer <= detectionRange && hasLineOfSight)
        {
            AimAtPlayer();

            if (Time.time >= nextFireTime)
            {
                Shoot();
                nextFireTime = Time.time + 1f / fireRate;
            }
        }
    }

    void TryAssignPlayer()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null && playerObj.activeInHierarchy)
        {
            player = playerObj.transform;
        }
    }

    void CheckLineOfSight()
    {
        if (player == null) return;

        RaycastHit hit;
        Vector3 directionToPlayer = (player.position - transform.position).normalized;

        if (Physics.Raycast(transform.position, directionToPlayer, out hit, detectionRange))
        {
            hasLineOfSight = hit.transform.CompareTag("Player");
        }
        else
        {
            hasLineOfSight = false;
        }
    }

    void AimAtPlayer()
    {
        if (player == null) return;

        Vector3 direction = (player.position - transform.position).normalized;
        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Euler(0, lookRotation.eulerAngles.y, 0);
        }
    }

    void Shoot()
    {
        if (projectilePrefab == null || firePoint == null || player == null)
        {
            Debug.LogWarning("Componentes essenciais não atribuídos!", this);
            return;
        }

        // Dispara animação
        if (enemyAnimator != null && enemyAnimator.isActiveAndEnabled)
        {
            enemyAnimator.SetTrigger(Attack);
        }
        else
        {
            Debug.LogWarning("Animator não disponível!", this);
        }

        // Instancia projétil
        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        Rigidbody rb = projectile.GetComponent<Rigidbody>();

        if (rb != null)
        {
            Vector3 direction = (player.position - firePoint.position).normalized;
            rb.velocity = direction * projectileSpeed;

            Collider projectileCollider = projectile.GetComponent<Collider>();
            Collider enemyCollider = GetComponent<Collider>();
            if (projectileCollider != null && enemyCollider != null)
            {
                Physics.IgnoreCollision(projectileCollider, enemyCollider, true);
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        // Mostra linha de visada
        if (player != null)
        {
            Gizmos.color = hasLineOfSight ? Color.green : Color.yellow;
            Gizmos.DrawLine(transform.position, player.position);
        }
    }
}