using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 5f;
    public float jumpForce = 5f;

    [Header("VR Settings")]
    public Transform vrCamera;

    [Header("Ground Check")]
    public Transform groundCheck; 
    public float groundDistance = 0.4f; 
    public LayerMask groundMask; 
    private Rigidbody rb;
    private Vector2 moveInput;
    private Vector3 moveDirection;
    private bool isGrounded; 

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (vrCamera == null)
        {
            vrCamera = Camera.main.transform;
            if (vrCamera == null)
            {
                Debug.LogError("Error: No se encontró la cámara de VR. Por favor, asígnala en el Inspector.");
            }
        }
    }

    private void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
       
        if (isGrounded)
        {
           
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    private void MovePlayer()
    {
        // 1. Obtener la dirección "adelante" de la cámara
        Vector3 camForward = vrCamera.forward;
        // 2. Obtener la dirección "derecha" de la cámara
        Vector3 camRight = vrCamera.right;

        // 3. Aplanar los vectores para que no te muevas hacia arriba/abajo al mirar
        camForward.y = 0;
        camRight.y = 0;

        // 4. Normalizar para asegurar velocidad constante
        camForward.Normalize();
        camRight.Normalize();

        // 5. Calcular la dirección de movimiento basada en la cámara y el input
        moveDirection = (camForward * moveInput.y + camRight * moveInput.x);

        // 6. Aplicar la velocidad (Tu lógica original para mantener el salto/gravedad)
        Vector3 newVelocity = new Vector3(moveDirection.x * speed, rb.linearVelocity.y, moveDirection.z * speed);
        rb.linearVelocity = newVelocity;
    }
    public void JumpForTest()
    {
        if (isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }
}