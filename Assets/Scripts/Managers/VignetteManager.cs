using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System.Collections;

public class VignetteManager : Singleton<VignetteManager>
{
    [Header("Configuración de Post-Procesado")]
    [Tooltip("Arrastra aquí el GameObject que contiene tu Global Volume")]
    [SerializeField] private Volume postProcessVolume;

    private Vignette vignette;
    private Coroutine activeVignetteCoroutine;

    private void Start()
    {
        if (postProcessVolume != null)
        {
            postProcessVolume.profile.TryGet(out vignette);
        }

        if (vignette == null)
        {
            Debug.LogError("¡Vignette no encontrado en el Volume Profile! Asegúrate de que el Volume esté asignado y tenga un override de Vignette.");
            this.enabled = false;
        }
        else
        {
            // Asegúrate de que la viñeta esté inactiva al empezar
            vignette.active = false;
        }
    }

    // --- Tu función original (la dejamos por si la usas en otro lado) ---
    public void ShowVignette(float duration, Color color, float intensity)
    {
        if (vignette != null)
        {
            if (activeVignetteCoroutine != null)
            {
                StopCoroutine(activeVignetteCoroutine);
            }
            activeVignetteCoroutine = StartCoroutine(VignetteEffect(duration, color, intensity));
        }
    }

    private IEnumerator VignetteEffect(float duration, Color color, float intensity)
    {
        vignette.color.Override(color);
        vignette.intensity.Override(intensity);
        vignette.active = true;

        yield return new WaitForSeconds(duration);

        vignette.active = false;
        activeVignetteCoroutine = null;
    }

    // --- ¡NUEVA FUNCIÓN DE ALERTA! ---
    // Esta función es para el parpadeo rojo de emergencia
    public void TriggerEmergencyFlash(float duration, Color color, float maxIntensity)
    {
        if (vignette == null) return;

        if (activeVignetteCoroutine != null)
        {
            StopCoroutine(activeVignetteCoroutine);
        }

        // Llamamos a la nueva corutina de parpadeo
        activeVignetteCoroutine = StartCoroutine(FlashVignetteEffect(duration, color, maxIntensity));
    }

    // --- ¡NUEVA CORUTINA DE PARPADEO! ---
    private IEnumerator FlashVignetteEffect(float duration, Color color, float maxIntensity)
    {
        vignette.color.Override(color);
        vignette.active = true; // La activamos solo una vez

        float timer = 0;
        bool flashingOn = true;

        // Mientras dure la alerta
        while (timer < duration)
        {
            // Parpadea la intensidad (entre maxIntensity y la mitad)
            vignette.intensity.Override(flashingOn ? maxIntensity : maxIntensity / 2f);
            flashingOn = !flashingOn;

            timer += 0.25f; // Controla la velocidad del parpadeo (4 veces por seg)
            yield return new WaitForSeconds(0.25f);
        }

        // Al terminar, apágalo
        vignette.active = false;
        activeVignetteCoroutine = null;
    }
    public void TriggerPulse(Color color, float intensity = 0.45f, float duration = 0.2f)
    {
        if (vignette == null) return;
        if (activeVignetteCoroutine != null) StopCoroutine(activeVignetteCoroutine);

        activeVignetteCoroutine = StartCoroutine(PulseRoutine(duration,color, intensity));
    }

    private IEnumerator PulseRoutine(float duration, Color color, float intensity)
    {
        // 1. Configurar
        vignette.color.Override(color);
        vignette.intensity.Override(intensity);
        vignette.active = true;

        // 2. Esperar
        yield return new WaitForSeconds(duration);

        // 3. Apagar
        vignette.active = false;
        activeVignetteCoroutine = null;
    }

    /// <summary>
    /// Mantiene la viñeta encendida (ideal para feedback de error continuo).
    /// </summary>
    public void SetContinuousFeedback(Color color)
    {
        if (vignette == null) return;
        if (activeVignetteCoroutine != null) StopCoroutine(activeVignetteCoroutine);

        vignette.color.Override(color);
        vignette.intensity.Override(0.5f); // Un poco más intenso para errores
        vignette.active = true;
    }
    public void SetCPRFeedback(Color color)
    {
        if (vignette == null) return;

        // Si había un flash ocurriendo, lo detenemos para que no interfiera
        if (activeVignetteCoroutine != null) StopCoroutine(activeVignetteCoroutine);

        vignette.color.Override(color);
        vignette.intensity.Override(0.45f); // Intensidad fija (ajusta a tu gusto, 0.45 es visible pero no ciega)
        vignette.active = true;
    }

    /// <summary>
    /// Apaga la viñeta completamente.
    /// </summary>
    public void ResetVignette()
    {
        if (vignette == null) return;

        if (activeVignetteCoroutine != null) StopCoroutine(activeVignetteCoroutine);

        vignette.active = false;
    }
}