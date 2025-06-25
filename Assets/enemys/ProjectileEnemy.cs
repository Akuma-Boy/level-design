using UnityEngine;

[RequireComponent(typeof(Animator))]
public class ProjectileEnemy : MonoBehaviour
{
    [Header("Attack Settings")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float projectileSpeed = 15f;
    [SerializeField] private float detectionRange = 20f;
    [SerializeField] private LayerMask obstacleLayers = ~0;
    
    [Header("Attack Pattern")]
    [SerializeField] private float initialBurstRate = 0.2f;
    [SerializeField] private float normalFireRate = 1f;
    [SerializeField] private float burstDuration = 3f;
    [SerializeField] private float attackAngleThreshold = 45f;
    
    [Header("Animation")]
    [SerializeField] private string attackTriggerName = "Attack";
    
    [Header("Debug")]
    [SerializeField] private bool showDebugInfo = true;
    [SerializeField] private Color debugRayColor = Color.red;
    
    private Transform player;
    private Animator animator;
    private float nextFireTime;
    private bool isInBurstMode;
    private float burstEndTime;
    private int attackTriggerHash;
    
    private void Awake()
    {
        animator = GetComponent<Animator>();
        attackTriggerHash = Animator.StringToHash(attackTriggerName);
        
        if (firePoint == null)
        {
            firePoint = transform;
            Debug.LogWarning("FirePoint not assigned, using enemy transform", this);
        }
    }
    
    private void Start()
    {
        InitializeAttackPattern();
        FindPlayer();
    }
    
    private void Update()
    {
        if (player == null)
        {
            FindPlayer();
            return;
        }
        
        UpdateAttackPattern();
        TryAttack();
    }
    
    private void InitializeAttackPattern()
    {
        isInBurstMode = true;
        burstEndTime = Time.time + burstDuration;
        nextFireTime = Time.time; // Permite ataque imediato
    }
    
    private void UpdateAttackPattern()
    {
        if (isInBurstMode && Time.time > burstEndTime)
        {
            isInBurstMode = false;
            if (showDebugInfo) Debug.Log("Switching to normal fire rate", this);
        }
    }
    
    private void FindPlayer()
    {
        var playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
            if (showDebugInfo) Debug.Log("Player found: " + player.name, this);
        }
        else
        {
            if (showDebugInfo) Debug.LogWarning("Player not found!", this);
            Invoke(nameof(FindPlayer), 0.5f);
        }
    }
    
    private void TryAttack()
    {
        if (CanAttack() && Time.time >= nextFireTime)
        {
            Attack();
            UpdateFireRate();
        }
    }
    
    private bool CanAttack()
    {
        if (player == null) return false;
        
        float distance = Vector3.Distance(transform.position, player.position);
        if (distance > detectionRange) return false;
        
        Vector3 direction = (player.position - transform.position).normalized;
        float angle = Vector3.Angle(transform.forward, direction);
        if (angle > attackAngleThreshold) return false;
        
        return HasLineOfSight();
    }
    
    private bool HasLineOfSight()
    {
        Vector3 direction = player.position - firePoint.position;
        if (Physics.Raycast(firePoint.position, direction, out var hit, detectionRange, obstacleLayers))
        {
            if (showDebugInfo)
            {
                Debug.DrawLine(firePoint.position, hit.point, 
                    hit.transform.CompareTag("Player") ? Color.green : debugRayColor, 0.1f);
            }
            return hit.transform.CompareTag("Player");
        }
        return false;
    }
    
    private void Attack()
    {
        TriggerAttackAnimation();
        FireProjectile();
    }
    
    private void TriggerAttackAnimation()
    {
        if (animator != null && animator.isActiveAndEnabled)
        {
            animator.SetTrigger(attackTriggerHash);
        }
    }
    
    private void FireProjectile()
    {
        if (projectilePrefab == null) return;
        
        Vector3 direction = (player.position - firePoint.position).normalized;
        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.LookRotation(direction));
        
        SetupProjectilePhysics(projectile, direction);
        Destroy(projectile, 5f);
    }
    
    private void SetupProjectilePhysics(GameObject projectile, Vector3 direction)
    {
        if (projectile.TryGetComponent<Rigidbody>(out var rb))
        {
            rb.linearVelocity = direction * projectileSpeed;
        }
        
        // Ignora colisão com o inimigo que atirou
        if (TryGetComponent<Collider>(out var enemyCollider))
        {
            foreach (var col in projectile.GetComponentsInChildren<Collider>())
            {
                Physics.IgnoreCollision(enemyCollider, col);
            }
        }
    }
    
    private void UpdateFireRate()
    {
        float currentRate = isInBurstMode ? initialBurstRate : normalFireRate;
        nextFireTime = Time.time + 1f / currentRate;
    }
    
    public void ResetAttackPattern()
    {
        isInBurstMode = true;
        burstEndTime = Time.time + burstDuration;
        nextFireTime = Time.time; // Permite ataque imediato após reset
        if (showDebugInfo) Debug.Log("Attack pattern reset to burst mode", this);
    }
    
    private void OnDrawGizmosSelected()
    {
        if (!showDebugInfo) return;
        
        // Detection range
        Gizmos.color = new Color(1, 0, 0, 0.1f);
        Gizmos.DrawSphere(transform.position, detectionRange);
        
        // Attack angle
        if (player != null)
        {
            Gizmos.color = Color.yellow;
            Vector3 leftBound = Quaternion.Euler(0, -attackAngleThreshold, 0) * transform.forward * detectionRange;
            Vector3 rightBound = Quaternion.Euler(0, attackAngleThreshold, 0) * transform.forward * detectionRange;
            Gizmos.DrawLine(transform.position, transform.position + leftBound);
            Gizmos.DrawLine(transform.position, transform.position + rightBound);
        }
    }
}