using UnityEngine;

public class ProjectileEnemy : MonoBehaviour
{
    [Header("Configurações de Tiro")]
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float projectileSpeed = 10f;
    public float fireRate = 1f;
    public float detectionRange = 15f;
    public float attackAngleThreshold = 45f;

    [Header("Referências")]
    public Animator enemyAnimator;
    public LayerMask obstacleLayers = ~0;

    // Debug
    [Header("Debug")]
    [SerializeField] private string debugStatus = "Iniciando...";
    [SerializeField] private float lastDistanceToPlayer;
    [SerializeField] private float lastAngleToPlayer;
    [SerializeField] private bool debugHasLineOfSight;

    private Transform player;
    private float nextFireTime = 0f;
    private static readonly int Attack = Animator.StringToHash("attack");

    void Awake()
    {
        if (enemyAnimator == null)
            enemyAnimator = GetComponentInChildren<Animator>(true);
    }

    void Start()
    {
        FindPlayer();
        if (firePoint == null) firePoint = transform;
    }

    void Update()
    {
        if (player == null)
        {
            FindPlayer();
            debugStatus = "Procurando player...";
            return;
        }

        UpdatePlayerDetection();

        if (CanAttack())
        {
            debugStatus = "Atacando!";
            AimAtPlayer();
            TryShoot();
        }
        else
        {
            debugStatus = $"Player detectado mas não pode atacar\nDistância: {lastDistanceToPlayer:F1}\nÂngulo: {lastAngleToPlayer:F1}°\nLinha de visão: {debugHasLineOfSight}";
        }
    }

    void FindPlayer()
    {
        var playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
            Debug.Log($"Player encontrado: {player.name}", this);
        }
        else
        {
            Debug.LogWarning("Player não encontrado!", this);
            Invoke(nameof(FindPlayer), 0.5f);
        }
    }

    void UpdatePlayerDetection()
    {
        lastDistanceToPlayer = Vector3.Distance(transform.position, player.position);
        debugHasLineOfSight = CheckLineOfSight();

        Debug.DrawLine(firePoint.position, player.position,
                      debugHasLineOfSight ? Color.green : Color.red, 0.1f);
    }

    bool CheckLineOfSight()
    {
        Vector3 direction = player.position - firePoint.position;
        RaycastHit hit;

        if (Physics.Raycast(firePoint.position, direction, out hit, detectionRange, obstacleLayers))
        {
            bool canSee = hit.transform.CompareTag("Player");
            if (!canSee) Debug.Log($"Obstáculo detectado: {hit.transform.name}", this);
            return canSee;
        }
        return false;
    }

    bool CanAttack()
    {
        if (player == null) return false;

        Vector3 direction = (player.position - transform.position).normalized;
        lastAngleToPlayer = Vector3.Angle(transform.forward, direction);

        bool inRange = lastDistanceToPlayer <= detectionRange;
        bool goodAngle = lastAngleToPlayer <= attackAngleThreshold;

        return inRange && goodAngle && debugHasLineOfSight;
    }

    void AimAtPlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        direction.y = 0;

        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                Quaternion.LookRotation(direction),
                Time.deltaTime * 10f
            );
        }
    }

    void TryShoot()
    {
        if (Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + 1f / fireRate;
        }
    }

    void Shoot()
    {
        if (projectilePrefab == null || firePoint == null || player == null)
        {
            Debug.LogWarning("Componentes faltando para atirar", this);
            return;
        }

        // Animação
        if (enemyAnimator != null && enemyAnimator.isActiveAndEnabled)
        {
            enemyAnimator.SetTrigger(Attack);
            Debug.Log("Animação de ataque disparada", this);
        }
        else
        {
            Debug.LogWarning("Animator não disponível!", this);
        }

        // Projétil
        Vector3 shootDirection = (player.position - firePoint.position).normalized;
        GameObject projectile = Instantiate(
            projectilePrefab,
            firePoint.position,
            Quaternion.LookRotation(shootDirection)
        );

        // Física
        if (projectile.TryGetComponent<Rigidbody>(out var rb))
        {
            rb.velocity = shootDirection * projectileSpeed;

            if (TryGetComponent<Collider>(out var enemyCollider))
            {
                foreach (var col in projectile.GetComponentsInChildren<Collider>())
                {
                    Physics.IgnoreCollision(enemyCollider, col);
                }
            }
        }
    }

    void OnGUI()
    {
        if (Debug.isDebugBuild)
        {
            GUI.color = Color.red;
            GUI.Label(new Rect(10, 100, 500, 200), $"STATUS INIMIGO: {debugStatus}");
        }
    }

    void OnDrawGizmosSelected()
    {
        // Alcance
        Gizmos.color = new Color(1, 0, 0, 0.2f);
        Gizmos.DrawSphere(transform.position, detectionRange);

        // Direção de ataque
        if (player != null)
        {
            Gizmos.color = debugHasLineOfSight ? Color.green : Color.yellow;
            Gizmos.DrawLine(firePoint.position, player.position);

            // Ângulo de ataque
            Gizmos.color = Color.blue;
            Vector3 leftBound = Quaternion.Euler(0, -attackAngleThreshold, 0) * transform.forward * detectionRange;
            Vector3 rightBound = Quaternion.Euler(0, attackAngleThreshold, 0) * transform.forward * detectionRange;
            Gizmos.DrawLine(transform.position, transform.position + leftBound);
            Gizmos.DrawLine(transform.position, transform.position + rightBound);
        }
    }
}