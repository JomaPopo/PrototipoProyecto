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

        Vector2 mouseDelta = Mouse.current.delta.ReadValue();
        float mouseX = mouseDelta.x * mouseSensitivity * Time.deltaTime;
        float mouseY = mouseDelta.y * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

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