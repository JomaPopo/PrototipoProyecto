using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.InputSystem; 

public class CPRManager : MonoBehaviour
{
    [Header("Configuración del RCP")]
    [SerializeField] private float beatsPerMinute = 110f;
    [SerializeField] private float timingWindow = 0.15f;
    [SerializeField] private float perfectHoldTime = 0.2f;
    [SerializeField] private float holdTimeWindow = 0.08f;

    [Header("Referencias de UI")]
    [SerializeField] private GameObject cprPanel;
    [SerializeField] private TextMeshProUGUI compressionCountText;
    [SerializeField] private Image rhythmGuideIndicator;
    [SerializeField] private TextMeshProUGUI feedbackText;
    [SerializeField] private Slider qualityBar;

    [Header("Referencias del Jugador (Arrastrar en Inspector)")]
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private MouseLook mouseLook;

    private int compressionCount = 0;
    private float beatInterval;
    private float nextBeatTime;
    private bool canPress = false;
    private bool inputPressedThisBeat = false;
    private float pressStartTime;
    private float totalQualityScore = 0;

    private bool isLeftHandDown = false;
    private bool isRightHandDown = false;

    public void StartCPR()
    {
        Debug.Log("Iniciando mini-juego de RCP...");
        cprPanel.SetActive(true);
        compressionCount = 0;
        totalQualityScore = 0;
        UpdateUI();

        beatInterval = 60f / beatsPerMinute;
        nextBeatTime = Time.time + beatInterval;

        if (playerMovement != null) playerMovement.enabled = false;
        if (mouseLook != null) mouseLook.DisableLook();
    }

    void Update()
    {
        if (Time.time >= nextBeatTime)
        {
            StartCoroutine(RhythmPulse());
            canPress = true;
            inputPressedThisBeat = false;
            nextBeatTime += beatInterval;
        }
        else if (Time.time >= nextBeatTime - timingWindow)
        {
            canPress = true;
        }
        else
        {
            canPress = false;
        }
    }

    public void OnCPRLeft(InputAction.CallbackContext context)
    {
        if (context.performed) 
        {
            isLeftHandDown = true;
            CheckBothHandsPressed();
        }
        else if (context.canceled) 
        {
            isLeftHandDown = false;
            CheckBothHandsReleased();
        }
    }

    public void OnCPRRight(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            isRightHandDown = true;
            CheckBothHandsPressed();
        }
        else if (context.canceled)
        {
            isRightHandDown = false;
            CheckBothHandsReleased();
        }
    }

    private void CheckBothHandsPressed()
    {
        if (!canPress || inputPressedThisBeat || !(isLeftHandDown && isRightHandDown))
        {
            return;
        }

        pressStartTime = Time.time;
        Debug.Log("Ambas manos presionadas.");
    }

    private void CheckBothHandsReleased()
    {
        if (inputPressedThisBeat) return;

        if (pressStartTime > 0)
        {
            float holdDuration = Time.time - pressStartTime;
            Debug.Log("Compresión completada. Duración: " + holdDuration);
            CheckCompressionQuality(holdDuration);
            inputPressedThisBeat = true; 
            pressStartTime = 0; 
            isLeftHandDown = false;
            isRightHandDown = false;
        }
    }

    private void CheckCompressionQuality(float holdDuration)
    {
        if (Time.time > nextBeatTime + timingWindow)
        {
            ShowFeedback("¡MUY LENTO!", Color.red);
            totalQualityScore -= 5;
            return;
        }
        if (holdDuration < perfectHoldTime - holdTimeWindow)
        {
            ShowFeedback("¡MÁS PROFUNDO!", Color.yellow);
            totalQualityScore += 5;
        }
        else if (holdDuration > perfectHoldTime + holdTimeWindow)
        {
            ShowFeedback("¡NO TE APOYES!", Color.yellow);
            totalQualityScore += 5;
        }
        else
        {
            ShowFeedback("¡PERFECTO!", Color.green);
            totalQualityScore += 10;
        }
        compressionCount++;
        UpdateUI();
        if (compressionCount >= 30)
        {
            EndCPRSequence();
        }
    }

    private void EndCPRSequence()
    {
        Debug.Log("Ciclo de 30 compresiones terminado. Puntuación: " + totalQualityScore);
        cprPanel.SetActive(false);
        if (playerMovement != null) playerMovement.enabled = true;
        if (mouseLook != null) mouseLook.EnableLook();
        this.enabled = false;
    }

    private void UpdateUI()
    {
        compressionCountText.text = $"{compressionCount} / 30";
        qualityBar.value = totalQualityScore / 300f;
    }

    private void ShowFeedback(string message, Color color)
    {
        feedbackText.text = message;
        feedbackText.color = color;
        feedbackText.gameObject.SetActive(true);
        Invoke(nameof(HideFeedback), 0.5f);
    }
    private void HideFeedback()
    {
        feedbackText.gameObject.SetActive(false);
    }

    private IEnumerator RhythmPulse()
    {
        rhythmGuideIndicator.color = Color.white;
        yield return new WaitForSeconds(0.1f);
        rhythmGuideIndicator.color = new Color(1, 1, 1, 0.5f);
    }
}