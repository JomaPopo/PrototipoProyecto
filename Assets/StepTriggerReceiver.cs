using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class StepTriggerReceiver : MonoBehaviour
{
    public RecommendationsSteps stepsManager;
    public AudioClip activationSound;
    public GameObject objetoAActivar; // ?? Objeto que se activará al ejecutar ActivarPaso

    private AudioSource audioSource;
    private bool yaActivado = false;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void ActivarPaso()
    {
        if (yaActivado) return;

        yaActivado = true;

        if (activationSound != null)
            audioSource.PlayOneShot(activationSound);

        if (objetoAActivar != null)
            objetoAActivar.SetActive(true); // ?? Activar el objeto oculto

        if (stepsManager != null)
            stepsManager.Siguiente();
    }

    // Llamar esto si quieres permitir reactivar
    public void ResetActivacion()
    {
        yaActivado = false;
    }
}
