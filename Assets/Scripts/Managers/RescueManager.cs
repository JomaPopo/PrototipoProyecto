using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem.Haptics;
public enum BodyPart { Hombros, Cabeza, Pecho, Manos, Pies, Rodilla, Frente, Menton }
public class RescueManager : Singleton<RescueManager>
{
    public enum RescueState { None, AwaitingRescue, VictimRescued, ConsciousnessCheck, CallForHelp, AirwayCheck, PerformCPR }

    [Header("Referencias de UI")]
    [SerializeField] private GameObject radioPanel;
    [SerializeField] private CPRManager cprManager;

    [Header("Estado Actual")]
    public RescueState currentState = RescueState.None;

    private NPCController currentVictim;
    private bool frenteTocada = false;
    private bool mentonTocado = false;

    private RescueState nextStateAfterPause = RescueState.None;

    void Start()
    {
        if (radioPanel != null) radioPanel.SetActive(false);
         AudioManager.Instance.PlayVoice(AudioManager.Instance.instructor_BriefingInicial);
    }
    private void Update() { if (Input.GetKeyDown(KeyCode.Z)) OnRadioCallMade(); }
    public void TriggerInitialAlert(string alertMessage)
    {
        if (currentState != RescueState.None) return;

        Debug.Log("Alerta Inicial Recibida.");
        UIManager.Instance.ShowPausePanel(alertMessage);

        PauseManager.Instance.FreeCursorForUI();

        nextStateAfterPause = RescueState.AwaitingRescue;
    }
    public void TriggerInitialAlert_VR(string alertMessage, float emergencyTime)
    {
        // 1. ¡INICIA EL RELOJ!
        GameManager.Instance.IniciarReloj();

        // 2. ACTIVA LA ALERTA VISUAL (¡USANDO TU VIGNETTEMANAGER!)
        // Llama a la NUEVA función que creamos en tu script
        VignetteManager.Instance.TriggerEmergencyFlash(5.0f, Color.red, 0.6f);

        

        // 4. MUESTRA LA INFO EN LA MUÑECA (SIN PAUSAR)
        // (Asumimos que tu UIManager tiene esta función, como sugerí)
        string tiempoFormateado = "04:00";
        UIManager.Instance.ShowWristAlert(alertMessage, tiempoFormateado);

        // 5. TRANSICIONA AL SIGUIENTE ESTADO INMEDIATAMENTE
        TransitionToState(RescueState.AwaitingRescue);
    }
    public void StartRescueSequence(NPCController victim)
    {
       
        if (currentState == RescueState.VictimRescued || currentState == RescueState.ConsciousnessCheck)
        {
            Debug.LogWarning("StartRescueSequence llamada cuando ya había una secuencia en progreso. Ignorando.");
            return;
        }


        Debug.Log("StartRescueSequence EJECUTÁNDOSE AHORA."); 
        currentVictim = victim;

        UIManager.Instance.ShowPausePanel("¡Has sacado a la víctima! Prepárate para evaluar.");
        PauseManager.Instance.FreeCursorForUI(); 
        nextStateAfterPause = RescueState.VictimRescued; 
    }

    public void OnPausePanelAcknowledged()
    {
        if (nextStateAfterPause == RescueState.None)
        {
            Debug.LogWarning("OnPausePanelAcknowledged fue llamado pero no había un siguiente estado pendiente.");
            return;
        }

        Debug.Log("Jugador presionó 'Ok'. Procediendo al siguiente paso: " + nextStateAfterPause);

        RescueState stateToTransitionTo = nextStateAfterPause;
        nextStateAfterPause = RescueState.None;

        TransitionToState(stateToTransitionTo);
    }

    public void TransitionToState(RescueState newState)
    {
        if (currentState == newState) return;

        currentState = newState;
        Debug.Log($"Nuevo estado de rescate: {newState}");
        UIManager.Instance.HideInstructions();
        // UIManager.HidePausePanel(); // HidePausePanel ya se llama desde el botón Ok

        if (currentVictim != null && currentVictim.interactionCanvas != null)
            currentVictim.interactionCanvas.SetActive(false);

        switch (currentState)
        {
            case RescueState.AwaitingRescue:

                //PauseManager.Instance.RegainControlFromUI();
                //UIManager.Instance.ShowWristObjective("¡Rápido! ¡Rescata a la víctima y llévala a una zona segura!");
                //AudioManager.Instance.PlayVoice(AudioManager.Instance.instructor_AlertaEmergencia); // Audio de "¡Vamos!"
                break;

            case RescueState.VictimRescued:
               
                UIManager.Instance.ShowInstruction("Toca sus hombros para ver si responde");
                AudioManager.Instance.PlayVoice(AudioManager.Instance.instructor_ComprobarConciencia);
                if (currentVictim != null && currentVictim.interactionCanvas != null)
                    currentVictim.interactionCanvas.SetActive(true);
                break;

            case RescueState.ConsciousnessCheck:
                UIManager.Instance.ShowPausePanel("Sigue inconsciente. Necesitamos pedir ayuda profesional.");
                PauseManager.Instance.FreeCursorForUI(); 
                nextStateAfterPause = RescueState.CallForHelp; 
                break;

            case RescueState.CallForHelp:
                PauseManager.Instance.RegainControlFromUI();
                UIManager.Instance.ShowInstruction("Activa la radio para pedir asistencia médica.");
                if (radioPanel != null) radioPanel.SetActive(true);
                break;

            case RescueState.AirwayCheck:
                PauseManager.Instance.FreeCursorForUI();
                UIManager.Instance.ShowInstruction("Toca el menton y la frente");
                AudioManager.Instance.PlayVoice(AudioManager.Instance.instructor_AbrirViasAereas);
                frenteTocada = false; mentonTocado = false;
                if (currentVictim != null && currentVictim.interactionCanvas != null)
                    currentVictim.interactionCanvas.SetActive(true); 
                break;

            case RescueState.PerformCPR:
                PauseManager.Instance.RegainControlFromUI();
                UIManager.Instance.ShowInstruction("¡Inicia el RCP! Sigue el ritmo de la guía y presiona Q y E a la vez.");
                if (cprManager != null)
                {
                    cprManager.enabled = true;
                    cprManager.StartCPR();
                }
                break;
        }
    }


    public void OnRadioCallMade()
    {
        if (currentState != RescueState.CallForHelp) return;

        Debug.Log("OnRadioCallMade: Iniciando secuencia de audio y mostrando panel.");

        if (radioPanel != null) radioPanel.SetActive(false);

        AudioManager.Instance.PlayRadioCallSequence();

        UIManager.Instance.ShowPausePanel("Llamada realizada. La ayuda está en camino. Presiona Ok para continuar.");

        PauseManager.Instance.FreeCursorForUI();

        nextStateAfterPause = RescueState.AirwayCheck;

    }

    public void BodyPartInteracted(BodyPart part)
    {
        if (currentState == RescueState.VictimRescued)
        {
            if (part == BodyPart.Hombros)
            {
                AudioManager.Instance.PlayVoice(AudioManager.Instance.playerCheckingConsciousness);
                TransitionToState(RescueState.ConsciousnessCheck);
            }
            else
            {
                AudioManager.Instance.PlayVoice(AudioManager.Instance.instructor_FeedbackIncorrectoConciencia);
                UIManager.Instance.ShowPausePanel("¡No! Toca sus HOMBros para ver si responde. Presiona Ok para continuar.");
                PauseManager.Instance.FreeCursorForUI();
                nextStateAfterPause = RescueState.VictimRescued; 
            }
        }
        else if (currentState == RescueState.AirwayCheck)
        {
            if (part == BodyPart.Frente) frenteTocada = true;
            else if (part == BodyPart.Menton) mentonTocado = true;
            else UIManager.Instance.ShowInstruction("¡Concéntrate! FRENTE y MENTÓN.");

            if (frenteTocada && mentonTocado)
            {
                UIManager.Instance.ShowPausePanel("¡Bien hecho! Vías respiratorias abiertas. Prepárate para el RCP.");
                PauseManager.Instance.FreeCursorForUI();
                nextStateAfterPause = RescueState.PerformCPR;
            }
        }
    }

    private IEnumerator ShowPanelAfterDelay(float delay, string panelMessage, RescueState nextState)
    {
        Debug.Log($"Esperando {delay} segundos antes de mostrar el panel de pausa...");
        yield return new WaitForSeconds(delay);
        UIManager.Instance.ShowPausePanel(panelMessage);
        PauseManager.Instance.FreeCursorForUI();
        nextStateAfterPause = nextState;
    }
}