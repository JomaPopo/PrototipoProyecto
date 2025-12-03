using UnityEngine;
using TMPro;
using Assets.Scripts.GameEvents; // Puedes borrar esta línea si ya no usas GameEvents

public class QuizManager : MonoBehaviour
{
    public GameObject[] panels;
    public TMP_Text resultText;

    

    private int currentIndex = 0;
    private int correctCount = 0;
    private int wrongCount = 0;

    // ¡El CPRManager llamará a esta función!
    public void StartQuiz()
    {
        // ¡ELIMINADO! El jugador YA está detenido.
        // if (playerMovementScript != null) ...
        // if (cameraLookScript != null) ...

        currentIndex = 0;
        correctCount = 0;
        wrongCount = 0;

        for (int i = 0; i < panels.Length; i++)
            panels[i].SetActive(false);

        if (panels.Length > 0)
            panels[0].SetActive(true); // Mostramos el primer panel (Intro o Pregunta 1)

        // ¡ELIMINADO! El cursor YA está libre.
        // Cursor.lockState = CursorLockMode.None;
        // Cursor.visible = true;
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
        if (currentIndex < panels.Length)
            panels[currentIndex].SetActive(false);

        currentIndex++;

        // Ahora: si quedan preguntas
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
        int lastPanel = panels.Length - 1;
        if (lastPanel >= 0)
            panels[lastPanel].SetActive(true);

        resultText.text = $"Respuestas correctas: {correctCount}\nRespuestas incorrectas: {wrongCount}";
    }

    /// <summary>
    /// Esta función debe ser llamada por el botón "Continuar" o "Finalizar"
    /// del ÚLTIMO panel (el de resultados).
    /// </summary>
    public void HidePanel(GameObject panel)
    {
        panel.SetActive(false);

        string contextoFinal = "CONTEXTO: Cuestionario completado. ¡Eres un salvavidas certificado!";
        string instruccionFinal = "Puedes explorar la carpa médica para aprender más, o presionar el boton de [Menú] y 'Salir' para terminar.";

        // Asumiendo que UIManager tiene las funciones correctas (Contexto + Instrucción)
        UIManager.Instance.ShowWristContext(contextoFinal);
        UIManager.Instance.ShowWristInstruction_Instant(instruccionFinal);
        GameManager.Instance.TriggerVictoria();
    }

    // --- (Tus funciones de Test no cambian) ---
    public int GetCorrectCountForTest() { return correctCount; }
    public int GetWrongCountForTest() { return wrongCount; }
    public int GetCurrentIndexForTest() { return currentIndex; }
}