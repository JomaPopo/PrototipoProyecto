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

        AudioManager.Instance.PlaySFX(AudioManager.Instance.instructor_BriefingInicial);
        UIManager.Instance.ShowInstruction("wasa");

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
        Debug.Log("¡Secuencia de rescate iniciada!");
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
                Debug.Log("Instructor: ¡Buen trabajo! Ahora comprueba si responde.");
                AudioManager.Instance.PlaySFX(AudioManager.Instance.instructor_ComprobarConciencia);

                break;

            case RescueState.ConsciousnessCheck:
                Debug.Log("Instructor: Sigue inconsciente. ¡Usa tu radio y pide ayuda!");

                if (currentVictim.interactionCanvas != null)
                {
                    currentVictim.interactionCanvas.SetActive(false);
                }
               
                TransitionToState(RescueState.CallForHelp);

                break;

            case RescueState.CallForHelp:
                if (radioPanel != null)
                {
                    radioPanel.SetActive(true);
                }
                break;


            case RescueState.AirwayCheck:
                Debug.Log("Instructor: ¡Excelente! La ayuda viene en camino. Ahora, abre sus vías respiratorias.");
                frenteTocada = false;
                mentonTocado = false;
                if (currentVictim.interactionCanvas != null)
                    currentVictim.interactionCanvas.SetActive(true);
                
                break;

            case RescueState.BreathingCheck:
                Debug.Log("Instructor: ¡Bien hecho! Vías respiratorias abiertas. Ahora, comprueba si respira.");
                if (currentVictim.interactionCanvas != null)
                    currentVictim.interactionCanvas.SetActive(false); 
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
                Debug.Log("¡ACCIÓN CORRECTA! El jugador tocó los hombros.");
                AudioManager.Instance.PlaySFX(AudioManager.Instance.playerCheckingConsciousness);

                TransitionToState(RescueState.ConsciousnessCheck);
            }
            else 
            {
                Debug.Log($"ACCIÓN INCORRECTA: El jugador tocó {part}.");
                AudioManager.Instance.PlaySFX(AudioManager.Instance.instructor_FeedbackIncorrectoConciencia);
                Debug.Log("Instructor (Feedback): ¡No! Toca sus hombros para ver si responde.");
            }
        }
        if (currentState == RescueState.AirwayCheck)
        {
            if (part == BodyPart.Frente)
            {
                frenteTocada = true;
                Debug.Log("Frente tocada.");
                // Aquí podrías cambiar el color del botón "Frente" para dar feedback
            }
            else if (part == BodyPart.Menton)
            {
                mentonTocado = true;
                Debug.Log("Mentón tocado.");
                // Aquí podrías cambiar el color del botón "Mentón"
            }
            else
            {
                Debug.Log("Instructor (Feedback): ¡Concéntrate! Frente y mentón.");
            }

            // Si ambas partes han sido tocadas
            if (frenteTocada && mentonTocado)
            {
                Debug.Log("¡MANIOBRA COMPLETA! Vías aéreas abiertas.");
                AudioManager.Instance.PlaySFX(AudioManager.Instance.instructor_FeedbackIncorrectoConciencia);
                TransitionToState(RescueState.BreathingCheck);
            }
        }



    }
    public void PerformConsciousnessCheck()
    {
        if (currentState == RescueState.VictimRescued)
        {
            Debug.Log("Jugador realiza la comprobación de conciencia.");

            TransitionToState(RescueState.ConsciousnessCheck);
        }
    }
}