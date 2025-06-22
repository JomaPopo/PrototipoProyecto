using UnityEngine;
using UnityEngine.InputSystem;

public class MouseLook : MonoBehaviour
{
    public float mouseSensitivity = 100f;
    public Transform playerBody;
    public PlayerMovement playerMovement;

    private float xRotation = 0f;
    private bool cursorLocked = false; // Comienza desbloqueado

    void Start()
    {
        UnlockCursor(); // Al iniciar, desbloquear el cursor
    }

    void Update()
    {
        if (Keyboard.current.enterKey.wasPressedThisFrame)
        {
            if (cursorLocked)
                UnlockCursor();
            else
                LockCursor();
        }

        // Si el cursor est� desbloqueado, no mover la c�mara
        if (!cursorLocked || Mouse.current == null) return;

        Vector2 mouseDelta = Mouse.current.delta.ReadValue();
        float mouseX = mouseDelta.x * mouseSensitivity * Time.deltaTime;
        float mouseY = mouseDelta.y * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        playerBody.Rotate(Vector3.up * mouseX);
    }

    private void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        cursorLocked = true;

        if (playerMovement != null) playerMovement.enabled = true;
    }

    private void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        cursorLocked = false;

        if (playerMovement != null) playerMovement.enabled = false;
    }
}
