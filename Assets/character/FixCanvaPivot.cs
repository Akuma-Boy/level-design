using UnityEngine;
using UnityEngine.UI;

public class FixCanvasPivot : MonoBehaviour
{
    void Start()
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        rectTransform.pivot = new Vector2(0.5f, 0.5f); // Centralizado
        rectTransform.anchoredPosition = Vector2.zero; // Reseta a posição
    }
}