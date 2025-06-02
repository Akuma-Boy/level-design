using UnityEngine;
using UnityEngine.UI;

public class GarrafaUIController : MonoBehaviour
{
    [Header("Refer�ncias")]
    [SerializeField] private Image garrafaFillImage;
    [SerializeField] private BottleController bottleController;

    [Header("Configura��o")]
    [SerializeField] private int totalGoles = 4;

    private void Start()
    {
        if (bottleController == null)
            bottleController = FindFirstObjectByType<BottleController>();

        UpdateFill();
    }

    private void Update()
    {
        UpdateFill();
    }

    private void UpdateFill()
    {
        if (bottleController == null || garrafaFillImage == null) return;

        float quantidade = Mathf.Clamp01((float)bottleController.GolesRestantes / totalGoles);
        garrafaFillImage.fillAmount = quantidade;
    }
}
