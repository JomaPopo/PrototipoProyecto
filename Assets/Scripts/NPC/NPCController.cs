using UnityEngine;
using UnityEngine.UI;  
using DG.Tweening;     
using TMPro;          
using System.Collections;

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
    public Animator animator; 
    public GameObject cuerpoNPC;

    [Header("Material a Cambiar")]
    public Material materialNuevo;
    public SkinnedMeshRenderer meshRenderer;
    private Material materialOriginal;

    [Header("Botones de Interacción")]
    public GameObject hombrosButton;
    public GameObject frenteButton;
    public GameObject mentonButton;

    private Tween hombrosTween;
    private Tween frenteTween;
    private Tween mentonTween;

    private Image hombrosImage;
    private Image frenteImage;
    private Image mentonImage;
    public GameObject waterSpitParticles;

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

   
    public void ActivateInteraction(BodyPart part, bool shouldGlow)
    {
        if (interactionCanvas != null && !interactionCanvas.activeSelf)
        {
            interactionCanvas.SetActive(true);
        }

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

        }
    }

    public void DeactivateInteraction(BodyPart part)
    {
        switch (part)
        {
            case BodyPart.Hombros:
                hombrosTween?.Kill(); 
                if (hombrosImage != null) hombrosImage.color = Color.white; 
                if (hombrosButton != null) hombrosButton.SetActive(false); 
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
    public void OnRevived()
    {
        Debug.Log("¡La víctima ha revivido!");

        CambiarAlMaterialOriginal();

        if (waterSpitParticles != null)
        {
            waterSpitParticles.SetActive(true);
            
         
        }

    }
    public void DeactivateAllInteractions()
    {
        hombrosTween?.Kill();
        frenteTween?.Kill();
        mentonTween?.Kill();

        if (hombrosImage != null) hombrosImage.color = Color.white;
        if (frenteImage != null) frenteImage.color = Color.white;
        if (mentonImage != null) mentonImage.color = Color.white;

        if (hombrosButton != null) hombrosButton.SetActive(false);
        if (frenteButton != null) frenteButton.SetActive(false);
        if (mentonButton != null) mentonButton.SetActive(false);
    }
}
