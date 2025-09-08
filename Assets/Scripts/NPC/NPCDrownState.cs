// Guardar como "NPCDrownState.cs"
using UnityEngine;

public class NPCDrownState : NPCBaseState
{
    public override void EnterState(NPCStateController npc)
    {
        Debug.LogWarning("¡NPC se está AHOGANDO!");
        // El NPC se detiene y muestra el indicador de ayuda
        npc.agent.isStopped = true;
        if (npc.drowningIndicator != null)
        {
            npc.drowningIndicator.SetActive(true);
        }
    }

    public override void UpdateState(NPCStateController npc)
    {
        // En este estado, el NPC no hace nada por sí mismo. Espera a ser rescatado.
    }

    public override void ExitState(NPCStateController npc)
    {
        // Cuando lo rescaten, ocultamos el indicador
        if (npc.drowningIndicator != null)
        {
            npc.drowningIndicator.SetActive(false);
        }
    }
}