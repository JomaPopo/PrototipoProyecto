using UnityEngine;
using System.Collections;
using System; // ¡Importante! Para usar 'Action'

public class PauseManager : Singleton<PauseManager>
{
    [Header("Referencias del Jugador (Arrastrar)")]
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private MouseLook playerMouseLook;

    // Función pública principal para iniciar una pausa
    public void StartControlledPause(float duration, Action onPauseComplete)
    {
        StartCoroutine(PauseCoroutine(duration, onPauseComplete));
    }

    private IEnumerator PauseCoroutine(float duration, Action onPauseComplete)
    {
        Debug.Log($"PAUSE MANAGER: Iniciando pausa de {duration} segundos.");

        // 1. Desactivamos los controles
        if (playerMovement != null) playerMovement.enabled = false;
        if (playerMouseLook != null) playerMouseLook.DisableLook(); // Esto también libera el cursor

        // 2. Esperamos el tiempo
        yield return new WaitForSeconds(duration);

        // 3. Reactivamos los controles
        if (playerMovement != null) playerMovement.enabled = true;
        if (playerMouseLook != null) playerMouseLook.EnableLook(); // Esto bloquea el cursor

        Debug.Log("PAUSE MANAGER: Pausa terminada. Devolviendo el control.");

        // 4. Avisamos al script que nos llamó (RescueManager) que ya hemos terminado
        onPauseComplete?.Invoke();
    }

    // Función especial si solo queremos liberar el cursor (para los botones)
    public void FreeCursorForUI()
    {
        if (playerMovement != null) playerMovement.enabled = false;
        if (playerMouseLook != null) playerMouseLook.DisableLook();
    }

    // Función especial para devolver el control (después de los botones)
    public void RegainControlFromUI()
    {
        if (playerMovement != null) playerMovement.enabled = true;
        if (playerMouseLook != null) playerMouseLook.EnableLook();
    }
}