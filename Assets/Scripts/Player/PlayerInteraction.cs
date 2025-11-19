using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Parámetros de Interacción")]
    [SerializeField] private float interactionDistance = 3f;
    [SerializeField] private Transform carryPosition;
    [SerializeField] private float dropCheckDistance = 2f;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI interactionText; 
    [SerializeField] private GameObject crosshair;

   // private Camera playerCamera;
    private NPCController currentlyCarriedNPC = null;
    private Rigidbody carriedNpcRigidbody = null;
    private Collider carriedNpcCollider = null;

    private Transform currentPointer;

    [Header("Referencias de Puntero")]
    [SerializeField] private Transform playerCamera; // Para PC
    [SerializeField] private Transform vrControllerHand; // Para VR
    // Referencia al componente PlayerInput
    private PlayerInput playerInput;

    void Awake() // Awake() es mejor que Start() para esto
    {
        playerInput = GetComponent<PlayerInput>();

        // Detectamos si estamos en modo VR
        // (Asegúrate de tener "using UnityEngine.XR;")
        if (UnityEngine.XR.XRSettings.isDeviceActive)
        {
            Debug.Log("Modo VR Detectado. Usando el mando como puntero.");
            currentPointer = vrControllerHand;

            // Ocultamos el cursor del mouse de PC
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Debug.Log("Modo PC Detectado. Usando la cámara como puntero.");
            currentPointer = playerCamera;
        }
    }
    void Start()
    {
        //playerCamera = GetComponentInChildren<Camera>();
        HideInteractionText();
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        // Solo nos interesa cuando el botón es PRESIONADO
        if (!context.performed) return;

        if (currentlyCarriedNPC != null)
        {
            TryToDropNPC(); // Tu lógica de soltar
        }
        else
        {
            // ¡USA LA VARIABLE MÁGICA!
            RaycastHit hit;
            if (Physics.Raycast(currentPointer.position, currentPointer.forward, out hit, interactionDistance))
            {
                NPCController npc = hit.collider.GetComponent<NPCController>();
                if (npc != null && npc.CurrentState == NPCController.State.Drowning)
                {
                    PickUpNPC(npc); // Tu lógica de agarrar
                }
            }
        }
    }

    // ¡Esta función es llamada por el EVENTO del PlayerInput!
    // Reemplaza a HandleUIClickInput()
    public void OnSelect(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        // Si estamos en la fase de RCP...
        if (currentlyCarriedNPC == null &&
            (RescueManager.Instance.currentState == RescueManager.RescueState.VictimRescued ||
             RescueManager.Instance.currentState == RescueManager.RescueState.AirwayCheck))
        {
            TryInteractWithWorldUI();
        }
    }


    void Update()
    {

        Debug.DrawRay(currentPointer.position, currentPointer.forward * interactionDistance, Color.cyan);
        if (currentlyCarriedNPC == null)
        {
            CheckForInteractables();
        }
        else 
        {
            CheckForRescueZone();
        }

        //HandleCarryInput();

        //HandleUIClickInput();
    }

    private void CheckForInteractables()
    {
        RaycastHit hit;
        bool hitSomethingInteractable = false;

        if (Physics.Raycast(currentPointer.position, currentPointer.forward, out hit, interactionDistance))
        {
            NPCController npc = hit.collider.GetComponent<NPCController>();
            if (npc != null && npc.CurrentState == NPCController.State.Drowning)
            {
                ShowInteractionText("Presiona [Interact] para Rescatar"); // Texto genérico
                hitSomethingInteractable = true;
            }
        }

        if (!hitSomethingInteractable)
        {
            HideInteractionText();
        }
    }

    private void CheckForRescueZone()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, dropCheckDistance))
        {
            if (hit.collider.GetComponent<RescueZone>() != null)
            {
                ShowInteractionText("Presiona [E] para dejar a la persona");
            }
            else
            {
                ShowInteractionText("Busca un lugar seguro (una toalla)");
                UIManager.Instance.ShowInstruction("Lleva ala victima una zona segura");
            }
        }
        else
        {
            ShowInteractionText("Busca un lugar seguro (una toalla)");
            UIManager.Instance.ShowInstruction("Lleva ala victima una zona segura");
        }
    }

    private void HandleCarryInput()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (currentlyCarriedNPC != null)
            {
                TryToDropNPC();

            }
            else
            {
                TryToPickUpNPC();
            }
        }
    }

    private void HandleUIClickInput()
    {
        if (currentlyCarriedNPC == null && RescueManager.Instance.currentState == RescueManager.RescueState.VictimRescued)
        {
            if (crosshair != null && !crosshair.activeSelf) crosshair.SetActive(true);

            if (Input.GetMouseButtonDown(0)) 
            {
                TryInteractWithWorldUI();
            }
        }
        if (currentlyCarriedNPC == null && RescueManager.Instance.currentState == RescueManager.RescueState.AirwayCheck)
        {
            if (crosshair != null && !crosshair.activeSelf) crosshair.SetActive(true);

            if (Input.GetMouseButtonDown(0))
            {
                TryInteractWithWorldUI();
            }
        }
        else
        {
            if (crosshair != null && crosshair.activeSelf) crosshair.SetActive(false);
        }
    }

    private void TryToPickUpNPC()
    {
        RaycastHit hit;
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, interactionDistance))
        {
            NPCController npc = hit.collider.GetComponent<NPCController>();
            if (npc != null && npc.CurrentState == NPCController.State.Drowning)
            {
                PickUpNPC(npc);
            }
        }
    }

    private void PickUpNPC(NPCController npcToCarry)
    {
        currentlyCarriedNPC = npcToCarry;
        carriedNpcRigidbody = npcToCarry.GetComponent<Rigidbody>();
        carriedNpcCollider = npcToCarry.GetComponent<Collider>(); 

        HideInteractionText();

        if (carriedNpcRigidbody != null) carriedNpcRigidbody.isKinematic = true;

        npcToCarry.transform.SetParent(carryPosition);
        npcToCarry.transform.localPosition = Vector3.zero;
        npcToCarry.transform.localRotation = Quaternion.identity;
        AudioManager.Instance.PlayVoice(AudioManager.Instance.instructor_ConfirmacionRescate);
string contexto = "CONTEXTO: ¡Tienes a la víctima!";
    string instruccion = "¡Bien hecho! ¡Ahora lléala a la zona segura (la toalla roja)!";
    
    UIManager.Instance.ShowWristContext(contexto);
        UIManager.Instance.ShowWristInstruction_Instant(instruccion);
    }

    private void TryToDropNPC()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, dropCheckDistance))
        {
            RescueZone zone = hit.collider.GetComponent<RescueZone>();
            if (zone != null)
            {
                // Llamamos a la Corutina
                StartCoroutine(DropNPCSequence(zone));
            }
        }
    }
    private IEnumerator DropNPCSequence(RescueZone zone)
    {
        // Guardamos referencias ANTES de limpiar las variables de clase
        NPCController npcToDrop = currentlyCarriedNPC;
        Rigidbody rbToDrop = carriedNpcRigidbody;
        Collider colToDrop = carriedNpcCollider;

        // Limpiamos las variables de clase INMEDIATAMENTE para evitar doble input
        currentlyCarriedNPC = null;
        carriedNpcRigidbody = null;
        carriedNpcCollider = null;

        // 1. Soltarlo del padre
        npcToDrop.transform.SetParent(null);

        // 2. Desactivar collider para evitar choque inicial
        if (colToDrop != null) colToDrop.enabled = false;

        // 3. Posicionar y Rotar
        npcToDrop.transform.position = zone.dropPoint.position;
        npcToDrop.transform.rotation = zone.dropPoint.rotation;

        // 4. Esperar un frame de física para que la posición se asiente
        yield return new WaitForFixedUpdate();

        // 5. Reactivar collider
        if (colToDrop != null) colToDrop.enabled = true;

        // 6. Reactivar la física (Rigidbody)
        if (rbToDrop != null) rbToDrop.isKinematic = false;

        // --- ¡LA CORRECCIÓN MÁS IMPORTANTE! ---
        // 7. Notificar al RescueManager con la referencia correcta
        RescueManager.Instance.StartRescueSequence(npcToDrop); // Usamos npcToDrop

        // 8. Notificar al NPC
        npcToDrop.OnRescued();
    }
    private void DropNPC(RescueZone zone)
    {
        currentlyCarriedNPC.transform.SetParent(null);
        currentlyCarriedNPC.transform.position = zone.dropPoint.position;
        currentlyCarriedNPC.transform.rotation = zone.dropPoint.rotation;
        if (carriedNpcRigidbody != null) carriedNpcRigidbody.isKinematic = false;
        RescueManager.Instance.StartRescueSequence(currentlyCarriedNPC);
        currentlyCarriedNPC.OnRescued();
        currentlyCarriedNPC = null;
        carriedNpcRigidbody = null;
    }

    private void TryInteractWithWorldUI()
    {
        RaycastHit hit;
        // ¡USA LA VARIABLE MÁGICA!
        if (Physics.Raycast(currentPointer.position, currentPointer.forward, out hit, interactionDistance))
        {
            // Esto asume que tus botones en el NPC tienen un Collider 3D
            // (como un Box Collider) además del componente Button.
            Button button = hit.collider.GetComponent<Button>();
            if (button != null)
            {
                Debug.Log($"Hit de {currentPointer.name} al botón: {button.name}");
                button.onClick.Invoke(); // ¡Clic!
            }
        }
    }

    private void ShowInteractionText(string message)
    {
        if (interactionText != null)
        {
            interactionText.text = message;
            interactionText.gameObject.SetActive(true);
        }
    }

    private void HideInteractionText()
    {
        if (interactionText != null)
        {
            interactionText.gameObject.SetActive(false);
        }
    }
}