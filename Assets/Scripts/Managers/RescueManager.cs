using UnityEngine;
public enum BodyPart
{
    Hombros,
    Cabeza,
    Pecho,
    Barriga,
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
                break;

        }
    }
    public void BodyPartInteracted(BodyPart part)
    {
        // Solo reaccionamos si estamos en el paso de "comprobar si responde".
        if (currentState != RescueState.VictimRescued) return;

        // Si el jugador tocó la parte correcta...
        if (part == BodyPart.Hombros)
        {
            Debug.Log("¡ACCIÓN CORRECTA! El jugador tocó los hombros.");
            // Aquí podrías activar una animación y reproducir el sonido "Hey_RCP"
           // AudioManager.Instance.PlaySound(AudioManager.Instance.heyRcpSound); // Necesitarás añadir esta referencia en AudioManager

            // Avanzamos al siguiente estado del rescate.
            TransitionToState(RescueState.ConsciousnessCheck);
        }
        else // Si el jugador tocó cualquier otra parte...
        {
            Debug.Log($"ACCIÓN INCORRECTA: El jugador tocó {part}.");
            // Damos feedback negativo.
            // AudioManager.Instance.PlayInstructorVoice("No, debes tocar los hombros.");
            Debug.Log("Instructor (Feedback): ¡No! Toca sus hombros para ver si responde.");
        }
    }
    public void PerformConsciousnessCheck()
    {
        if (currentState == RescueState.VictimRescued)
        {
            Debug.Log("Jugador realiza la comprobación de conciencia.");
            // Aquí podrías activar una animación y reproducir un sonido
            // AudioManager.Instance.PlaySound("Hey_RCP");

            TransitionToState(RescueState.ConsciousnessCheck);
        }
    }
}