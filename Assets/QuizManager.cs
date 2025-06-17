using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuizManager : MonoBehaviour
{
    [Tooltip("Paneles: 0=Presentación, 1-4=Preguntas A-D, 5=Resultados")]
    public GameObject[] panels;
    

    [Header("Resultados")]
    public TMP_Text resultText;

    private int currentIndex = 0;
    private int correctCount = 0;
    private int wrongCount = 0;

    void Start()
    {
        // Desactivar todos los paneles y mostrar solo el de presentación
        foreach (var panel in panels)
            panel.SetActive(false);

        if (panels.Length > 0)
            panels[0].SetActive(true);
    }

    /// <summary>
    /// Llamar desde el botón de "Continuar" del panel de presentación.
    /// </summary>
    public void OnContinue()
    {
        ShowNextPanel();
    }

    /// <summary>
    /// Llamar en OnClick de cada botón de respuesta: true para correcta, false para incorrecta.
    /// </summary>
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
        // Ocultar panel actual
        panels[currentIndex].SetActive(false);
        currentIndex++;

        // Si no es el panel de resultados, mostrar siguiente pregunta
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
        // Mostrar panel de resultados
        panels[panels.Length - 1].SetActive(true);
        resultText.text = $"Respuestas correctas: {correctCount}\n" +
                          $"Respuestas incorrectas: {wrongCount}";
    }

    public void HidePanel(GameObject panel)
    {
        panel.SetActive(false);
    }


}
