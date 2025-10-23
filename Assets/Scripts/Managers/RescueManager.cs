using UnityEngine;
public enum BodyPart
{
    Hombros,
    Cabeza,
    Pecho,
    Manos,
    Pies,
    Rodilla,
    Frente,
    Menton
}
public class RescueManager : Singleton<RescueManager> 
{
    public enum RescueState
    {
        None,
        VictimRescued, 
        ConsciousnessCheck, 
        CallForHelp,
        AirwayCheck,
        BreathingCheck,
        PerformCPR
    }
    [Header("Referencias de UI")]
    [SerializeField] private GameObject radioPanel;
    public MouseLook playerMouseLook;

    [SerializeField] private CPRManager cprManager;

    [Header("Estado Actual")]
    public RescueState currentState;

    private NPCController currentVictim;
    private bool frenteTocada = false;
    private bool mentonTocado = false;

    void Start()
    {
        if (radioPanel != null)
        {
            radioPanel.SetActive(false);
        }
        VignetteManager.Instance.ShowVignette(1.5f, Color.red, 0.9f);

        AudioManager.Instance.PlayVoice(AudioManager.Instance.instructor_BriefingInicial);

    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            OnRadioCallMade();
        }
    }
    public void StartRescueSequence(NPCController victim)
    {
        Debug.Log("�Secuencia de rescate iniciada!");
        UIManager.Instance.ShowInstruction("Este atento�");

        currentVictim = victim;
        if (currentVictim.interactionCanvas != null)
        {
            currentVictim.interactionCanvas.SetActive(true);
        }

        TransitionToState(RescueState.VictimRescued);
    }

    public void TransitionToState(RescueState newState)
    {
        currentState = newState;
        Debug.Log($"Nuevo estado de rescate: {newState}");

        switch (currentState)
        {
            case RescueState.VictimRescued:
                Debug.Log("Instructor: �Buen trabajo! Ahora comprueba si responde.");
                AudioManager.Instance.PlayVoice(AudioManager.Instance.instructor_ComprobarConciencia);
                UIManager.Instance.ShowInstruction("Toca sus hombros para ver si responde");

                break;

            case RescueState.ConsciousnessCheck:
                Debug.Log("Instructor: Sigue inconsciente. �Usa tu radio y pide ayuda!");

                if (currentVictim.interactionCanvas != null)
                {
                    currentVictim.interactionCanvas.SetActive(false);
                }
                UIManager.Instance.ShowInstruction("Usa tu radio para pedir ayuda");

                TransitionToState(RescueState.CallForHelp);

                break;

            case RescueState.CallForHelp:
                if (radioPanel != null)
                {
                    radioPanel.SetActive(true);
                }
                break;


            case RescueState.AirwayCheck:
                Debug.Log("Instructor: �Excelente! La ayuda viene en camino. Ahora, abre sus v�as respiratorias.");
                frenteTocada = false;
                mentonTocado = false;
                if (currentVictim.interactionCanvas != null)
                    currentVictim.interactionCanvas.SetActive(true);

                UIManager.Instance.ShowInstruction("Toca el menton y la frente");


                break;

            case RescueState.BreathingCheck:
                Debug.Log("Instructor: �Bien hecho! V�as respiratorias abiertas. Ahora, comprueba si respira.");
                if (currentVictim.interactionCanvas != null)
                    currentVictim.interactionCanvas.SetActive(false);
                TransitionToState(RescueState.PerformCPR);

                break;
            case RescueState.PerformCPR:
                UIManager.Instance.ShowInstruction("�Inicia el RCP! Sigue el ritmo de la gu�a y presiona Q y E a la vez.");
                if (currentVictim.interactionCanvas != null)
                    currentVictim.interactionCanvas.SetActive(false);
                if (playerMouseLook != null)
                    playerMouseLook.EnableLook();

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
        Debug.Log("El jugador ha realizado la llamada por radio.");
        if (radioPanel != null)
        {
            radioPanel.SetActive(false);
        }

        AudioManager.Instance.PlayRadioCallSequence();
        
        TransitionToState(RescueState.AirwayCheck);
    }
    public void BodyPartInteracted(BodyPart part)
    {
        if (currentState == RescueState.VictimRescued) 
        {
            if (part == BodyPart.Hombros)
            {
                Debug.Log("�ACCI�N CORRECTA! El jugador toc� los hombros.");
                AudioManager.Instance.PlayVoice(AudioManager.Instance.playerCheckingConsciousness);

                TransitionToState(RescueState.ConsciousnessCheck);
            }
            else 
            {
                Debug.Log($"ACCI�N INCORRECTA: El jugador toc� {part}.");
                AudioManager.Instance.PlayVoice(AudioManager.Instance.instructor_FeedbackIncorrectoConciencia);
                Debug.Log("Instructor (Feedback): �No! Toca sus hombros para ver si responde.");
            }
        }
        if (currentState == RescueState.AirwayCheck)
        {
            if (part == BodyPart.Frente)
            {
                frenteTocada = true;
                Debug.Log("Frente tocada.");
                // Aqu� podr�as cambiar el color del bot�n "Frente" para dar feedback
            }
            else if (part == BodyPart.Menton)
            {
                mentonTocado = true;
                Debug.Log("Ment�n tocado.");
                // Aqu� podr�as cambiar el color del bot�n "Ment�n"
            }
            else
            {
                Debug.Log("Instructor (Feedback): �Conc�ntrate! Frente y ment�n.");
            }

            // Si ambas partes han sido tocadas
            if (frenteTocada && mentonTocado)
            {
                Debug.Log("�MANIOBRA COMPLETA! V�as a�reas abiertas.");
               // AudioManager.Instance.PlaySFX(AudioManager.Instance.instructor_FeedbackIncorrectoConciencia);
                TransitionToState(RescueState.BreathingCheck);
            }
        }



    }
    public void PerformConsciousnessCheck()
    {
        if (currentState == RescueState.VictimRescued)
        {
            Debug.Log("Jugador realiza la comprobaci�n de conciencia.");

            TransitionToState(RescueState.ConsciousnessCheck);
        }
    }
}