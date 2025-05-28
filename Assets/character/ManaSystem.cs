using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ManaSystem : MonoBehaviour
{
    [Header("Configurações")]
    [SerializeField] private int maxMana = 100;
    [SerializeField] private int currentMana;
    [SerializeField] private int defaultManaCost = 10;
    [SerializeField] private float manaRegenRate = 5f;

    [Header("Referências")]
    [SerializeField] private Image manaBarFill;
    [SerializeField] private Text manaText;

    [Header("Eventos")]
    public UnityEvent OnManaChanged;
    public UnityEvent OnNotEnoughMana;

    private void Start()
    {
        currentMana = maxMana;
        UpdateManaBar();
    }

    private void Update()
    {
        if (currentMana < maxMana)
        {
            RegenerateMana();
        }
    }

    public bool HasEnoughMana(int requiredMana)
    {
        return currentMana >= requiredMana;
    }

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

    public bool HasEnoughManaForDefaultCost()
    {
        return HasEnoughMana(defaultManaCost);
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

    public void RestoreMana(int amount)
    {
        currentMana = Mathf.Min(maxMana, currentMana + amount);
        OnManaChanged.Invoke();
        UpdateManaBar();
    }

    public void SetDefaultManaCost(int cost) => defaultManaCost = cost;
    public void SetRegenRate(float rate) => manaRegenRate = rate;
}