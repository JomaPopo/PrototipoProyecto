using UnityEngine;
using TMPro;
using UnityEngine.Profiling; // Necesario para medir la memoria

public class MonitorRendimiento_QA : MonoBehaviour
{
    public TextMeshProUGUI textoFPS;
    public TextMeshProUGUI textoMemoria; // Añade un nuevo texto para la memoria

    private float tiempoRefresco = 0.5f;
    private float temporizador;
    private int contadorFrames;

    void Update()
    {
        if (Time.unscaledTime > temporizador)
        {
            // --- Cálculo de FPS (igual que antes) ---
            int fps = (int)(contadorFrames / tiempoRefresco);
            textoFPS.text = "FPS: " + fps;

            // --- Nuevo: Cálculo de Memoria ---
            long memoriaUsadaBytes = Profiler.GetTotalAllocatedMemoryLong();
            float memoriaUsadaMB = memoriaUsadaBytes / 1024f / 1024f;
            textoMemoria.text = "Memoria: " + memoriaUsadaMB.ToString("F1") + " MB";

            // Reinicia el contador
            temporizador = Time.unscaledTime + tiempoRefresco;
            contadorFrames = 0;
        }
        else
        {
            contadorFrames++;
        }
    }
}