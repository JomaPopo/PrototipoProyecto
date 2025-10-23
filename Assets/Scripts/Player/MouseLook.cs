using UnityEngine;
using UnityEngine.InputSystem;

public class MouseLook : MonoBehaviour
{
    [Header("Look Settings")]
    public float mouseSensitivity = 100f;
    public Transform playerBody;

    private float xRotation = 0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        Vector2 mouseDelta = context.ReadValue<Vector2>();
        ProcessLook(mouseDelta);
    }

    public void EnableLook()
    {
        this.enabled = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void DisableLook()
    {
        this.enabled = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ProcessLook(Vector2 mouseDelta)
    {
        // --- LA CORRECCIÓN ESTÁ AQUÍ ---
        // Usamos Time.deltaTime para un movimiento suave e independiente del framerate.
        float mouseX = mouseDelta.x * mouseSensitivity * Time.deltaTime;
        float mouseY = mouseDelta.y * mouseSensitivity * Time.deltaTime;

        // El resto de tu lógica es correcta.
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        // Rotación vertical (arriba/abajo) se aplica a la cámara.
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // Rotación horizontal (izquierda/derecha) se aplica al cuerpo del jugador.
        if (playerBody != null)
            playerBody.Rotate(Vector3.up * mouseX);
    }
}