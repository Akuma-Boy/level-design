using UnityEngine;

public class Movimento : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float mouseSensitivity = 100f;
    
    public Transform playerCamera; // Câmera principal (não filha de nada)
    public RectTransform handRectTransform; // Referência ao RectTransform da mão
    
    private float xRotation = 0f;
    private float yRotation = 0f;
    
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        
        // Se estiver usando Screen Space - Camera no Canvas
        if (handRectTransform != null)
        {
            // Configura posição inicial (opcional)
            handRectTransform.anchoredPosition = new Vector2(200, -100); // Ajuste conforme necessário
        }
    }
    
    void Update()
    {
        
        // Rotação da câmera com o mouse
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
        
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        yRotation += mouseX;
        
        // Aplica rotação apenas à câmera
        playerCamera.localRotation = Quaternion.Euler(xRotation, yRotation, 0f);
        
        // Atualiza a posição da mão no Canvas (se necessário)
        // Neste caso, como é UI, ela já mantém a posição fixa automaticamente
    }
}