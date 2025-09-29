using UnityEngine;
public enum BodyPart
{
    Hombros,
    Cabeza,
    Pecho,
    Manos,
    Pies,
    Rodilla
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

    public RescueState currentState;
    private NPCController currentVictim;

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
                break;

            case RescueState.ConsciousnessCheck:
                Debug.Log("Instructor: Sigue inconsciente. ¡Usa tu radio y pide ayuda!");
                if (currentVictim.interactionCanvas != null)
                {
                    currentVictim.interactionCanvas.SetActive(false);
                }
                break;

        }
    }
    public void BodyPartInteracted(BodyPart part)
    {
        if (currentState != RescueState.VictimRescued) return;

        if (part == BodyPart.Hombros)
        {
            Debug.Log("¡ACCIÓN CORRECTA! El jugador tocó los hombros.");
           // AudioManager.Instance.PlaySound(AudioManager.Instance.heyRcpSound);

            TransitionToState(RescueState.ConsciousnessCheck);
        }
        else // Si el jugador tocó cualquier otra parte...
        {
            Debug.Log($"ACCIÓN INCORRECTA: El jugador tocó {part}.");
            // AudioManager.Instance.PlayInstructorVoice("No, debes tocar los hombros.");
            Debug.Log("Instructor (Feedback): ¡No! Toca sus hombros para ver si responde.");
        }
    }
    public void PerformConsciousnessCheck()
    {
        if (currentState == RescueState.VictimRescued)
        {
            Debug.Log("Jugador realiza la comprobación de conciencia.");
            // AudioManager.Instance.PlaySound("Hey_RCP");

            TransitionToState(RescueState.ConsciousnessCheck);
        }
    }
}