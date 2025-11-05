using UnityEngine;
using TMPro;
using System.Collections;
using System.Text;
using UnityEngine.UI;
using System;

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
    [SerializeField] private TextMeshProUGUI wristChecklistText;
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
        panelTypingCoroutine = StartCoroutine(TypeText(panelInstructionText, message, panelOkButton));
    }
    public void HidePausePanel()
    {
        StopAllTypingCoroutines();

        if (pauseInstructionPanel != null && pauseInstructionPanel.activeSelf)
        {
            pauseInstructionPanel.SetActive(false);
            if (crosshair != null) crosshair.SetActive(true);
            Debug.Log("UIManager: Panel de pausa oculto.");

            RescueManager.Instance.OnPausePanelAcknowledged();
        }
        GameManager.Instance.IniciarReloj();

    }


    private IEnumerator TypeText(TextMeshProUGUI textComponent, string message, Button okButtonToShow, Action onCompleteCallback = null)
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
        onCompleteCallback?.Invoke();

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
        {
            StopAllTypingCoroutines();
            typingCoroutine = StartCoroutine(TypeText(wristText, message, null));
        }

        if (wristTimerText != null)
            wristTimerText.text = timerMessage;

        if (wristChecklistText != null)
            wristChecklistText.gameObject.SetActive(false);
    }
    public void ShowChecklistStep(string instructionMessage, string checklistMessage, Action onTypingFinished)
    {
        // 1. Pone el CHECKLIST "de frente" (como ya lo tenías)
        if (wristChecklistText != null)
        {
            wristChecklistText.gameObject.SetActive(true);
            wristChecklistText.text = checklistMessage;
        }

        // 2. Pone la INSTRUCCIÓN DETALLADA usando tu tipeo
        if (wristText != null)
        {
            StopAllTypingCoroutines();

            // ¡LE PASAMOS LA FUNCIÓN DE "AVISO"!
            typingCoroutine = StartCoroutine(TypeText(wristText, instructionMessage, null, onTypingFinished));
        }
    }
    public void ShowChecklistStep(string instructionMessage, string checklistMessage)
    {
        if (wristChecklistText != null)
        {
            wristChecklistText.gameObject.SetActive(true);
            wristChecklistText.text = checklistMessage;
        }

        if (wristText != null)
        {
            StopAllTypingCoroutines();
            typingCoroutine = StartCoroutine(TypeText(wristText, instructionMessage, null));
        }
    }
    public void ShowChecklistStep(string message)
    {
        if (wristText != null)
            wristText.text = message;
    }
}