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

    void Start()
    {
        TryAssignPlayer();

        if (enemyAnimator == null)
            enemyAnimator = GetComponent<Animator>();
    }

    void Update()
    {
        // Se perdeu referência ao jogador, tenta achar de novo
        if (player == null)
        {
            TryAssignPlayer();
            return;
        }

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer <= detectionRange)
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
        if (playerObj != null)
        {
            player = playerObj.transform;
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
        if (projectilePrefab == null || firePoint == null)
        {
            Debug.LogWarning("Prefab ou firePoint não atribuídos!", this);
            return;
        }

        // Dispara a animação de ataque
        if (enemyAnimator != null)
            enemyAnimator.SetTrigger(Attack);

        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        Rigidbody rb = projectile.GetComponent<Rigidbody>();

        if (rb != null && player != null)
        {
            Vector3 direction = (player.position - firePoint.position).normalized;
            rb.linearVelocity = direction * projectileSpeed;

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
    }
}
