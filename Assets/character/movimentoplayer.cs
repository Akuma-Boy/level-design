using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float mouseSensitivity = 100f;

    [Header("References")]
    [SerializeField] private Transform cameraPivot; // Objeto que segura a câmera (para rotação vertical)
    [SerializeField] private Transform mainCamera;  // A câmera em si (para direção do movimento)

    private CharacterController characterController;
    private float xRotation = 0f;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked; // Trava o mouse no centro

        if (mainCamera == null)
            mainCamera = Camera.main.transform;
    }

    void Update()
    {
        // ===== MOVIMENTO WASD (relativo à câmera) =====
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // Pega a direção da câmera (ignorando inclinação para cima/baixo)
        Vector3 cameraForward = mainCamera.forward;
        Vector3 cameraRight = mainCamera.right;
        cameraForward.y = 0f;
        cameraRight.y = 0f;
        cameraForward.Normalize();
        cameraRight.Normalize();

        // Move na direção da câmera
        Vector3 moveDirection = (cameraForward * vertical + cameraRight * horizontal) * moveSpeed;
        characterController.SimpleMove(moveDirection);

        // ===== ROTAÇÃO DO MOUSE =====
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Rotaciona o corpo do jogador horizontalmente (esquerda/direita)
        transform.Rotate(Vector3.up * mouseX);

        // Rotaciona a câmera verticalmente (cima/baixo, com limite)
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -80f, 80f); // Evita virar a câmera de cabeça para baixo
        cameraPivot.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }
}