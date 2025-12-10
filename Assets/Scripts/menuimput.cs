using UnityEngine;
using UnityEngine.UI; // Para controlar el color de la imagen
using UnityEngine.InputSystem; // Para el Input System

public class MenuInputDebug : MonoBehaviour
{
    [Header("Referencias UI")]
    [Tooltip("Arrastra aquí la Imagen (el cuadro) que quieres que cambie de color")]
    [SerializeField] private Image debugBox;

    [Header("Colores")]
    [SerializeField] private Color colorNormal = Color.red;
    [SerializeField] private Color colorActivo = Color.green;

    // Variables para guardar el estado de cada mano
    private bool leftPressed = false;
    private bool rightPressed = false;

    void Start()
    {
        if (debugBox != null)
            debugBox.color = colorNormal;
    }

    void Update()
    {
        // Verificamos si AMBOS están presionados
        bool bothPressed = leftPressed && rightPressed;

        // Cambiamos el color según el estado
        if (debugBox != null)
        {
            debugBox.color = bothPressed ? colorActivo : colorNormal;
        }
    }

    // --- FUNCIONES PARA EL PLAYER INPUT ---

    // Conecta esto al evento de tu mano IZQUIERDA (CPR_Izquierda)
    public void OnLeftInput(InputAction.CallbackContext context)
    {
        if (context.performed) leftPressed = true;
        else if (context.canceled) leftPressed = false;
    }

    // Conecta esto al evento de tu mano DERECHA (CPR_Derecha)
    public void OnRightInput(InputAction.CallbackContext context)
    {
        if (context.performed) rightPressed = true;
        else if (context.canceled) rightPressed = false;
    }
}