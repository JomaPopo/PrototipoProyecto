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
    [SerializeField] private Button panelOkButton; 

    [Header("UI del Checklist")]
    [Tooltip("Arrastra aquí los 4 Toggles del checklist en orden")]
    public Toggle[] checklistSteps;
    [SerializeField] private GameObject checklistPanel;

    [Header("Otros Elementos (Opcional)")]
    [SerializeField] private GameObject crosshair;

    private Coroutine typingCoroutine;
    private Coroutine panelTypingCoroutine;
    private readonly StringBuilder stringBuilder = new StringBuilder();

    [Header("UI de Muñeca (VR)")]
    [SerializeField] private GameObject wristCommunicatorCanvas;
    [SerializeField] private TextMeshProUGUI wristText;
    [SerializeField] private TextMeshProUGUI wristTimerText;
    [SerializeField] private TextMeshProUGUI wristContextText; // ¡Para el CONTEXTO!
    
    protected override void Awake()
    {
        base.Awake();
        if (instructionText != null) instructionText.gameObject.SetActive(false);
        if (pauseInstructionPanel != null) pauseInstructionPanel.SetActive(false);
        if (crosshair != null) crosshair.SetActive(true);
        if (wristContextText != null) wristContextText.gameObject.SetActive(false);
        if (checklistPanel != null) checklistPanel.SetActive(false);
    }

    public void CompleteChecklistStep(int stepIndex)
    {
        if (checklistSteps != null && stepIndex < checklistSteps.Length)
        {
            // ¡Aquí es donde se marca el check!
            checklistSteps[stepIndex].isOn = true;
        }
    }
    public void ShowWristContext(string message)
    {
        // Asegura que el panel padre esté activo
        if (wristCommunicatorCanvas != null)
            wristCommunicatorCanvas.SetActive(true);

        if (wristContextText != null)
        {
            wristContextText.text = message;
            wristContextText.gameObject.SetActive(true);
        }
    }
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

    }
    public void ShowWristInstruction_Instant(string instructionMessage)
    {
        if (wristCommunicatorCanvas != null)
            wristCommunicatorCanvas.SetActive(true);

        if (wristText != null)
        {
            // ¡Detiene cualquier tipeo anterior!
            StopAllTypingCoroutines();

            // ¡Pone el texto de frente!
            wristText.text = instructionMessage;
            wristText.gameObject.SetActive(true);
        }
    }
    public void ShowWristInstruction(string instructionMessage, Action onTypingFinished = null)
    {
        if (wristCommunicatorCanvas != null)
            wristCommunicatorCanvas.SetActive(true);

        if (wristText != null)
        {
            StopAllTypingCoroutines();
            typingCoroutine = StartCoroutine(TypeText(wristText, instructionMessage, null, onTypingFinished));
        }
    }

    public void HideWristContext()
    {
        if (wristContextText != null)
            wristContextText.gameObject.SetActive(false);
    }
    public void ShowChecklistPanel()
    {
        if (checklistPanel != null)
            checklistPanel.SetActive(true);
    }

  

  
   


}