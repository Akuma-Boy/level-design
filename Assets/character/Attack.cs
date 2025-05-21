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
    [SerializeField] private float projectileSpawnDistance = 1f;
    [SerializeField] private float comboTimeout = 1f; // Tempo máximo entre ataques para combo
    
    private float nextAttackTime = 0f;
    private bool pistols = false;
    private float lastPistolAttackTime;
    private bool canDoSecondAttack = false;

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
                if (Time.time - lastPistolAttackTime > comboTimeout)
                {
                    Debug.Log("[DEBUG] Trigger AttackPistols chamado!");
                    animator.ResetTrigger("AttackPistols");
                    animator.ResetTrigger("AttackPistols2"); // Reseta o segundo ataque também
                    animator.SetTrigger("AttackPistols");
                    canDoSecondAttack = true;
                }
                else if (canDoSecondAttack)
                {
                    Debug.Log("[DEBUG] Trigger AttackPistols2 chamado!");
                    animator.ResetTrigger("AttackPistols");
                    animator.ResetTrigger("AttackPistols2");
                    animator.SetTrigger("AttackPistols2");
                    canDoSecondAttack = false;
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

        // Resetar combo se passar do tempo limite
        if (canDoSecondAttack && Time.time - lastPistolAttackTime > comboTimeout)
        {
            canDoSecondAttack = false;
            Debug.Log("[DEBUG] Combo resetado (tempo esgotado)");
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
                              playerCamera.transform.forward * projectileSpawnDistance;
        
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

    // Método opcional para debug (verifica se os parâmetros existem no Animator)
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