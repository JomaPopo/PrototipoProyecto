using UnityEngine;
using System.Collections;
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
        // 1. Mostramos el panel de pausa con el mensaje de alerta
        UIManager.Instance.ShowPausePanel(alertMessage);

        // 2. Le decimos al PauseManager que detenga al jugador y libere el cursor
        PauseManager.Instance.FreeCursorForUI();

        // 3. Guardamos el estado al que iremos DESPUÉS de que el jugador pulse "Ok"
        nextStateAfterPause = RescueState.AwaitingRescue;
    }

    public void StartRescueSequence(NPCController victim)
    {
       
        if (currentState == RescueState.VictimRescued || currentState == RescueState.ConsciousnessCheck)
        {
            Debug.LogWarning("StartRescueSequence llamada cuando ya había una secuencia en progreso. Ignorando.");
            return;
        }


        Debug.Log("StartRescueSequence EJECUTÁNDOSE AHORA."); // Añade este log para confirmar
        currentVictim = victim;

        // Mostramos el panel de pausa después de dejarlo en la toalla
        UIManager.Instance.ShowPausePanel("¡Has sacado a la víctima! Prepárate para evaluar.");
        PauseManager.Instance.FreeCursorForUI(); // Detenemos al jugador y liberamos cursor
        nextStateAfterPause = RescueState.VictimRescued; // Guardamos el siguiente paso
    }

    public void OnPausePanelAcknowledged()
    {
        if (nextStateAfterPause == RescueState.None)
        {
            Debug.LogWarning("OnPausePanelAcknowledged fue llamado pero no había un siguiente estado pendiente.");
            return;
        }

        Debug.Log("Jugador presionó 'Ok'. Procediendo al siguiente paso: " + nextStateAfterPause);

        // Guardamos el estado al que vamos
        RescueState stateToTransitionTo = nextStateAfterPause;
        // Reseteamos la variable de espera
        nextStateAfterPause = RescueState.None;

        // Transicionamos al estado que estaba pendiente
        TransitionToState(stateToTransitionTo);
    }

    public void TransitionToState(RescueState newState)
    {
        // Evitamos re-entrar al mismo estado si OnPausePanelAcknowledged se llama por error
        if (currentState == newState) return;

        currentState = newState;
        Debug.Log($"Nuevo estado de rescate: {newState}");
        // Limpiamos CUALQUIER instrucción anterior (sea panel o tipeo)
        UIManager.Instance.HideInstructions();
        // UIManager.HidePausePanel(); // HidePausePanel ya se llama desde el botón Ok

        // Aseguramos que los botones del NPC estén ocultos por defecto
        if (currentVictim != null && currentVictim.interactionCanvas != null)
            currentVictim.interactionCanvas.SetActive(false);

        switch (currentState)
        {
            case RescueState.AwaitingRescue:
               
                PauseManager.Instance.RegainControlFromUI();
                UIManager.Instance.ShowInstruction("¡Rápido! ¡Rescata a la víctima y llévala a una zona segura!");
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

        // 2. Mostramos el panel de pausa INMEDIATAMENTE
        UIManager.Instance.ShowPausePanel("Llamada realizada. La ayuda está en camino. Presiona Ok para continuar.");

        // 3. Liberamos el cursor y detenemos al jugador INMEDIATAMENTE
        PauseManager.Instance.FreeCursorForUI();

        // 4. Guardamos el estado al que iremos DESPUÉS de pulsar "Ok"
        nextStateAfterPause = RescueState.AirwayCheck;

        // YA NO usamos la corutina ShowPanelAfterDelay ni la pausa automática aquí.
        // El juego ahora ESPERA a que el jugador presione "Ok".
    }

    public void BodyPartInteracted(BodyPart part)
    {
        if (currentState == RescueState.VictimRescued)
        {
            if (part == BodyPart.Hombros)
            {
                AudioManager.Instance.PlayVoice(AudioManager.Instance.playerCheckingConsciousness);
                // Pasamos DIRECTO al estado intermedio que mostrará el panel
                TransitionToState(RescueState.ConsciousnessCheck);
            }
            else
            {
                AudioManager.Instance.PlayVoice(AudioManager.Instance.instructor_FeedbackIncorrectoConciencia);
                // Podríamos mostrar el panel con el feedback incorrecto aquí también si quisiéramos
                UIManager.Instance.ShowPausePanel("¡No! Toca sus HOMBros para ver si responde. Presiona Ok para continuar.");
                PauseManager.Instance.FreeCursorForUI();
                nextStateAfterPause = RescueState.VictimRescued; // Volvemos al mismo paso
            }
        }
        else if (currentState == RescueState.AirwayCheck)
        {
            if (part == BodyPart.Frente) frenteTocada = true;
            else if (part == BodyPart.Menton) mentonTocado = true;
            else UIManager.Instance.ShowInstruction("¡Concéntrate! FRENTE y MENTÓN."); // Feedback rápido aquí

            if (frenteTocada && mentonTocado)
            {
                // Mostramos el panel de pausa antes de ir al RCP
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