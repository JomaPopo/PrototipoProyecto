// Guardar como "NPCBaseState.cs"
using UnityEngine;

// "abstract" significa que esta clase es un molde, no se puede usar directamente.
public abstract class NPCBaseState
{
    // Cada estado podrá entrar, actualizarse y salir.
    public abstract void EnterState(NPCStateController npc);
    public abstract void UpdateState(NPCStateController npc);
    public abstract void ExitState(NPCStateController npc);
}