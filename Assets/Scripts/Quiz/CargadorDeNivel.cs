using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // Para la barra de carga
using System.Collections;

public class CargadorDeNivel : MonoBehaviour
{
    public GameObject pantallaDeCarga;
    public Slider barraDeCarga;

    // Llama a esta función desde tu botón
    public void CargarNivel(string nombreDeEscena)
    {
        pantallaDeCarga.SetActive(true);
        StartCoroutine(CargarEscenaAsync(nombreDeEscena));
    }

    IEnumerator CargarEscenaAsync(string nombreDeEscena)
    {
        // Inicia la carga en segundo plano
        AsyncOperation operacion = SceneManager.LoadSceneAsync(nombreDeEscena);

        // Evita que la escena se active sola al terminar
        operacion.allowSceneActivation = false;

        // Mientras la escena se carga...
        while (!operacion.isDone)
        {
            // operacion.progress va de 0.0 a 0.9
            float progreso = Mathf.Clamp01(operacion.progress / 0.9f);
            barraDeCarga.value = progreso;

            // Cuando la carga llega al 90% (0.9), ya está lista
            if (operacion.progress >= 0.9f)
            {
                // Aquí podrías poner un texto "Presiona espacio para continuar"

                // Y luego permitir la activación
                operacion.allowSceneActivation = true;
            }

            yield return null;
        }
    }
}