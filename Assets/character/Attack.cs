using UnityEngine;
using UnityEngine.Events;

public class Attack : MonoBehaviour
{
    [Header("Referências")]
    [SerializeField] private Animator animator;
    [SerializeField] private Camera playerCamera;
    [SerializeField] private ManaSystem manaSystem;

    [Header("Configurações do Projétil")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float normalProjectileSpeed = 20f;
    [SerializeField] private float pistolsProjectileSpeed = 30f;
    [SerializeField] private float normalAttackCooldown = 0.5f;
    [SerializeField] private float pistolsAttackCooldown = 0.2f;
    [SerializeField] private int pistolManaCost = 10; // Custo de mana por tiro de pistola

    [Header("Eventos")]
    public UnityEvent OnPistolShot; // Disparado quando atira com pistola
    public UnityEvent OnNotEnoughMana; // Disparado quando não tem mana suficiente

    [Tooltip("Offset de spawn do projétil em relação à câmera (X = lateral, Y = altura, Z = frente).")]
    [SerializeField] private Vector3 projectileSpawnOffset = new Vector3(0f, 0f, 1f);

    [SerializeField] private float comboWindow = 0.3f;

    private float nextAttackTime = 0f;
    private bool pistols = false;
    private bool readyForCombo = false;
    private float lastPistolAttackTime;
    private float currentAttackCooldown;
    private float currentProjectileSpeed;

    void Start()
    {
        if (animator == null)
            animator = GetComponent<Animator>();

        if (playerCamera == null)
            playerCamera = Camera.main;

        if (manaSystem == null)
            manaSystem = GetComponent<ManaSystem>();

        currentAttackCooldown = normalAttackCooldown;
        currentProjectileSpeed = normalProjectileSpeed;
    }

    void Update()
    {
        // Toggle das pistolas com a tecla 2
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            TogglePistols();
        }

        // Ataque com botão esquerdo do mouse
        if (Input.GetMouseButtonDown(0) && Time.time >= nextAttackTime)
        {
            if (pistols)
            {
                if (manaSystem != null)
                {
                    if (!manaSystem.TrySpendMana(pistolManaCost))
                    {
                        OnNotEnoughMana.Invoke();
                        Debug.Log("Mana insuficiente para atirar!");
                        return;
                    }

                    OnPistolShot.Invoke();
                    PerformPistolAttack();
                    Shoot();
                    nextAttackTime = Time.time + currentAttackCooldown;
                }
            }
            else
            {
                PerformNormalAttack();
                Shoot();
                nextAttackTime = Time.time + currentAttackCooldown;
            }
        }


        ResetComboIfTimeout();
    }

    private void TogglePistols()
    {
        pistols = !pistols;
        animator.SetBool("Pistols", pistols);

        currentAttackCooldown = pistols ? pistolsAttackCooldown : normalAttackCooldown;
        currentProjectileSpeed = pistols ? pistolsProjectileSpeed : normalProjectileSpeed;

        Debug.Log($"Pistols: {pistols} | Cooldown: {currentAttackCooldown} | Speed: {currentProjectileSpeed}");
    }

    private void PerformPistolAttack()
    {
        if (Time.time - lastPistolAttackTime <= comboWindow && readyForCombo)
        {
            animator.ResetTrigger("AttackPistols");
            animator.SetTrigger("AttackPistols2");
            readyForCombo = false;
        }
        else
        {
            animator.ResetTrigger("AttackPistols2");
            animator.SetTrigger("AttackPistols");
            readyForCombo = true;
        }

        lastPistolAttackTime = Time.time;
        OnPistolShot.Invoke();
    }

    private void PerformNormalAttack()
    {
        animator.SetTrigger("Attack");
    }

    private void ResetComboIfTimeout()
    {
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
            projectileScript.speed = currentProjectileSpeed;
        }
        else
        {
            Rigidbody rb = projectile.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = playerCamera.transform.forward * currentProjectileSpeed;
            }
        }
    }

    // Método para ajustar o custo de mana dinamicamente
    public void SetPistolManaCost(int newCost)
    {
        pistolManaCost = Mathf.Max(0, newCost);
    }

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