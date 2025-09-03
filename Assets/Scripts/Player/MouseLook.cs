using UnityEngine;
using UnityEngine.InputSystem;

public class MouseLook : MonoBehaviour
{
    [Header("Look Settings")]
    public float mouseSensitivity = 100f;
    public Transform playerBody;

    [Header("Cursor Settings")]
    public bool lockCursorOnEnable = true;

    private float xRotation = 0f;
    private bool cursorLocked = false;

    void Start()
    {
        UnlockCursor();
        this.enabled = false;
    }

    void Update()
    {
        if (Keyboard.current.enterKey.wasPressedThisFrame)
        {
            ToggleCursor();
        }

        if (!cursorLocked || Mouse.current == null) return;

        // Leemos el input del mouse
        Vector2 mouseDelta = Mouse.current.delta.ReadValue();

        // �NUEVO! Llamamos a la funci�n p�blica con el input que le�mos
        ProcessLook(mouseDelta);
    }

    // �NUEVA FUNCI�N P�BLICA! Toda la l�gica de rotaci�n est� aqu�.
    // Ahora nuestros tests pueden llamar a esta funci�n directamente.
    public void ProcessLook(Vector2 mouseDelta)
    {
        float mouseX = mouseDelta.x * mouseSensitivity * Time.deltaTime;
        float mouseY = mouseDelta.y * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        if (playerBody != null)
            playerBody.Rotate(Vector3.up * mouseX);
    }

    private void ToggleCursor()
    {
        if (cursorLocked)
            UnlockCursor();
        else
            LockCursor();
    }

    private void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        cursorLocked = true;
    }

    private void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        cursorLocked = false;
    }


    public void EnableMouseLook()
    {
        this.enabled = true;
        if (lockCursorOnEnable) LockCursor();
    }
}