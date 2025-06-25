using UnityEngine;

public class InteracaoDialogo : MonoBehaviour
{
    public GameObject painelDialogo;    // Painel da UI com o texto
    public Transform jogador;           // Referência ao transform do jogador
    public float distanciaMaxima = 3f;  // Distância para interagir

    private bool dialogoAtivo = false;

    void Update()
    {
        float distancia = Vector3.Distance(jogador.position, transform.position);

        if (Input.GetKeyDown(KeyCode.Q) && distancia <= distanciaMaxima)
        {
            painelDialogo.SetActive(true);
            dialogoAtivo = true;
        }

        if (Input.GetKeyDown(KeyCode.Escape) && dialogoAtivo)
        {
            painelDialogo.SetActive(false);
            dialogoAtivo = false;
        }
    }
}
