using UnityEngine;
using TMPro;
using Assets.Scripts.GameEvents;

public class QuizManager : MonoBehaviour
{
    public GameObject[] panels;
    public TMP_Text resultText;

    [Header("Control de Movimiento")]
    public MonoBehaviour playerMovementScript;
    public MonoBehaviour cameraLookScript;

    [Header("Evento al finalizar completamente el quiz")]
    public GameEvent quizFinishedEvent;

    private int currentIndex = 0;
    private int correctCount = 0;
    private int wrongCount = 0;

    void Start()
    {
        if (playerMovementScript != null)
            playerMovementScript.enabled = false;

        if (cameraLookScript != null)
            cameraLookScript.enabled = false;

        for (int i = 0; i < panels.Length; i++)
            panels[i].SetActive(false);

        if (panels.Length > 0)
            panels[0].SetActive(true);

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

        // Aseguramos que no se pase el índice
        if (lastPanel >= 0)
            panels[lastPanel].SetActive(true);

        resultText.text = $"Respuestas correctas: {correctCount}\nRespuestas incorrectas: {wrongCount}";
    }

    public void HidePanel(GameObject panel)
    {
        panel.SetActive(false);

        if (playerMovementScript != null)
            playerMovementScript.enabled = true;

        if (cameraLookScript != null)
            cameraLookScript.enabled = true;

       Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (quizFinishedEvent != null)
            quizFinishedEvent.Raise();
    }
    public int GetCorrectCountForTest()
    {
        return correctCount;
    }

    public int GetWrongCountForTest()
    {
        return wrongCount;
    }
    public int GetCurrentIndexForTest()
    {
        return currentIndex;
    }
}
