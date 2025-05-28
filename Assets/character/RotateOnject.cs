using UnityEngine;

public class RotateObject : MonoBehaviour
{
    public Vector3 rotationSpeed = new Vector3(0, 100, 0); // Gira no eixo Y

    void Update()
    {
        transform.Rotate(rotationSpeed * Time.deltaTime);
    }
}
