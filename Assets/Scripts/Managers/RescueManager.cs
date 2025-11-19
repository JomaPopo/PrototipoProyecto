using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem.Haptics;
using System;
using Mono.Cecil.Cil;
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


        nextStateAfterPause = RescueState.AwaitingRescue;
    }
    public void TriggerInitialAlert_VR(string alertMessage, float emergencyTime)
    {
        GameManager.Instance.IniciarReloj();

        VignetteManager.Instance.TriggerEmergencyFlash(5.0f, Color.red, 0.6f);

        

        string tiempoFormateado = "04:00";
        UIManager.Instance.ShowWristAlert(alertMessage, tiempoFormateado);

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

       
        TransitionToState(RescueState.VictimRescued);
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
  

        if (currentVictim != null && currentVictim.interactionCanvas != null)
            currentVictim.interactionCanvas.SetActive(false);

        switch (currentState)
        {
            case RescueState.AwaitingRescue:

       
                break;

            case RescueState.VictimRescued:

                UIManager.Instance.ShowChecklistPanel();

                string contexto = "CONTEXTO: La víctima está tumbada en la zona segura. Tienes que hacer algo.";
                string instruccion = "PASO 1: ¡Verificar Conciencia! Toca sus hombros y llámalo en voz alta para ver si responde.";

                UIManager.Instance.ShowWristContext(contexto);
                UIManager.Instance.ShowWristInstruction(instruccion);

                AudioManager.Instance.PlayVoice(AudioManager.Instance.instructor_ComprobarConciencia);
                StartCoroutine(ActivateButtonsAfterDelay(6.0f, BodyPart.Hombros));
                break;

            case RescueState.ConsciousnessCheck:
                UIManager.Instance.ShowPausePanel("Sigue inconsciente. Necesitamos pedir ayuda profesional.");
                nextStateAfterPause = RescueState.CallForHelp; 
                break;

            case RescueState.CallForHelp:
                //PauseManager.Instance.RegainControlFromUI();
                //UIManager.Instance.ShowInstruction("Activa la radio para pedir asistencia médica.");
                if (radioPanel != null) radioPanel.SetActive(true);
                break;

            case RescueState.AirwayCheck:
      
                Debug.Log("¡Transición a AirwayCheck! Activando botones de Frente y Mentón.");
                if (currentVictim != null)
                {
                    currentVictim.ActivateInteraction(BodyPart.Frente, true); 
                    currentVictim.ActivateInteraction(BodyPart.Menton, true); 
                }
                frenteTocada = false;
                mentonTocado = false;

                break;

            case RescueState.PerformCPR:
                
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

        if (radioPanel != null)
            radioPanel.SetActive(false);

        AudioManager.Instance.PlayVoice(AudioManager.Instance.timbreDeLlamada);
        float duracionAudioLlamada = 4f;

        // --- ¡AQUÍ ESTÁ TU LÓGICA DE CONTEXTO! ---
        // 1. Texto de ESPERA (mientras suena el timbre)
        string contextoEspera = "CONTEXTO: La llamada está en curso...";
        string instruccionEspera = "(Esperando respuesta...)";
        UIManager.Instance.ShowWristContext(contextoEspera);
        UIManager.Instance.ShowWristInstruction(instruccionEspera);

        // 2. Marcar Toggle
        UIManager.Instance.CompleteChecklistStep(1); // Marca Paso 2

        // 3. Texto del SIGUIENTE paso (Paso 3: Vías Aéreas)
        string contextoSiguiente = "CONTEXTO: ¡Ayuda en camino! Debes mantenerlo estable.";
        string instruccionSiguiente = "PASO 3: Abre las vías respiratorias. Coloca una mano en la frente y otra en el mentón.";

        // 4. Llamar Corutina Genérica (¡con 2 strings!)
        StartCoroutine(TransitionAfterDelay(
            RescueState.AirwayCheck,
            duracionAudioLlamada,
            contextoSiguiente,      // <-- El contexto que faltaba
            instruccionSiguiente    // <-- La instrucción
        ));
    }

    public void BodyPartInteracted(BodyPart part)
    {
        if (currentState == RescueState.VictimRescued)
        {
            if (part == BodyPart.Hombros)
            {
                if (currentVictim != null)
                    currentVictim.DeactivateInteraction(BodyPart.Hombros);

                AudioManager.Instance.PlayVoice(AudioManager.Instance.playerCheckingConsciousness);
                float duracionAudioFrase = 3.0f;
                string contextoEspera = "CONTEXTO: La víctima no responde. Está inconsciente.";
                string instruccionEspera = "(Debes pedir ayuda...)";
                UIManager.Instance.ShowWristContext(contextoEspera);
                UIManager.Instance.ShowWristInstruction(instruccionEspera);
                UIManager.Instance.CompleteChecklistStep(0); // Marca Toggle 1

                string contextoSiguiente = "CONTEXTO: Necesitas ayuda profesional ¡ya!";
                string instruccionSiguiente = "PASO 2: Activa tu radio para llamar al 106.";

                StartCoroutine(TransitionAfterDelay(
                    RescueState.CallForHelp,
                    duracionAudioFrase,
                    contextoSiguiente,  // Le pasamos el contexto
                    instruccionSiguiente // Le pasamos la instrucción
                ));
            }
            else
            {
                AudioManager.Instance.PlayVoice(AudioManager.Instance.instructor_FeedbackIncorrectoConciencia);
                // (Opcional: UIManager.Instance.ShowWristInstruction("¡Error! Toca los HOMBROS."))
            }
        }
        else if (currentState == RescueState.AirwayCheck)
        {
            if (part == BodyPart.Frente)
            {
                frenteTocada = true;
                currentVictim.DeactivateInteraction(BodyPart.Frente);
            }
            else if (part == BodyPart.Menton)
            {
                mentonTocado = true;
                currentVictim.DeactivateInteraction(BodyPart.Menton);
            }
            else
            {
                // Feedback de error
                UIManager.Instance.ShowWristInstruction("¡ERROR! Concéntrate. Toca FRENTE y MENTÓN.");
                return; // Importante: Salir para que no siga
            }

            // --- ¡COMPROBAMOS SI AMBOS HAN SIDO TOCADOS! ---
            if (frenteTocada && mentonTocado)
            {
                // (Opcional: poner audio de "¡Bien hecho!")
                // AudioManager.Instance.PlayVoice(audioDeExito);
                float duracionAudioOK = 3.0f;

                // --- ¡AQUÍ ESTÁ TU LÓGICA DE CONTEXTO! ---
                // 1. Texto de ESPERA
                string contextoEspera = "CONTEXTO: ¡Bien hecho! Las vías respiratorias están abiertas.";
                string instruccionEspera = "(Preparando para RCP...)";
                UIManager.Instance.ShowWristContext(contextoEspera);
                UIManager.Instance.ShowWristInstruction(instruccionEspera);

                // 2. Marcar Toggle
                UIManager.Instance.CompleteChecklistStep(2); // Marca Paso 3

                // 3. Texto del SIGUIENTE paso (Paso 4: RCP)
                string contextoSiguiente = "CONTEXTO: El corazón no late. Debes bombear sangre manualmente.";
                string instruccionSiguiente = "PASO 4: ¡Inicia RCP! Presiona <b>[Q] + [E]</b> (o <b>Grips de VR</b>) al ritmo.";

                // 4. Llamar Corutina Genérica (¡con 2 strings!)
                StartCoroutine(TransitionAfterDelay(
                    RescueState.PerformCPR,
                    duracionAudioOK,
                    contextoSiguiente,      
                    instruccionSiguiente    
                ));
            }
        }
    }

    private IEnumerator ShowPanelAfterDelay(float delay, string panelMessage, RescueState nextState)
    {
        Debug.Log($"Esperando {delay} segundos antes de mostrar el panel de pausa...");
        yield return new WaitForSeconds(delay);
        UIManager.Instance.ShowPausePanel(panelMessage);
        //PauseManager.Instance.FreeCursorForUI();
        nextStateAfterPause = nextState;
    }

    private IEnumerator TransitionAfterDelay(RescueState nextState, float delay, string nextContext, string nextInstruction)
    {
        Debug.Log($"TIEMPO MUERTO (Audio): Esperando {delay}s...");
        yield return new WaitForSeconds(delay);

        Action onTypingIsDone = () =>
        {
            Debug.Log("¡Tipeo terminado! Transicionando a " + nextState);
            TransitionToState(nextState);
        };

        // ¡Llama a las DOS funciones!
        UIManager.Instance.ShowWristContext(nextContext);
        UIManager.Instance.ShowWristInstruction(nextInstruction, onTypingIsDone);
    }

    private IEnumerator ActivateButtonsAfterDelay(float delay, params BodyPart[] partsToActivate)
    {
        Debug.Log($"TIEMPO MUERTO: Esperando {delay}s para activar {partsToActivate.Length} botón(es).");

        yield return new WaitForSeconds(delay);

        Debug.Log("TIEMPO MUERTO: Activando botones AHORA.");
        if (currentVictim != null)
        {
            foreach (BodyPart part in partsToActivate)
            {
                currentVictim.ActivateInteraction(part, true);
            }
        }
    }
}