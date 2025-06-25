using UnityEngine;
using UnityEngine.UI; // Necessário para trabalhar com UI

public class StartGameMenu : MonoBehaviour
{
    public GameObject imagemParaEsconder;

    public void EsconderImagem()
    {
        if (imagemParaEsconder != null)
        {
            imagemParaEsconder.SetActive(false);
        }
    }
}