using UnityEngine;
using UnityEngine.EventSystems;

public class DoubleClickButton : MonoBehaviour
{
    [Header("Click Settings")]
    [SerializeField] private GameObject targetObject1; // Primer objeto a activar
    [SerializeField] private GameObject targetObject2; // Segundo objeto a activar

    [Header("Audio")]
    [SerializeField] private AudioSource clickAudioSource; // Sonido al hacer clic

    [Header("Cámara")]
    [SerializeField] private Camera camara; // Cámara que se moverá

    [Header("Sistema de pasos")]
    [SerializeField] private RecommendationsSteps pasosManager; // Referencia al sistema de pasos

    [Header("Control de rotación de cámara")]
    [SerializeField] private RaycastDetector raycastDetector; // ? Nueva referencia al script de rotación

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

            // Mover cámara a posición fija
            if (camara != null)
            {
                camara.transform.position = new Vector3(-82.3f, 0.7f, -3.788f);
                camara.transform.rotation = Quaternion.identity;
            }

            // Mover al siguiente paso
            if (pasosManager != null)
                pasosManager.Siguiente();

            // Desactivar rotación de cámara
            if (raycastDetector != null)
                raycastDetector.activarRotacion = false;

            // Reiniciar contador
            clickCount = 0;
        }

        // Evitar que Enter lo vuelva a activar
        EventSystem.current.SetSelectedGameObject(null);
    }
}
