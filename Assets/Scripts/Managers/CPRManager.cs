using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.InputSystem;

public class CPRManager : MonoBehaviour
{
    [Header("Configuración del Ritmo")]
    [SerializeField] private float rhythmIncreaseAmount = 5f;
    [SerializeField] private float rhythmDecayRate = 15f;
    [SerializeField] private float targetZoneMin = 100f;
    [SerializeField] private float targetZoneMax = 120f;
    [SerializeField] private float riseSmoothness = 10f;

    [Header("Referencias de UI")]
    [SerializeField] private GameObject cprPanel;
    [SerializeField] private TextMeshProUGUI compressionCountText;
    [SerializeField] private TextMeshProUGUI feedbackText;
    [SerializeField] private Slider qualityBar;
    [SerializeField] private Slider rhythmSlider;

    [Header("Referencias del Cuestionario")]
    [SerializeField] private QuizManager quizManager;
    [SerializeField] private GameObject quizPanelGameObject;

    

    [Header("Feedback Visual (Manos 3D)")]
    [Tooltip("Arrastra el GameObject de las manos que están sobre el pecho")]
    [SerializeField] private GameObject handsModel;
    [Tooltip("Qué tanto bajan las manos (0.05 = 5cm en escala real)")]
    [SerializeField] private float compressionDepth = 0.05f;
    [SerializeField] private float handAnimationSpeed = 15f;

    private Vector3 handsInitialPos;
    private Vector3 handsPressedPos;

    private int compressionCount = 0;
    private float totalQualityScore = 0;
    private float currentRhythmValue = 0f;
    private float targetRhythmValue = 0f;
    private bool canPress = true;
    [SerializeField] private float minQualityToWin = 70f;

    private bool isLeftHandDown = false;
    private bool isRightHandDown = false;
    public NPCController currentVictim;

    void Awake()
    {
        

        if (handsModel != null)
        {
            handsInitialPos = handsModel.transform.localPosition;
            handsPressedPos = handsInitialPos - new Vector3(0, compressionDepth, 0);
            handsModel.SetActive(false); // Ocultas al principio
        }
    }

    public void StartCPR()
    {
        cprPanel.SetActive(true);

        if (handsModel != null) handsModel.SetActive(true);

        compressionCount = 0;
        totalQualityScore = 0;

        rhythmSlider.minValue = 80;
        rhythmSlider.maxValue = 140;

        float startingRhythmValue = (targetZoneMin + targetZoneMax) / 2f;
        targetRhythmValue = startingRhythmValue;
        currentRhythmValue = startingRhythmValue;

        UpdateUI();
        feedbackText.gameObject.SetActive(false);
    }

    void Update()
    {
        if (targetRhythmValue > rhythmSlider.minValue)
        {
            targetRhythmValue -= rhythmDecayRate * Time.deltaTime;
        }
        targetRhythmValue = Mathf.Max(targetRhythmValue, rhythmSlider.minValue);
        currentRhythmValue = Mathf.Lerp(currentRhythmValue, targetRhythmValue, Time.deltaTime * riseSmoothness);
        rhythmSlider.value = currentRhythmValue;

        CheckRhythmQuality();

        AnimateHands();
    }

    private void AnimateHands()
    {
        if (handsModel == null) return;

        Vector3 targetPos;

        if (canPress == false)
        {
            targetPos = handsPressedPos;
        }
        else
        {
            targetPos = handsInitialPos;
        }

        handsModel.transform.localPosition = Vector3.Lerp(handsModel.transform.localPosition, targetPos, Time.deltaTime * handAnimationSpeed);
    }

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

    private void CheckRhythmQuality()
    {
        if (targetRhythmValue >= targetZoneMin && targetRhythmValue <= targetZoneMax)
        {
            totalQualityScore += Time.deltaTime * 5;
            ShowFeedback("¡RITMO PERFECTO!", Color.green);
            VignetteManager.Instance.TriggerPulse(Color.green, 0.3f, 0.15f);
        }
        else if (targetRhythmValue < targetZoneMin)
        {
            ShowFeedback("¡MÁS RÁPIDO!", Color.yellow);
            VignetteManager.Instance.SetContinuousFeedback(Color.yellow);
        }
        else
        {
            ShowFeedback("¡MUY RÁPIDO!", Color.red);
            VignetteManager.Instance.SetContinuousFeedback(Color.red);
        }
    }

    private void EndCPRSequence()
    {
        cprPanel.SetActive(false);

        if (handsModel != null) handsModel.SetActive(false);
        if (CrowdManager.Instance != null)
        {
            CrowdManager.Instance.DeactivateCrowd();
        }
        VignetteManager.Instance.ResetVignette();
        this.enabled = false;

        if (totalQualityScore >= minQualityToWin)
        {
            

            StartCoroutine(ReviveSequenceRoutine());
        }
        else
        {
            string motivo = $"Calidad de RCP muy baja ({totalQualityScore:F0}%). La víctima no sobrevivió.";
            GameManager.Instance.TriggerDerrota(motivo);
        }
    }
    private IEnumerator ReviveSequenceRoutine()
    {
        Debug.Log("RCP Exitoso. Iniciando secuencia de recuperación...");

        GameManager.Instance.PausarReloj();

        if (currentVictim != null)
        {
            currentVictim.OnRevived();
        }

        string contexto = "CONTEXTO: ¡Está reaccionando! Está expulsando agua.";
        string instruccion = "Observa a la víctima.";
        UIManager.Instance.ShowWristContext(contexto);
        UIManager.Instance.ShowWristInstruction_Instant(instruccion);

        yield return new WaitForSeconds(5.0f);

        Debug.Log("Secuencia terminada. Pasando al Quiz.");

        string instruccionFinal = "¡Buen trabajo! Has salvado una vida. Ahora completa este cuestionario.";
        string contextoFinal = "CONTEXTO: El paciente está estable.";

        UIManager.Instance.ShowWristContext(contextoFinal);
        UIManager.Instance.ShowWristInstruction(instruccionFinal);
        UIManager.Instance.CompleteChecklistStep(3); 

        if (quizPanelGameObject != null)
            quizPanelGameObject.SetActive(true);

        if (quizManager != null)
            quizManager.StartQuiz();
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