// Guardar como "NPCWalkState.cs"
using UnityEngine;

public class NPCWalkState : NPCBaseState
{
    private float walkTimer;

    public override void EnterState(NPCStateController npc)
    {
        Debug.Log("NPC entrando en estado: Caminar");
        // Ponemos al NPC a caminar a un punto aleatorio de la playa
        npc.agent.isStopped = false;
        npc.agent.SetDestination(npc.GetRandomPointOnBeach());
        // Reiniciamos el temporizador con un valor aleatorio
        walkTimer = Random.Range(npc.minWalkTime, npc.maxWalkTime);
    }

    public override void UpdateState(NPCStateController npc)
    {
        // Descontamos el tiempo
        walkTimer -= Time.deltaTime;

        // Si el temporizador llega a cero O si el NPC llega a su destino, decidimos qué hacer a continuación
        if (walkTimer <= 0 || !npc.agent.pathPending && npc.agent.remainingDistance < 0.5f)
        {
            npc.TransitionToState(npc.SwimState);
        }
    }

    public override void ExitState(NPCStateController npc)
    {
        // Código de limpieza si fuera necesario al salir del estado de caminar
    }
}