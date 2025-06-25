using UnityEngine;
using UnityEngine.SceneManagement;

public class VerificadorPrefab : MonoBehaviour
{
    public GameObject prefabAlvo;  // Refer�ncia ao prefab instanciado na cena

    void Update()
    {
        if (prefabAlvo == null)
        {
            // Quando o prefab foi destru�do, reinicia a cena
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
