using UnityEngine;

public class BottleController : MonoBehaviour
{
    [Header("Referências")]
    public Animator animator;
    public PlayerHealth playerHealth;
    [SerializeField] private Attack attackScript;

    [Header("Configurações")]
    public int healAmount = 30;
    public int GolesRestantes { get; private set; } = 4;
    private const int maxGoles = 4;

    [Header("Estado Interno")]
    private bool garrafaCheia = true;
    private bool garrafaVazia = false;
    public bool garrafaEquipando { get; private set; } = false;
    private bool acabouDeUsarGarrafa = false;


    private void Start()
    {
        if (attackScript == null)
            attackScript = Object.FindFirstObjectByType<Attack>();

        AtualizarEstadoGarrafa();
    }

    private void Update()
    {
        HandleInput();
    }

    private void HandleInput()
    {
        // Alternar garrafa com tecla E
        if (Input.GetKeyDown(KeyCode.E))
        {
            ToggleGarrafa();
        }

        // Usar gole com clique esquerdo do mouse
        if (Input.GetMouseButtonDown(0) && garrafaEquipando && garrafaCheia)
        {
            UsarGarrafa();
        }
    }

    private void ToggleGarrafa()
    {
        garrafaEquipando = !garrafaEquipando;
        animator.SetBool("Garrafa", garrafaEquipando);

        if (attackScript != null)
            attackScript.ForcarDesativarPistolas();

        if (garrafaEquipando)
        {
            AtualizarEstadoGarrafa();
        }
        else
        {
            ResetarAnimacoesGarrafa();
        }
    }

    private void UsarGarrafa()
    {
        if (GolesRestantes <= 0) return;

        garrafaEquipando = false;
        ResetarAnimacoesGarrafa();
        animator.SetTrigger("GarrafaUso");

        if (playerHealth != null)
            playerHealth.Heal(healAmount);

        GolesRestantes--;

        garrafaCheia = GolesRestantes > 0;
        garrafaVazia = !garrafaCheia;
        acabouDeUsarGarrafa = true;


        AtualizarEstadoGarrafa();
    }

    private void AtualizarEstadoGarrafa()
    {
        if (!garrafaEquipando) return;

        animator.SetBool("GarrafaCheia", garrafaCheia);
        animator.SetBool("GarrafaVazia", garrafaVazia);
    }

    private void ResetarAnimacoesGarrafa()
    {
        animator.SetBool("Garrafa", false);
        animator.SetBool("GarrafaCheia", false);
        animator.SetBool("GarrafaVazia", false);
    }

    public void ForcarDesativarGarrafa()
    {
        garrafaEquipando = false;
        ResetarAnimacoesGarrafa();
    }

    public void RecarregarGarrafa()
    {
        GolesRestantes = maxGoles;
        garrafaCheia = true;
        garrafaVazia = false;
        AtualizarEstadoGarrafa();
    }
    public bool GarrafaAcabouDeSerUsada()
    {
        bool temp = acabouDeUsarGarrafa;
        acabouDeUsarGarrafa = false; // reseta logo após ser checado
        return temp;
    }

}
