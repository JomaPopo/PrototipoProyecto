using UnityEngine;
using UnityEngine.EventSystems;

public class DoubleClickButton : MonoBehaviour
{
    [Header("Click Settings")]
    [SerializeField] private GameObject targetObject1; // Primer objeto a activar
    [SerializeField] private GameObject targetObject2; // Segundo objeto a activar

    [Header("Audio")]
    [SerializeField] private AudioSource clickAudioSource; // Sonido al hacer clic

    [Header("C�mara")]
    [SerializeField] private Camera camara; // C�mara que se mover�

    [Header("Sistema de pasos")]
    [SerializeField] private RecommendationsSteps pasosManager; // Referencia al sistema de pasos

    [Header("Control de rotaci�n de c�mara")]
    [SerializeField] private RaycastDetector raycastDetector; // ? Nueva referencia al script de rotaci�n

    private int clickCount = 0;

    public void OnButtonClick()
    {
        // Reproducir sonido
        if (clickAudioSource != null)
            clickAudioSource.Play();

        clickCount++;

        if (clickCount == 2)
        {
            // Activar ambos objetos
            if (targetObject1 != null)
                targetObject1.SetActive(true);

            if (targetObject2 != null)
                targetObject2.SetActive(true);

            // Mover c�mara a posici�n fija
            if (camara != null)
            {
                camara.transform.position = new Vector3(-82.3f, 0.7f, -3.788f);
                camara.transform.rotation = Quaternion.identity;
            }

            // Mover al siguiente paso
            if (pasosManager != null)
                pasosManager.Siguiente();

            // Desactivar rotaci�n de c�mara
            if (raycastDetector != null)
                raycastDetector.activarRotacion = false;

            // Reiniciar contador
            clickCount = 0;
        }

        // Evitar que Enter lo vuelva a activar
        EventSystem.current.SetSelectedGameObject(null);
    }
}
