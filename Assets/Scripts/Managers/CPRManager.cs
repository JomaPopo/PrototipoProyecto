using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.InputSystem;

public class CPRManager : MonoBehaviour
{
    [Header("Configuración del Ritmo")]
    [Tooltip("Cuánto 'sube' la barra con cada pulsación correcta.")]
    [SerializeField] private float rhythmIncreaseAmount = 5f;
    [Tooltip("Cuánto 'baja' la barra por segundo si no se presiona nada.")]
    [SerializeField] private float rhythmDecayRate = 15f;
    [Tooltip("El valor mínimo del rango perfecto.")]
    [SerializeField] private float targetZoneMin = 100f;
    [Tooltip("El valor máximo del rango perfecto.")]
    [SerializeField] private float targetZoneMax = 120f;
    [SerializeField] private float riseSmoothness = 10f;


    [Header("Referencias de UI")]
    [SerializeField] private GameObject cprPanel;
    [SerializeField] private TextMeshProUGUI compressionCountText;
    [SerializeField] private TextMeshProUGUI feedbackText;
    [SerializeField] private Slider qualityBar;
    [SerializeField] private Slider rhythmSlider;

    [Header("Referencias del Jugador")]
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private MouseLook mouseLook;

    // --- Variables de control ---
    private int compressionCount = 0;
    private float totalQualityScore = 0;
    private float currentRhythmValue = 0f; 
    private float targetRhythmValue = 0f;  
    private bool canPress = true;  

    private bool isLeftHandDown = false;
    private bool isRightHandDown = false;

    public void StartCPR()
    {
        cprPanel.SetActive(true);
        compressionCount = 0;
        totalQualityScore = 0;

        rhythmSlider.minValue = 80;
        rhythmSlider.maxValue = 140;

        // --- ¡AQUÍ ESTÁ EL CAMBIO! ---
        // Calculamos el punto medio de la zona objetivo para empezar ahí.
        float startingRhythmValue = (targetZoneMin + targetZoneMax) / 2f; // Ej: (100 + 120) / 2 = 110

        // Inicializamos ambos valores en el punto medio.
        targetRhythmValue = startingRhythmValue;
        currentRhythmValue = startingRhythmValue;

        UpdateUI();
        feedbackText.gameObject.SetActive(false);

        if (playerMovement != null) playerMovement.enabled = false;
        if (mouseLook != null) mouseLook.DisableLook();
    }

    void Update()
    {
        // 1. Aplicamos el decaimiento al valor REAL (target).
        if (targetRhythmValue > rhythmSlider.minValue)
        {
            targetRhythmValue -= rhythmDecayRate * Time.deltaTime;
        }
        targetRhythmValue = Mathf.Max(targetRhythmValue, rhythmSlider.minValue);

        // 2. El valor VISUAL (current) persigue suavemente al valor REAL. ¡AQUÍ ESTÁ LA MAGIA!
        currentRhythmValue = Mathf.Lerp(currentRhythmValue, targetRhythmValue, Time.deltaTime * riseSmoothness);

        // 3. Actualizamos la barra con el valor visual suavizado.
        rhythmSlider.value = currentRhythmValue;

        // 4. La lógica del juego usa el valor REAL para ser precisa.
        CheckRhythmQuality();
    }

    // --- MANEJO DE INPUTS ---
    public void OnCPRLeft(InputAction.CallbackContext context)
    {
        if (context.performed) isLeftHandDown = true;
        else if (context.canceled) isLeftHandDown = false;
        HandleInputState();
    }

    public void OnCPRRight(InputAction.CallbackContext context)
    {
        if (context.performed) isRightHandDown = true;
        else if (context.canceled) isRightHandDown = false;
        HandleInputState();
    }

    private void HandleInputState()
    {
        bool bothHandsPressed = isLeftHandDown && isRightHandDown;

        if (bothHandsPressed && canPress)
        {
            canPress = false;

            // "Bombeamos" el valor REAL (target).
            targetRhythmValue += rhythmIncreaseAmount;
            targetRhythmValue = Mathf.Min(targetRhythmValue, rhythmSlider.maxValue);

            compressionCount++;
            UpdateUI();
            if (compressionCount >= 100)
            {
                EndCPRSequence();
            }
        }
        else if (!bothHandsPressed)
        {
            canPress = true;
        }
    }

    // --- LÓGICA DE CALIDAD (Usa el valor real para precisión) ---
    private void CheckRhythmQuality()
    {
        // Usamos targetRhythmValue para que el feedback sea instantáneo y preciso.
        if (targetRhythmValue >= targetZoneMin && targetRhythmValue <= targetZoneMax)
        {
            totalQualityScore += Time.deltaTime * 5;
            ShowFeedback("¡RITMO PERFECTO!", Color.green);
        }
        else if (targetRhythmValue < targetZoneMin)
        {
            ShowFeedback("¡MÁS RÁPIDO!", Color.yellow);
        }
        else
        {
            ShowFeedback("¡MUY RÁPIDO!", Color.red);
        }
    }

    // --- FUNCIONES DE UI Y FIN ---
    private void EndCPRSequence()
    {
        cprPanel.SetActive(false);
        if (playerMovement != null) playerMovement.enabled = true;
        if (mouseLook != null) mouseLook.EnableLook();
        this.enabled = false;
    }

    private void UpdateUI()
    {
        compressionCountText.text = $"{compressionCount} / 100";
        qualityBar.value = totalQualityScore / 100f;
    }

    private void ShowFeedback(string message, Color color)
    {
        if (feedbackText.gameObject.activeInHierarchy && feedbackText.text == message) return;

        feedbackText.text = message;
        feedbackText.color = color;
        feedbackText.gameObject.SetActive(true);
    }
}