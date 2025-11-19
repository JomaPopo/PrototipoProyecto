using UnityEngine;
using UnityEngine.InputSystem;

public class InputTestLogger : MonoBehaviour
{
    // --- MANO IZQUIERDA ---

    public void OnTestLeftTrigger(InputAction.CallbackContext context)
    {
        // context.performed significa "justo cuando lo presionaste"
        if (context.performed)
        {
            Debug.Log("--- ¡GATILLO IZQUIERDO (ÍNDICE) PRESIONADO! ---");
        }
    }

    public void OnTestLeftGrip(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log("--- ¡AGARRE IZQUIERDO (GRIP) PRESIONADO! ---");
        }
    }

    // --- MANO DERECHA ---

    public void OnTestRightTrigger(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log("--- ¡GATILLO DERECHO (ÍNDICE) PRESIONADO! ---");
        }
    }

    public void OnTestRightGrip(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log("--- ¡AGARRE DERECHO (GRIP) PRESIONADO! ---");
        }
    }
}