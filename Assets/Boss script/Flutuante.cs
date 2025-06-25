using UnityEngine;

public class Flutuante : MonoBehaviour
{
    public float amplitude = 0.2f;   // Altura do movimento
    public float velocidade = 1f;    // Velocidade da flutuação

    private Vector3 posicaoInicial;

    void Start()
    {
        posicaoInicial = transform.position;
    }

    void Update()
    {
        float offsetY = Mathf.Sin(Time.time * velocidade) * amplitude;
        transform.position = posicaoInicial + new Vector3(0, offsetY, 0);
    }
}
