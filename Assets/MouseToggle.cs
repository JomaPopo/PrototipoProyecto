using UnityEngine;

public class MouseToggle : MonoBehaviour
{
    private bool cursorVisible = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            cursorVisible = !cursorVisible;

            Cursor.visible = cursorVisible;

            Cursor.lockState = cursorVisible
                ? CursorLockMode.None   // Libera el cursor
                : CursorLockMode.Locked; // Bloquea el cursor al centro
        }
    }
}
