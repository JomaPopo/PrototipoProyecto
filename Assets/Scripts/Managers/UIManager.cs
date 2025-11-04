using UnityEngine;
using TMPro;
using System.Collections;
using System.Text;
using UnityEngine.UI; 

public class UIManager : Singleton<UIManager>
{
    [Header("Feedback Rápido (Tipeo)")]
    [SerializeField] private TextMeshProUGUI instructionText;
    [SerializeField] private float typingSpeed = 0.05f;

    [Header("Panel de Pausa con Confirmación")]
    [SerializeField] private GameObject pauseInstructionPanel;
    [SerializeField] private TextMeshProUGUI panelInstructionText;
    [SerializeField] private Button panelOkButton; // ¡NUEVO! Referencia al botón "Ok"

    [Header("Otros Elementos (Opcional)")]
    [SerializeField] private GameObject crosshair;

    private Coroutine typingCoroutine;
    private Coroutine panelTypingCoroutine;
    private readonly StringBuilder stringBuilder = new StringBuilder();

    [Header("UI de Muñeca (VR)")]
    [SerializeField] private GameObject wristCommunicatorCanvas;
    [SerializeField] private TextMeshProUGUI wristText;
    [SerializeField] private TextMeshProUGUI wristTimerText;

    protected override void Awake()
    {
        base.Awake();
        if (instructionText != null) instructionText.gameObject.SetActive(false);
        if (pauseInstructionPanel != null) pauseInstructionPanel.SetActive(false);
        if (crosshair != null) crosshair.SetActive(true);
    }

    // --- Sistema de Tipeo (Feedback rápido) ---
    public void ShowInstruction(string message)
    {
        HidePausePanel();
        if (instructionText != null)
        {
            StopAllTypingCoroutines();
            typingCoroutine = StartCoroutine(TypeText(instructionText, message, null)); // No necesita botón Ok
        }
    }

    public void HideInstructions()
    {
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        if (instructionText != null) instructionText.gameObject.SetActive(false);
        typingCoroutine = null; // Asegurarse de resetear
    }

    // --- Sistema de Panel de Pausa (con tipeo y botón retardado) ---
    public void ShowPausePanel(string message)
    {
        HideInstructions();
        if (pauseInstructionPanel == null || panelInstructionText == null || panelOkButton == null)
        {
            Debug.LogError("UIManager: El panel de pausa, su texto o su botón 'Ok' no están asignados.");
            return;
        }
        GameManager.Instance.PausarReloj();
        panelInstructionText.text = "";
        panelOkButton.gameObject.SetActive(false); // Ocultamos el botón al inicio
        pauseInstructionPanel.SetActive(true);
        if (crosshair != null) crosshair.SetActive(false);
        Debug.Log($"UIManager: Mostrando panel de pausa y tipeando: '{message}'");

        StopAllTypingCoroutines();
        // Iniciamos el tipeo para el panel, PASÁNDOLE la referencia al botón Ok
        panelTypingCoroutine = StartCoroutine(TypeText(panelInstructionText, message, panelOkButton));
    }

    public void HidePausePanel()
    {
        StopAllTypingCoroutines(); // Detenemos cualquier tipeo

        if (pauseInstructionPanel != null && pauseInstructionPanel.activeSelf)
        {
            pauseInstructionPanel.SetActive(false);
            if (crosshair != null) crosshair.SetActive(true);
            Debug.Log("UIManager: Panel de pausa oculto.");

            // Asegúrate de que esta línea esté descomentada si usas el PauseManager
            RescueManager.Instance.OnPausePanelAcknowledged();
        }
        GameManager.Instance.IniciarReloj();

    }

    
    private IEnumerator TypeText(TextMeshProUGUI textComponent, string message, Button okButtonToShow)
    {
        var waitInstruction = new WaitForSeconds(typingSpeed);
        textComponent.gameObject.SetActive(true);
        textComponent.text = "";
        stringBuilder.Clear();

        for (int i = 0; i < message.Length; i++)
        {
            stringBuilder.Append(message[i]);
            textComponent.text = stringBuilder.ToString();
            yield return waitInstruction;
        }

        
        if (okButtonToShow != null)
        {
            okButtonToShow.gameObject.SetActive(true);
            Debug.Log("UIManager: Tipeo completo, botón 'Ok' activado.");
        }

        if (textComponent == instructionText) typingCoroutine = null;
        if (textComponent == panelInstructionText) panelTypingCoroutine = null;
    }

    private void StopAllTypingCoroutines()
    {
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        if (panelTypingCoroutine != null) StopCoroutine(panelTypingCoroutine);
        typingCoroutine = null;
        panelTypingCoroutine = null;
    }
    public void ShowWristAlert(string message, string timerMessage)
    {
        if (wristCommunicatorCanvas != null)
            wristCommunicatorCanvas.SetActive(true);

        if (wristText != null)
            wristText.text = message;

        if (wristTimerText != null)
            wristTimerText.text = timerMessage;
    }
    public void ShowWristObjective(string message)
    {
        if (wristText != null)
            wristText.text = message;
    }
}