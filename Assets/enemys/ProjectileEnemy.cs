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
    public Transform player;
    public Animator enemyAnimator;

    private float nextFireTime = 0f;
    private static readonly int Attack = Animator.StringToHash("attack");

    void Start()
    {
        FindPlayer();

        if (enemyAnimator == null)
        {
            enemyAnimator = GetComponent<Animator>();
        }
    }

    void Update()
    {
        if (player == null)
        {
            FindPlayer();
            if (player == null) return;
        }

        if (Vector3.Distance(transform.position, player.position) <= detectionRange)
        {
            AimAtPlayer();

            if (Time.time >= nextFireTime)
            {
                Shoot();
                nextFireTime = Time.time + 1f / fireRate;
            }
        }
    }

    void FindPlayer()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) player = playerObj.transform;
    }

    void AimAtPlayer()
    {
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
        {
            enemyAnimator.SetTrigger(Attack);
        }

        // Instancia o projétil
        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        Rigidbody rb = projectile.GetComponent<Rigidbody>();

        if (rb != null)
        {
            Vector3 direction = (player.position - firePoint.position).normalized;

            // CORREÇÃO: Usando linearVelocity em vez de velocity
            rb.linearVelocity = direction * projectileSpeed;

            // Ignora colisão inicial com o inimigo
            Collider projectileCollider = projectile.GetComponent<Collider>();
            Collider enemyCollider = GetComponent<Collider>();
            if (projectileCollider != null && enemyCollider != null)
            {
                Physics.IgnoreCollision(projectileCollider, enemyCollider, true);
            }
        }
        else
        {
            Debug.LogWarning("Projétil não tem componente Rigidbody!", projectile);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}