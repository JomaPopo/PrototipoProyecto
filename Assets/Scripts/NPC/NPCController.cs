using UnityEngine;

public class NPCController : MonoBehaviour
{
    public enum State { Swimming, Drowning, Rescued }

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
    public GameObject interactionCanvas, panelViaRespiratoria;
    private Transform currentWaypoint;

    private Rigidbody rb;
    public Animator animator; // Variable para el Animator
    public GameObject cuerpoNPC;

    [Header("Material a Cambiar")]
    public Material materialNuevo;
    public SkinnedMeshRenderer meshRenderer;
    private Material materialOriginal;
    void Awake()
    {
        if (drowningIndicator != null)
            drowningIndicator.SetActive(false);
        if (interactionCanvas != null)
            interactionCanvas.SetActive(false);

        rb = GetComponent<Rigidbody>();

        // Si no lo asignaste a mano, búscalo en los hijos
        if (meshRenderer == null)
        {
            meshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        }

        if (meshRenderer == null)
        {
            Debug.LogError("¡No se encontró el SkinnedMeshRenderer!");
            return;
        }

        // Guardamos el material original (Element 1) para poder volver a él
        if (meshRenderer.materials.Length > 1)
        {
            materialOriginal = meshRenderer.materials[1];
        }
        // animator = GetComponentInChildren<Animator>();
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

        // --- CAMBIO A BOOLEANOS ---
        if (animator != null)
        {
            animator.SetBool("Nado", true);
            animator.SetBool("Agarrado", false);
            animator.SetBool("Echado", false);
        }

        swimTimer = Random.Range(minSwimTime, maxSwimTime);
        SetNewRandomWaypoint();
    }

    void EnterDrowningState()
    {
        Debug.LogWarning("Entrando al estado: Ahogándose");
        currentState = State.Drowning;

        if (animator != null)
        {
            animator.SetBool("Nado", false);
            animator.SetBool("Agarrado", true);
            animator.SetBool("Echado", false);
            CambiarAlMaterialNuevo();
        }

        AudioManager.Instance.PlayVoice(AudioManager.Instance.instructor_AlertaEmergencia);
        UIManager.Instance.ShowInstruction("Busca a la persona ahogada");
        if (drowningIndicator != null)
            drowningIndicator.SetActive(true);
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        RescueManager.Instance.TriggerInitialAlert("Bueno una persona esta en problemas tienes que encontrarla y rescatarla en menos de 4 minutos");
        //PauseManager.Instance.RegainControlFromUI();
    }

    private void EnterRescuedState()
    {
        currentState = State.Rescued;

        // --- Lógica de Animación (Boleanos) ---
        if (animator != null)
        {
            animator.SetBool("Nado", false);
            animator.SetBool("Agarrado", false);
            animator.SetBool("Echado", true);
        }

        if (drowningIndicator != null)
            drowningIndicator.SetActive(false);

        // --- LÍNEA AÑADIDA PARA GIRAR 180° ---
        // Esto ajusta la rotación en Y a 180, pero mantiene la rotación en X y Z
        // que la animación "Echado" pueda estar usando.
       // cuerpoNPC.transform.eulerAngles = new Vector3(-88.595f, -257.637f, 80.198f);
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
    public void CambiarAlMaterialNuevo()
    {
        if (materialNuevo == null)
        {
            Debug.LogError("No hay un 'Material Nuevo' asignado en el Inspector.");
            return;
        }

        Material[] materialesActuales = meshRenderer.materials;

        materialesActuales[1] = materialNuevo;

        meshRenderer.materials = materialesActuales;
    }
    public void CambiarAlMaterialOriginal()
    {
        Material[] materialesActuales = meshRenderer.materials;
        materialesActuales[1] = materialOriginal;
        meshRenderer.materials = materialesActuales;
    }
}