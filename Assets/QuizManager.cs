using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuizManager : MonoBehaviour
{
    [Tooltip("Paneles: 0=Presentación, 1-4=Preguntas A-D, 5=Resultados")]
    public GameObject[] panels;

    [Header("Resultados")]
    public TMP_Text resultText;

    [Header("Control de Movimiento")]
    public MonoBehaviour playerMovementScript;
    public MonoBehaviour cameraLookScript;

    private int currentIndex = 0;
    private int correctCount = 0;
    private int wrongCount = 0;

    void Start()
    {
        // Desactivar movimiento y cámara al inicio
        if (playerMovementScript != null)
            playerMovementScript.enabled = false;

        if (cameraLookScript != null)
            cameraLookScript.enabled = false;

        // Mostrar solo el panel de presentación
        foreach (var panel in panels)
            panel.SetActive(false);

        if (panels.Length > 0)
            panels[0].SetActive(true);

        // Asegurar que el cursor esté visible al inicio
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void OnContinue()
    {
        ShowNextPanel();
    }

    public void OnOptionSelected(bool isCorrect)
    {
        if (isCorrect)
            correctCount++;
        else
            wrongCount++;

        ShowNextPanel();
    }

    private void ShowNextPanel()
    {
        panels[currentIndex].SetActive(false);
        currentIndex++;

        if (currentIndex < panels.Length - 1)
        {
            panels[currentIndex].SetActive(true);
        }
        else
        {
            ShowResultsPanel();
        }
    }

    private void ShowResultsPanel()
    {
        panels[panels.Length - 1].SetActive(true);
        resultText.text = $"Respuestas correctas: {correctCount}\n" +
                          $"Respuestas incorrectas: {wrongCount}";

        // El jugador podrá moverse solo cuando cierre este panel usando un botón
    }

    public void HidePanel(GameObject panel)
    {
        panel.SetActive(false);

        // ? Activar movimiento y cámara
        if (playerMovementScript != null)
            playerMovementScript.enabled = true;

        if (cameraLookScript != null)
            cameraLookScript.enabled = true;

        // ? Bloquear y ocultar el cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
