using UnityEngine;

public class EnemyBottleDrop : MonoBehaviour
{
    [Header("Configurações")]
    [SerializeField] private ParticleSystem healingParticles;
    [SerializeField] private float particleSpeed = 5f;
    [SerializeField] private float particleLifetime = 2f;

    private EnemyHP enemyHP;
    private Transform playerTransform;
    private BottleController bottleController;

    private void Awake()
    {
        enemyHP = GetComponent<EnemyHP>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        bottleController = FindAnyObjectByType<BottleController>();
    }

    private void OnEnable()
    {
        if (enemyHP != null)
        {
            enemyHP.OnDeath += HandleEnemyDeath;
        }
    }

    private void OnDisable()
    {
        if (enemyHP != null)
        {
            enemyHP.OnDeath -= HandleEnemyDeath;
        }
    }

    private void HandleEnemyDeath()
    {
        // Regenera o gole imediatamente
        RegeneratePlayerGulp();

        // Efeito visual das partículas (opcional)
        if (healingParticles != null && playerTransform != null)
        {
            ParticleSystem particles = Instantiate(healingParticles, transform.position, Quaternion.identity);

            ParticleSystem.MainModule main = particles.main;
            main.simulationSpace = ParticleSystemSimulationSpace.World;
            main.loop = false;

            var emitParams = new ParticleSystem.EmitParams();
            emitParams.position = transform.position;
            emitParams.velocity = (playerTransform.position - transform.position).normalized * particleSpeed;
            emitParams.startLifetime = particleLifetime;

            particles.Emit(emitParams, 1);
            Destroy(particles.gameObject, particleLifetime);
        }
    }

    private void RegeneratePlayerGulp()
    {
        if (bottleController != null)
        {
            bottleController.GolesRestantes = Mathf.Min(bottleController.GolesRestantes + 1, 4);
            bottleController.AtualizarEstadoGarrafa();
        }
    }
}