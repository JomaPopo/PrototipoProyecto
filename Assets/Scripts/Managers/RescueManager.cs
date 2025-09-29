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
        Debug.Log("�Secuencia de rescate iniciada!");
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
                Debug.Log("Instructor: �Buen trabajo! Ahora comprueba si responde.");
                break;

            case RescueState.ConsciousnessCheck:
                Debug.Log("Instructor: Sigue inconsciente. �Usa tu radio y pide ayuda!");
                break;

        }
    }
    public void BodyPartInteracted(BodyPart part)
    {
        // Solo reaccionamos si estamos en el paso de "comprobar si responde".
        if (currentState != RescueState.VictimRescued) return;

        // Si el jugador toc� la parte correcta...
        if (part == BodyPart.Hombros)
        {
            Debug.Log("�ACCI�N CORRECTA! El jugador toc� los hombros.");
            // Aqu� podr�as activar una animaci�n y reproducir el sonido "Hey_RCP"
           // AudioManager.Instance.PlaySound(AudioManager.Instance.heyRcpSound); // Necesitar�s a�adir esta referencia en AudioManager

            // Avanzamos al siguiente estado del rescate.
            TransitionToState(RescueState.ConsciousnessCheck);
        }
        else // Si el jugador toc� cualquier otra parte...
        {
            Debug.Log($"ACCI�N INCORRECTA: El jugador toc� {part}.");
            // Damos feedback negativo.
            // AudioManager.Instance.PlayInstructorVoice("No, debes tocar los hombros.");
            Debug.Log("Instructor (Feedback): �No! Toca sus hombros para ver si responde.");
        }
    }
    public void PerformConsciousnessCheck()
    {
        if (currentState == RescueState.VictimRescued)
        {
            Debug.Log("Jugador realiza la comprobaci�n de conciencia.");
            // Aqu� podr�as activar una animaci�n y reproducir un sonido
            // AudioManager.Instance.PlaySound("Hey_RCP");

            TransitionToState(RescueState.ConsciousnessCheck);
        }
    }
}