// Guardar como "NPCSwimState.cs"
using UnityEngine;

public class NPCSwimState : NPCBaseState
{
    private float swimTimer;

    public override void EnterState(NPCStateController npc)
    {
        Debug.Log("NPC entrando en estado: Nadar");
        // Ponemos al NPC a "nadar" a un punto aleatorio en el mar
        npc.agent.isStopped = false;
        npc.agent.SetDestination(npc.GetRandomPointInSea());
        swimTimer = Random.Range(npc.minSwimTime, npc.maxSwimTime);
    }

    public override void UpdateState(NPCStateController npc)
    {
        swimTimer -= Time.deltaTime;

        if (swimTimer <= 0 || !npc.agent.pathPending && npc.agent.remainingDistance < 0.5f)
        {
            // Tiramos un "dado" para ver si se ahoga o si vuelve a la playa
            if (Random.value < npc.drowningChance)
            {
                npc.TransitionToState(npc.DrownState);
            }
            else
            {
                npc.TransitionToState(npc.WalkState);
            }
        }
    }

    public override void ExitState(NPCStateController npc)
    {
        // Limpieza
    }
}