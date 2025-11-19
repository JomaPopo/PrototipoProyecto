using UnityEngine;

// Asegura que este script siempre esté junto a un CharacterController
[RequireComponent(typeof(CharacterController))]
public class CharacterGravity : MonoBehaviour
{
    [Header("Referencias")]
    private CharacterController controller;

    [Header("Gravedad")]
    [Tooltip("La fuerza de gravedad a aplicar")]
    public float gravityValue = -9.81f; // La gravedad estándar

    [Tooltip("Usado para 'pegar' al jugador al suelo")]
    public float groundStickForce = -2f;

    // Esta variable guardará nuestra velocidad de caída
    private Vector3 playerVelocity;

    private void Start()
    {
        // Obtenemos la referencia al controlador en este mismo objeto
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        // --- LÓGICA DE GRAVEDAD ---

        // 1. Revisa si el controlador está tocando el suelo
        bool isGrounded = controller.isGrounded;

        // 2. Si estamos en el suelo y nuestra velocidad vertical es negativa...
        if (isGrounded && playerVelocity.y < 0)
        {
            // Reseteamos la velocidad de caída. 
            // Usamos un valor pequeño (groundStickForce) en lugar de 0 
            // para asegurar que nos quedemos "pegados" al suelo.
            playerVelocity.y = groundStickForce;
        }

        // 3. Si NO estamos en el suelo, aplicamos la gravedad
        if (!isGrounded)
        {
            // Acumulamos la velocidad de caída ( v = g * t )
            playerVelocity.y += gravityValue * Time.deltaTime;
        }

        // 4. Aplicamos el movimiento de gravedad al controlador
        // Esto moverá al jugador hacia abajo ( d = v * t )
        // Los otros scripts (ContinuousMoveProvider) se encargarán del X y Z.
        // Nosotros solo manejamos el Y.
        controller.Move(playerVelocity * Time.deltaTime);
    }
}