using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [Header("Configurações Básicas")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float rotationSpeed = 5f;
    
    [Header("Flutuação ao Redor do Player")]
    [SerializeField] private float orbitDistance = 5f;
    [SerializeField] private float orbitSpeed = 2f;
    [SerializeField] private float floatHeight = 1f;
    [SerializeField] private float heightAdjustSpeed = 2f;
    
    [Header("Área de Provocação")]
    [SerializeField] private float aggroRadius = 8f;
    [SerializeField] private float deaggroRadius = 12f;
    
    [Header("Configurações de Física")]
    [SerializeField] private bool useGravity = false;
    [SerializeField] private float groundCheckDistance = 0.5f;
    [SerializeField] private LayerMask groundLayer;
    
    private Transform player;
    private Rigidbody rb;
    private Vector3 targetPosition;
    private bool isAggroed;
    private bool isGrounded;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        rb = GetComponent<Rigidbody>();
        
        // Configuração inicial do Rigidbody
        if (rb != null)
        {
            rb.useGravity = useGravity;
            rb.constraints = RigidbodyConstraints.FreezeRotation;
        }
    }

    void Update()
    {
        if (player == null) return;

        CheckGroundStatus();
        HandleAggroState();
        CalculateMovement();
    }

    void FixedUpdate()
    {
        if (rb != null)
        {
            HandleMovement();
            MaintainFloatHeight();
        }
    }

    void CheckGroundStatus()
    {
        isGrounded = Physics.Raycast(
            transform.position, 
            Vector3.down, 
            groundCheckDistance, 
            groundLayer
        );
    }

    void HandleAggroState()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        
        if (!isAggroed && distanceToPlayer < aggroRadius)
        {
            isAggroed = true;
        }
        else if (isAggroed && distanceToPlayer > deaggroRadius)
        {
            isAggroed = false;
        }
    }

    void CalculateMovement()
    {
        if (isAggroed)
        {
            // Orbita ao redor do jogador
            Vector3 orbitDirection = (transform.position - player.position).normalized;
            orbitDirection.y = 0;
            targetPosition = player.position + (orbitDirection * orbitDistance);
        }
        else
        {
            // Movimento aleatório quando não está aggro
            if (Vector3.Distance(transform.position, targetPosition) < 1f || Random.value < 0.01f)
            {
                targetPosition = transform.position + new Vector3(
                    Random.Range(-5f, 5f), 
                    0, 
                    Random.Range(-5f, 5f)
                );
            }
        }
        
        // Ajusta a altura alvo
        targetPosition.y = player.position.y + floatHeight;
    }

    void HandleMovement()
    {
        if (isGrounded || !useGravity)
        {
            Vector3 direction = (targetPosition - transform.position).normalized;
            direction.y = 0; // Mantém o movimento no plano horizontal
            
            // Movimento suave usando Rigidbody
            rb.linearVelocity = new Vector3(
                direction.x * moveSpeed,
                rb.linearVelocity.y,
                direction.z * moveSpeed
            );
            
            // Rotação suave
            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                rb.rotation = Quaternion.Slerp(
                    rb.rotation, 
                    targetRotation, 
                    rotationSpeed * Time.fixedDeltaTime
                );
            }
        }
    }

    void MaintainFloatHeight()
    {
        if (!useGravity && !isGrounded)
        {
            // Ajuste suave da altura para flutuação
            float newY = Mathf.Lerp(
                transform.position.y, 
                targetPosition.y, 
                heightAdjustSpeed * Time.fixedDeltaTime
            );
            
            rb.linearVelocity = new Vector3(
                rb.linearVelocity.x,
                (newY - transform.position.y) * heightAdjustSpeed,
                rb.linearVelocity.z
            );
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, aggroRadius);
        
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, deaggroRadius);
        
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * groundCheckDistance);
    }
}