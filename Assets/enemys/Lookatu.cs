using UnityEngine;

public class Lookatu : MonoBehaviour
{
    [Header("Configurações de Rotação")]
    [SerializeField] private bool lookAtCamera = true;
    [SerializeField] private float rotationSpeed = 5f;
    
    [Header("Ajuste de Frente")]
    [SerializeField] private Vector3 forwardDirection = Vector3.forward;
    [SerializeField] private bool lockXRotation = false;
    [SerializeField] private bool lockYRotation = true;
    [SerializeField] private bool lockZRotation = false;

    private Transform cameraTransform;
    private Vector3 originalForward;

    void Start()
    {
        // Obtém a transformação da câmera principal
        cameraTransform = Camera.main.transform;
        originalForward = transform.forward;
    }

    void Update()
    {
        if (lookAtCamera && cameraTransform != null)
        {
            // Calcula a direção para a câmera
            Vector3 directionToCamera = cameraTransform.position - transform.position;
            
            // Aplica os locks de eixo
            if (lockXRotation) directionToCamera.x = 0;
            if (lockYRotation) directionToCamera.y = 0;
            if (lockZRotation) directionToCamera.z = 0;
            
            // Calcula a rotação desejada
            Quaternion targetRotation = Quaternion.LookRotation(directionToCamera);
            
            // Ajusta a direção "frente" conforme definido
            targetRotation *= Quaternion.FromToRotation(Vector3.forward, forwardDirection);
            
            // Suaviza a rotação
            transform.rotation = Quaternion.Slerp(
                transform.rotation, 
                targetRotation, 
                rotationSpeed * Time.deltaTime
            );
        }
    }

    // Método para ajustar a direção "frente" via script
    public void SetForwardDirection(Vector3 newForward)
    {
        forwardDirection = newForward.normalized;
    }

    // Método para resetar para a frente original
    public void ResetForwardDirection()
    {
        forwardDirection = originalForward;
    }
}