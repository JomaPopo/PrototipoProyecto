using UnityEngine;
using UnityEngine.UI;  // Para el componente 'Image'
using DG.Tweening;     // Para las animaciones de DOTween
using TMPro;           // Para TextMeshPro
using System.Collections;
// (No necesitamos 'List' o 'Linq' para esta versión)

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

    // --- ¡ESTA ES LA LÓGICA ANTIGUA QUE QUERÍAS! ---
    [Header("Botones de Interacción")]
    public GameObject hombrosButton;
    public GameObject frenteButton;
    public GameObject mentonButton;
    // (Añade aquí tus otros botones como 'pieButton' si los necesitas)

    // Variables privadas para guardar los "tweens" (animaciones)
    private Tween hombrosTween;
    private Tween frenteTween;
    private Tween mentonTween;

    // Variables para guardar las Imágenes (para el fade)
    private Image hombrosImage;
    private Image frenteImage;
    private Image mentonImage;

    // (Quitamos el 'playerCameraTransform' porque esta versión no lo usa)

    void Awake()
    {
        if (drowningIndicator != null)
            drowningIndicator.SetActive(false);
        if (interactionCanvas != null)
            interactionCanvas.SetActive(false);

        rb = GetComponent<Rigidbody>();

        if (meshRenderer == null)
            meshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();

        if (meshRenderer == null)
        {
            Debug.LogError("¡No se encontró el SkinnedMeshRenderer!");
            return;
        }

        if (meshRenderer.materials.Length > 1)
            materialOriginal = meshRenderer.materials[1];

        // --- ¡LÓGICA DE AWAKE ANTIGUA! ---
        if (hombrosButton != null) hombrosImage = hombrosButton.GetComponentInChildren<Image>();
        if (frenteButton != null) frenteImage = frenteButton.GetComponentInChildren<Image>();
        if (mentonButton != null) mentonImage = mentonButton.GetComponentInChildren<Image>();

        DeactivateAllInteractions();
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
        string mensaje = "¡Persona ahogándose! ¡Tienes 4 minutos!";
        RescueManager.Instance.TriggerInitialAlert_VR(mensaje, 240.0f);
    }

    private void EnterRescuedState()
    {
        currentState = State.Rescued;

        if (animator != null)
        {
            animator.SetBool("Nado", false);
            animator.SetBool("Agarrado", false);
            animator.SetBool("Echado", true);
        }

        if (drowningIndicator != null)
            drowningIndicator.SetActive(false);
    }

    void SetNewRandomWaypoint()
    {
        if (swimWaypoints.Length > 0)
        {
            int randomIndex = Random.Range(0, swimWaypoints.Length);
            currentWaypoint = swimWaypoints[randomIndex];
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

    // =================================================================
    // --- ¡FUNCIONES DE INTERACCIÓN REVERTIDAS (SIMPLES)! ---
    // =================================================================

    /// <summary>
    /// (VERSIÓN SIMPLE) Activa un botón y lo hace brillar (solo la imagen).
    /// </summary>
    public void ActivateInteraction(BodyPart part, bool shouldGlow)
    {
        // 1. Encendemos el canvas general si no lo está
        if (interactionCanvas != null && !interactionCanvas.activeSelf)
        {
            interactionCanvas.SetActive(true);
        }

        // 2. Usamos un Switch para manejar cada parte del cuerpo
        switch (part)
        {
            case BodyPart.Hombros:
                if (hombrosButton != null)
                {
                    hombrosButton.SetActive(true);
                    if (shouldGlow && hombrosImage != null)
                    {
                        hombrosTween?.Kill();
                        hombrosImage.color = Color.white;
                        hombrosTween = hombrosImage.DOFade(0.5f, 0.75f)
                                        .SetEase(Ease.InOutSine)
                                        .SetLoops(-1, LoopType.Yoyo);
                    }
                }
                break;

            case BodyPart.Frente:
                if (frenteButton != null)
                {
                    frenteButton.SetActive(true);
                    if (shouldGlow && frenteImage != null)
                    {
                        frenteTween?.Kill();
                        frenteImage.color = Color.white;
                        frenteTween = frenteImage.DOFade(0.5f, 0.75f)
                                        .SetEase(Ease.InOutSine)
                                        .SetLoops(-1, LoopType.Yoyo);
                    }
                }
                break;

            case BodyPart.Menton:
                if (mentonButton != null)
                {
                    mentonButton.SetActive(true);
                    if (shouldGlow && mentonImage != null)
                    {
                        mentonTween?.Kill();
                        mentonImage.color = Color.white;
                        mentonTween = mentonImage.DOFade(0.5f, 0.75f)
                                        .SetEase(Ease.InOutSine)
                                        .SetLoops(-1, LoopType.Yoyo);
                    }
                }
                break;

                // (Si necesitas añadir 'Pies', 'Manos', etc.,
                // tendrías que añadir un nuevo 'case' aquí manualmente)
        }
    }

    /// <summary>
    /// (VERSIÓN SIMPLE) Desactiva un botón y detiene su brillo.
    /// </summary>
    public void DeactivateInteraction(BodyPart part)
    {
        switch (part)
        {
            case BodyPart.Hombros:
                hombrosTween?.Kill(); // Detiene la animación
                if (hombrosImage != null) hombrosImage.color = Color.white; // Resetea la opacidad
                if (hombrosButton != null) hombrosButton.SetActive(false); // Oculta el botón
                break;

            case BodyPart.Frente:
                frenteTween?.Kill();
                if (frenteImage != null) frenteImage.color = Color.white;
                if (frenteButton != null) frenteButton.SetActive(false);
                break;

            case BodyPart.Menton:
                mentonTween?.Kill();
                if (mentonImage != null) mentonImage.color = Color.white;
                if (mentonButton != null) mentonButton.SetActive(false);
                break;
        }
    }

    /// <summary>
    /// (VERSIÓN SIMPLE) Desactiva TODOS los botones.
    /// </summary>
    public void DeactivateAllInteractions()
    {
        // Detenemos todas las animaciones
        hombrosTween?.Kill();
        frenteTween?.Kill();
        mentonTween?.Kill();

        // Reseteamos el color de todas las imágenes
        if (hombrosImage != null) hombrosImage.color = Color.white;
        if (frenteImage != null) frenteImage.color = Color.white;
        if (mentonImage != null) mentonImage.color = Color.white;

        // Ocultamos todos los botones
        if (hombrosButton != null) hombrosButton.SetActive(false);
        if (frenteButton != null) frenteButton.SetActive(false);
        if (mentonButton != null) mentonButton.SetActive(false);
    }
}
