using UnityEngine;
using UnityEngine.InputSystem;

public class MouseLook : MonoBehaviour
{
    [Header("Look Settings")]
    public float mouseSensitivity = 100f;
    public Transform playerBody;

    private float xRotation = 0f;

    // Ya no necesitamos la lógica de Start() que lo desactivaba.
    // Ahora, el script estará siempre activo cuando el juego comience.
    void Start()
    {
        // Bloqueamos el cursor en el centro de la pantalla al empezar el juego.
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Esta es la nueva función pública que será llamada por el PlayerInput.
    public void OnLook(InputAction.CallbackContext context)
    {
        // Leemos el valor Vector2 del movimiento del mouse desde el contexto del input.
        Vector2 mouseDelta = context.ReadValue<Vector2>();

        // Llamamos a nuestra función de lógica con ese valor.
        ProcessLook(mouseDelta);
    }

    // Esta función contiene la lógica de rotación que ya tenías.
    // La mantenemos separada por si la necesitamos para los tests.
    public void ProcessLook(Vector2 mouseDelta)
    {
        // Usamos Time.fixedDeltaTime para que la sensibilidad sea consistente
        // independientemente de los frames por segundo.
        float mouseX = mouseDelta.x * mouseSensitivity * Time.fixedDeltaTime;
        float mouseY = mouseDelta.y * mouseSensitivity * Time.fixedDeltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        // Rotamos la cámara arriba y abajo (eje X)
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // Rotamos el cuerpo del jugador a los lados (eje Y)
        if (playerBody != null)
            playerBody.Rotate(Vector3.up * mouseX);
    }
}