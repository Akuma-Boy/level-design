using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ManaSystem : MonoBehaviour
{
    [Header("Configura��es")]
    [SerializeField] private int maxMana = 100;
    [SerializeField] private int currentMana;
    [SerializeField] private int defaultManaCost = 10; // Custo padr�o de mana
    [SerializeField] private float manaRegenRate = 5f;

    [Header("Refer�ncias")]
    [SerializeField] private Image manaBarFill;
    [SerializeField] private Text manaText;

    [Header("Eventos")]
    public UnityEvent OnManaChanged;
    public UnityEvent OnNotEnoughMana;

    // M�todo �nico que pode verificar qualquer custo
    public bool HasEnoughMana(int requiredMana)
    {
        return currentMana >= requiredMana;
    }

    // M�todo para gastar mana com verifica��o
    public bool TrySpendMana(int amount)
    {
        if (HasEnoughMana(amount))
        {
            currentMana -= amount;
            OnManaChanged.Invoke();
            UpdateManaBar();
            return true;
        }
        return false;
    }

    // M�todo espec�fico para o custo padr�o (opcional)
    public bool HasEnoughManaForDefaultCost()
    {
        return HasEnoughMana(defaultManaCost);
    }

    void Start()
    {
        currentMana = maxMana;
        UpdateManaBar();
    }

    void Update()
    {
        if (currentMana < maxMana)
        {
            RegenerateMana();
        }
    }

    private void RegenerateMana()
    {
        float manaToAdd = manaRegenRate * Time.deltaTime;
        currentMana = Mathf.Min(maxMana, currentMana + (int)manaToAdd);
        OnManaChanged.Invoke();
        UpdateManaBar();
    }

    private void UpdateManaBar()
    {
        if (manaBarFill != null)
        {
            manaBarFill.fillAmount = (float)currentMana / maxMana;
        }

        if (manaText != null)
        {
            manaText.text = $"{currentMana}/{maxMana}";
        }
    }

    public void SetDefaultManaCost(int cost) => defaultManaCost = cost;
    public void SetRegenRate(float rate) => manaRegenRate = rate;

    // M�todo para recuperar mana
    public void RestoreMana(int amount)
    {
        currentMana = Mathf.Min(maxMana, currentMana + amount);
        OnManaChanged.Invoke();
        UpdateManaBar();
    }






}