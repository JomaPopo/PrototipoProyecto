using UnityEngine;

// ¡Este script va EN CADA GameObject de botón (hombrosButton, frenteButton, etc.)!
public class InteractionButton : MonoBehaviour
{
    [Header("Tipo de Parte del Cuerpo")]
    [Tooltip("Elige qué parte representa este botón.")]
    public BodyPart partType;

    // Guardaremos la cámara del jugador aquí
    private Transform playerCameraTransform;

    void Awake()
    {
        // 1. Buscamos la cámara del jugador automáticamente
        // (Asegúrate de que tu cámara de VR/PC tenga el tag "MainCamera")
        try
        {
            playerCameraTransform = Camera.main.transform;
        }
        catch (System.Exception)
        {
            // Este error solo saldrá una vez por botón, ¡pero es importante!
            Debug.LogError($"¡No se encontró una cámara con el tag 'MainCamera'! El botón de {partType} no podrá mirar al jugador.");
        }
    }

    /// <summary>
    /// Esta función se llama CADA FRAME (después de Update)
    /// ¡Aquí es donde hacemos el "Look At" vertical!
    /// </summary>
    void LateUpdate()
    {
        // Si no encontramos la cámara, no hacemos nada
        if (playerCameraTransform == null) return;

        // 1. Obtenemos la dirección del botón a la cámara
        Vector3 directionToCamera = playerCameraTransform.position - transform.position;

        // 2. --- ¡LA MAGIA! ---
        // Anulamos la 'Y'. Esto fuerza al botón a "pararse" verticalmente
        // y solo rotar horizontalmente (en su eje Y) para mirarte.
        // Esto resuelve el problema de que el NPC (y el canvas) esté "echado".
        directionToCamera.y = 0;

        // Si la cámara está justo encima, evitamos un error
        if (directionToCamera == Vector3.zero) return;

        // 3. Creamos la rotación para que mire al jugador
        // (Usamos -direction porque el "frente" de un Canvas es su Z negativa)
        Quaternion targetRotation = Quaternion.LookRotation(-directionToCamera);

        // 4. Aplicamos la rotación
        transform.rotation = targetRotation;
    }

    /// <summary>
    /// ¡Esta es tu función original! No se toca.
    /// Se llama desde el inspector del botón (en el evento OnClick).
    /// </summary>
    public void OnPressed()
    {
        Debug.Log($"Se ha presionado el botón de: {partType}");
        RescueManager.Instance.BodyPartInteracted(partType);
    }
}

