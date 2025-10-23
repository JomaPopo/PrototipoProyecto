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
    }
    public void ShowVignette(float duration, Color color, float intensity )
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
}