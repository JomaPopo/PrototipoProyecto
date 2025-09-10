using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class NPCStateController : MonoBehaviour
{
    // ¡NUEVO! Un enum para seleccionar el estado inicial desde el Inspector.
    public enum StartingState { WalkingOnBeach, SwimmingInSea, Drowning }

    [Header("Control de Estado para Pruebas")]
    public StartingState startInState = StartingState.WalkingOnBeach;

    [Header("Parámetros de Comportamiento")]
    public float minWalkTime = 10f;
    public float maxWalkTime = 30f;
    public float minSwimTime = 15f;
    public float maxSwimTime = 40f;
    [Range(0, 1)]
    public float drowningChance = 0.25f;

    [Header("Referencias")]
    public GameObject drowningIndicator;
    [HideInInspector] public NavMeshAgent agent;

    // --- Sistema de Estados ---
    private NPCBaseState currentState;
    public NPCBaseState CurrentState => currentState;

    public readonly NPCWalkState WalkState = new NPCWalkState();
    public readonly NPCSwimState SwimState = new NPCSwimState();
    public readonly NPCDrownState DrownState = new NPCDrownState();

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Start()
    {
        if (drowningIndicator != null)
            drowningIndicator.SetActive(false);

        // ¡NUEVA LÓGICA! Leemos la selección del Inspector para decidir el estado inicial.
        switch (startInState)
        {
            case StartingState.WalkingOnBeach:
                TransitionToState(WalkState);
                break;
            case StartingState.SwimmingInSea:
                TransitionToState(SwimState);
                break;
            case StartingState.Drowning:
                TransitionToState(DrownState);
                break;
        }
    }

    void Update()
    {
        if (currentState != null)
        {
            currentState.UpdateState(this);
        }
    }

    public void TransitionToState(NPCBaseState nextState)
    {
        if (currentState != null)
        {
            currentState.ExitState(this);
        }
        currentState = nextState;
        currentState.EnterState(this);
    }

    // --- Funciones de Navegación (sin cambios) ---
    public Vector3 GetRandomPointOnBeach()
    {
        return GetRandomPointOnNavMesh(0);
    }

    public Vector3 GetRandomPointInSea()
    {
        return GetRandomPointOnNavMesh(3);
    }

    private Vector3 GetRandomPointOnNavMesh(int areaIndex)
    {
        Vector3 randomDirection = Random.insideUnitSphere * 100f;
        randomDirection += transform.position;
        NavMeshHit hit;
        NavMesh.SamplePosition(randomDirection, out hit, 100f, 1 << areaIndex);
        return hit.position;
    }
}