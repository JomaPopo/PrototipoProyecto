using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem.Haptics;
using System;
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
        GameManager.Instance.IniciarReloj();

        VignetteManager.Instance.TriggerEmergencyFlash(5.0f, Color.red, 0.6f);

        

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

        //UIManager.Instance.ShowPausePanel("¡Has sacado a la víctima! Prepárate para evaluar.");
        //PauseManager.Instance.FreeCursorForUI(); 
        //nextStateAfterPause = RescueState.VictimRescued;
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
        // UIManager.Instance.HideInstructions();
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


                string instruccion = "PASO 1: ¡Verificar Conciencia! Toca sus hombros y llámalo en voz alta para ver si responde.";

                string checklist = "<b>[ ] 1. Tocar Hombros</b>\n" +
                                   "  [ ] 2. Pedir Ayuda (106)\n" +
                                   "  [ ] 3. Abrir Vía Aérea\n" +
                                   "  [ ] 4. Iniciar RCP";

                UIManager.Instance.ShowChecklistStep(instruccion, checklist);
                AudioManager.Instance.PlayVoice(AudioManager.Instance.instructor_ComprobarConciencia);

                StartCoroutine(ActivateButtonsAfterDelay(6.0f, BodyPart.Hombros));
                break;

            case RescueState.ConsciousnessCheck:
                UIManager.Instance.ShowPausePanel("Sigue inconsciente. Necesitamos pedir ayuda profesional.");
                PauseManager.Instance.FreeCursorForUI(); 
                nextStateAfterPause = RescueState.CallForHelp; 
                break;

            case RescueState.CallForHelp:
                //PauseManager.Instance.RegainControlFromUI();
                //UIManager.Instance.ShowInstruction("Activa la radio para pedir asistencia médica.");
                if (radioPanel != null) radioPanel.SetActive(true);
                break;

            case RescueState.AirwayCheck:
                // 1. ¡BORRA CUALQUIER CÓDIGO ANTIGUO DE PC!
                // (Como PauseManager.Instance.FreeCursorForUI(), 
                // UIManager.Instance.ShowInstruction(), etc.)

                // 2. AÑADE ESTAS LÍNEAS (ESTO ES LO QUE ACTIVA TUS BOTONES):
                Debug.Log("¡Transición a AirwayCheck! Activando botones de Frente y Mentón.");
                if (currentVictim != null)
                {
                    currentVictim.ActivateInteraction(BodyPart.Frente, true); // true = brillar
                    currentVictim.ActivateInteraction(BodyPart.Menton, true); // true = brillar
                }

                // 3. Resetea las variables (¡esto ya lo tenías y es correcto!)
                frenteTocada = false;
                mentonTocado = false;

                break;

            case RescueState.PerformCPR:
               // PauseManager.Instance.RegainControlFromUI();
                //UIManager.Instance.ShowInstruction("¡Inicia el RCP! Sigue el ritmo de la guía y presiona Q y E a la vez.");
                if (cprManager != null)
                {
                    cprManager.enabled = true;
                    cprManager.StartCPR();
                }
                break;
        }
    }


    // En RescueManager.cs
    public void OnRadioCallMade()
    {
        if (currentState != RescueState.CallForHelp) return;

        // 1. Ocultamos el botón de la radio
        if (radioPanel != null)
            radioPanel.SetActive(false);

        // 2. Reproducimos el audio
        // AudioManager.Instance.PlayRadioCallSequence();
        AudioManager.Instance.PlayVoice(AudioManager.Instance.timbreDeLlamada);
        // 3. Define la duración del audio (tu "tiempo muerto")
        float duracionAudioLlamada = 4f; // ¡Ajusta este número!

        // 4. Muestra el texto de ESPERA (para jugadores sordos)
        string instruccionEspera = "Llamada al 106 en curso...";
        string checklistActual = "[✔] 1. Tocar Hombros\n" +
                                 "<b>[✔] 2. Pedir Ayuda (106)</b>\n" +
                                 "  [ ] 3. Abrir Vía Aérea\n" +
                                 "  [ ] 4. Iniciar RCP";
        UIManager.Instance.ShowChecklistStep(instruccionEspera, checklistActual);

        // 5. Prepara los textos para el SIGUIENTE paso (después del delay)
        string instruccionSiguiente = "PASO 3: ¡Ayuda en camino! Ahora abre las vías respiratorias. Coloca una mano en la frente y otra en el mentón.";
        string checklistSiguiente = "[✔] 1. Tocar Hombros\n" +
                                    "[✔] 2. Pedir Ayuda (106)\n" +
                                    "<b>[ ] 3. Abrir Vía Aérea</b>\n" +
                                    "  [ ] 4. Iniciar RCP";

        // 6. ¡LLAMA A LA CORUTINA GENÉRICA!
        StartCoroutine(TransitionAfterDelay(
            RescueState.AirwayCheck,  // A dónde ir
            duracionAudioLlamada,     // Cuánto esperar
            instruccionSiguiente,     // Qué instrucción mostrar DESPUÉS
            checklistSiguiente        // Qué checklist mostrar DESPUÉS
        ));
    }

    public void BodyPartInteracted(BodyPart part)
    {
        if (currentState == RescueState.VictimRescued)
        {
            // --- ¡EL JUGADOR ACERTÓ! ---
            if (part == BodyPart.Hombros)
            {
                // 1. Apagamos el brillo de los hombros INMEDIATAMENTE
                if (currentVictim != null)
                    currentVictim.DeactivateInteraction(BodyPart.Hombros);

                // 2. Reproducimos el audio de "no responde"
                AudioManager.Instance.PlayVoice(AudioManager.Instance.playerCheckingConsciousness);

                // 3. Definimos tu "tiempo muerto" de 3 segundos
                float duracionAudioFrase = 3.0f;

                // 4. Mostramos el feedback INMEDIATO en la muñeca
                // (Le decimos que el paso 1 está completo y que espere)
                string instruccionEspera = "La víctima no responde...";
                string checklistActual = "<b>[✔] 1. Tocar Hombros</b>\n" + // ¡Marcamos el check!
                                         "  [ ] 2. Pedir Ayuda (106)\n" +
                                         "  [ ] 3. Abrir Vía Aérea\n" +
                                         "  [ ] 4. Iniciar RCP";
                UIManager.Instance.ShowChecklistStep(instruccionEspera, checklistActual);

                // 5. Preparamos los textos para el SIGUIENTE paso (después del delay)
                string instruccionSiguiente = "PASO 2: ¡No responde! Pide ayuda profesional. Activa tu radio para llamar al 106.";
                string checklistSiguiente = "[✔] 1. Tocar Hombros\n" +
                                            "<b>[ ] 2. Pedir Ayuda (106)</b>\n" + // Resaltamos el paso 2
                                            "  [ ] 3. Abrir Vía Aérea\n" +
                                            "  [ ] 4. Iniciar RCP";

                // 6. ¡LLAMAMOS A NUESTRA CORUTINA GENÉRICA!
                // Le decimos que espere 3 segundos antes de pasar al
                // estado CallForHelp y mostrar los textos del siguiente paso.
                StartCoroutine(TransitionAfterDelay(
                    RescueState.CallForHelp,    // A dónde ir
                    duracionAudioFrase,         // Cuánto esperar (3 seg)
                    instruccionSiguiente,       // Qué instrucción mostrar DESPUÉS
                    checklistSiguiente          // Qué checklist mostrar DESPUÉS
                ));
            }
            // --- ¡EL JUGADOR SE EQUIVOCÓ! ---
            else
            {
                // (Tu lógica de error va aquí)
                AudioManager.Instance.PlayVoice(AudioManager.Instance.instructor_FeedbackIncorrectoConciencia);
                // UIManager.Instance.ShowWristFeedback("¡Error! Sigue el checklist. Toca los HOMBROS.");
            }
        }
        else if (currentState == RescueState.AirwayCheck)
        {
            if (part == BodyPart.Frente) frenteTocada = true;
            else if (part == BodyPart.Menton) mentonTocado = true;

            if (frenteTocada && mentonTocado)
            {

                currentVictim.DeactivateInteraction(BodyPart.Frente);
                currentVictim.DeactivateInteraction(BodyPart.Menton);

                float duracionAudioOK = 3.0f; // ¡El tiempo que dure tu audio!

                string instruccionEspera = "¡Bien hecho! Vías respiratorias abiertas. Prepárate para el RCP.";
                string checklistActual = "[✔] 1. Tocar Hombros\n" +
                                         "[✔] 2. Pedir Ayuda (106)\n" +
                                         "<b>[✔] 3. Abrir Vía Aérea</b>\n" +
                                         "  [ ] 4. Iniciar RCP";
                UIManager.Instance.ShowChecklistStep(instruccionEspera, checklistActual);

                string instruccionSiguiente = "PASO 4: ¡Inicia RCP! Presiona <b>[Q] + [E]</b> (o los <b>Grips de VR</b>) para bombear al ritmo.";
                string checklistSiguiente = "[✔] 1. Tocar Hombros\n" +
                                            "[✔] 2. Pedir Ayuda (106)\n" +
                                            "[✔] 3. Abrir Vía Aérea\n" +
                                            "<b>[ ] 4. Iniciar RCP</b>";

                StartCoroutine(TransitionAfterDelay(
                    RescueState.PerformCPR,   // A dónde ir
                    duracionAudioOK,          // Cuánto esperar
                    instruccionSiguiente,     // Qué instrucción mostrar DESPUÉS
                    checklistSiguiente        // Qué checklist mostrar DESPUÉS
                ));
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
    
    private IEnumerator TransitionAfterDelay(RescueState nextState, float delay, string nextInstruction, string nextChecklist)
    {
        Debug.Log($"TIEMPO MUERTO (Audio): Esperando {delay}s...");
        yield return new WaitForSeconds(delay);

        Action onTypingIsDone = () =>
        {
            Debug.Log("¡Tipeo terminado! Transicionando a " + nextState);
            TransitionToState(nextState);
        };

        UIManager.Instance.ShowChecklistStep(nextInstruction, nextChecklist, onTypingIsDone);

        
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