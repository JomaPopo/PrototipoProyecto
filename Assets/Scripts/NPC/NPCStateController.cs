using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class NPCStateController : MonoBehaviour
{
    // --- Variables Públicas (igual que antes) ---
    [Header("Parámetros de Comportamiento")]
    public float minWalkTime = 10f;
    public float maxWalkTime = 30f;
    public float minSwimTime = 15f;
    public float maxSwimTime = 40f;
    [Range(0, 1)]
    public float drowningChance = 0.25f;

    [Header("Referencias")]
    public GameObject drowningIndicator;
    [HideInInspector] public NavMeshAgent agent; // Lo hacemos público pero oculto en inspector

    // --- Sistema de Estados ---
    [SerializeField]private NPCBaseState currentState;

    // Creamos una instancia de cada estado que el NPC puede tener
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

        // El estado inicial es caminar
        TransitionToState(WalkState);
    }

    void Update()
    {
        // El controlador ya no piensa, solo le dice al estado actual que trabaje.
        if (currentState != null)
        {
            currentState.UpdateState(this);
        }
    }

    // La función que usan los estados para cambiar al siguiente
    public void TransitionToState(NPCBaseState nextState)
    {
        if (currentState != null)
        {
            currentState.ExitState(this);
        }
        currentState = nextState;
        currentState.EnterState(this);
    }

    // --- Funciones para encontrar puntos (igual que antes) ---
    public Vector3 GetRandomPointOnBeach()
    {
        return GetRandomPointOnNavMesh(0); // Arena
    }

    public Vector3 GetRandomPointInSea()
    {
        return GetRandomPointOnNavMesh(3); // Agua (¡Asegúrate de que este número coincida con tu NavMesh Area!)
    }

    private Vector3 GetRandomPointOnNavMesh(int areaIndex)
    {
        Vector3 randomDirection = Random.insideUnitSphere * 100f; // Aumentamos el radio de búsqueda
        randomDirection += transform.position;
        NavMeshHit hit;
        NavMesh.SamplePosition(randomDirection, out hit, 100f, 1 << areaIndex);
        return hit.position;
    }
}