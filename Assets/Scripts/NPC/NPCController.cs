using UnityEngine;

public class NPCController : MonoBehaviour
{
    public enum State { Swimming, Drowning , Rescued }

    [Header("Estado Actual")]
    private State currentState;
    public State CurrentState => currentState;


    [Header("Parámetros de Movimiento")]
    public float swimmingSpeed = 3.5f;
    public float rotationSpeed = 5f;

    [Header("Parámetros de Comportamiento")]
    public float minSwimTime = 15f;
    public float maxSwimTime = 40f;
    private float swimTimer;

    [Header("Referencias")]
    public GameObject drowningIndicator;
    public Transform[] swimWaypoints;
    public GameObject interactionCanvas,panelViaRespiratoria;
    private Transform currentWaypoint;

    private Rigidbody rb;

    void Awake()
    {
        if (drowningIndicator != null)
            drowningIndicator.SetActive(false);
        if (interactionCanvas != null)
            interactionCanvas.SetActive(false);
        rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        EnterSwimmingState();
    }

    void Update()
    {
        if (currentState == State.Swimming)
        {
            swimTimer -= Time.deltaTime;

            if (swimTimer <= 0)
            {
                EnterDrowningState();
            }
            else if (currentWaypoint != null && Vector3.Distance(transform.position, currentWaypoint.position) < 1.5f)
            {
                SetNewRandomWaypoint();
            }
        }
    }

    void FixedUpdate()
    {
        if (currentState == State.Swimming && currentWaypoint != null)
        {
            MoveAndRotateTowardsWaypoint();
        }
    }
    public void OnRescued()
    {
        EnterRescuedState();
    }

    void EnterSwimmingState()
    {
        Debug.Log("Entrando al estado: Nadando");
        currentState = State.Swimming;

        swimTimer = Random.Range(minSwimTime, maxSwimTime);
        SetNewRandomWaypoint();
    }

    void EnterDrowningState()
    {
        Debug.LogWarning("Entrando al estado: Ahogándose");
        AudioManager.Instance.PlayVoice(AudioManager.Instance.instructor_AlertaEmergencia);
        UIManager.Instance.ShowInstruction("Busca a la persona ahogada");

        currentState = State.Drowning;

        if (drowningIndicator != null)
            drowningIndicator.SetActive(true);

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }
    private void EnterRescuedState()
    {
        currentState = State.Rescued;
        if (drowningIndicator != null)
            drowningIndicator.SetActive(false);
    }
    void SetNewRandomWaypoint()
    {
        if (swimWaypoints.Length > 0)
        {
            int randomIndex = Random.Range(0, swimWaypoints.Length);
            currentWaypoint = swimWaypoints[randomIndex];
            Debug.Log($"Nuevo destino: {currentWaypoint.name}");
        }
        else
        {
            Debug.LogError("No se han asignado Waypoints de nado en el Inspector.");
            EnterDrowningState();
        }
    }

    void MoveAndRotateTowardsWaypoint()
    {
        Vector3 direction = (currentWaypoint.position - transform.position).normalized;
        direction.y = 0;

        Vector3 newPosition = transform.position + direction * swimmingSpeed * Time.fixedDeltaTime;
        rb.MovePosition(newPosition);

        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.fixedDeltaTime * rotationSpeed);
        }
    }
}