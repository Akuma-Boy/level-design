using UnityEngine;
using UnityEngine.SceneManagement;

public class VerificadorPrefab : MonoBehaviour
{
    public GameObject prefabAlvo;  // Referência ao prefab instanciado na cena

    void Update()
    {
        if (prefabAlvo == null)
        {
            // Quando o prefab foi destruído, reinicia a cena
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
