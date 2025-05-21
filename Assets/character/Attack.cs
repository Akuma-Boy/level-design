using UnityEngine;

public class Attack : MonoBehaviour
{
    [Header("Referências")]
    [SerializeField] private Animator animator;
    [SerializeField] private Camera playerCamera;

    [Header("Configurações do Projétil")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float projectileSpeed = 20f;
    [SerializeField] private float attackCooldown = 0.5f;

    [Tooltip("Offset de spawn do projétil em relação à câmera (X = lateral, Y = altura, Z = frente).")]
    [SerializeField] private Vector3 projectileSpawnOffset = new Vector3(0f, 0f, 1f);

    [SerializeField] private float comboWindow = 0.3f; // Janela de tempo para o combo

    private float nextAttackTime = 0f;
    private bool pistols = false;
    private bool readyForCombo = false; // Controla se pode executar o segundo ataque
    private float lastPistolAttackTime;

    void Start()
    {
        if (animator == null)
            animator = GetComponent<Animator>();

        if (playerCamera == null)
            playerCamera = Camera.main;
    }

    void Update()
    {
        // Toggle das pistolas com a tecla 2
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            pistols = !pistols;
            animator.SetBool("Pistols", pistols);
            Debug.Log("Pistols: " + pistols);
        }

        // Ataque com botão esquerdo do mouse
        if (Input.GetMouseButtonDown(0) && Time.time >= nextAttackTime)
        {
            if (pistols)
            {
                if (Time.time - lastPistolAttackTime <= comboWindow && readyForCombo)
                {
                    // Segundo ataque do combo
                    Debug.Log("[DEBUG] Trigger AttackPistols2 chamado!");
                    animator.ResetTrigger("AttackPistols");
                    animator.SetTrigger("AttackPistols2");
                    readyForCombo = false;
                }
                else
                {
                    // Primeiro ataque
                    Debug.Log("[DEBUG] Trigger AttackPistols chamado!");
                    animator.ResetTrigger("AttackPistols2");
                    animator.SetTrigger("AttackPistols");
                    readyForCombo = true;
                }
                lastPistolAttackTime = Time.time;
            }
            else
            {
                animator.SetTrigger("Attack");
                Shoot();
            }
            nextAttackTime = Time.time + attackCooldown;
        }

        // Reseta o combo se o tempo passar
        if (Time.time - lastPistolAttackTime > comboWindow && readyForCombo)
        {
            readyForCombo = false;
        }
    }

    void Shoot()
    {
        if (projectilePrefab == null)
        {
            Debug.LogError("Prefab do projétil não atribuído!");
            return;
        }

        Vector3 spawnPosition = playerCamera.transform.position +
                                playerCamera.transform.forward * projectileSpawnOffset.z +
                                playerCamera.transform.right * projectileSpawnOffset.x +
                                playerCamera.transform.up * projectileSpawnOffset.y;

        GameObject projectile = Instantiate(
            projectilePrefab,
            spawnPosition,
            playerCamera.transform.rotation
        );

        Projectile projectileScript = projectile.GetComponent<Projectile>();
        if (projectileScript != null)
        {
            projectileScript.speed = projectileSpeed;
        }
        else
        {
            Rigidbody rb = projectile.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = playerCamera.transform.forward * projectileSpeed;
            }
        }
    }

    // Método opcional para debug
    void OnValidate()
    {
        if (animator != null)
        {
            Debug.Log("Parâmetros no Animator:");
            foreach (AnimatorControllerParameter param in animator.parameters)
            {
                Debug.Log("- " + param.name + " (" + param.type + ")");
            }
        }
    }
}
