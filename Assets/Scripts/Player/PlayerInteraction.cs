using UnityEngine;
using TMPro;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Parámetros de Interacción")]
    [SerializeField] private float interactionDistance = 3f;
    [SerializeField] private Transform carryPosition;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI interactionText;

    private Camera playerCamera;
    private NPCStateController currentlyCarriedNPC = null;
    private Rigidbody carriedNpcRigidbody = null; // Guardamos una referencia a su Rigidbody

    void Start()
    {
        playerCamera = GetComponentInChildren<Camera>();
        if (interactionText != null)
        {
            interactionText.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (currentlyCarriedNPC == null)
        {
            CheckForInteractable();
        }
        else
        {
            // Si estamos cargando a alguien, el texto no debe aparecer
            if (interactionText != null && interactionText.gameObject.activeSelf)
            {
                interactionText.gameObject.SetActive(false);
            }
        }

        HandleInteractionInput();
    }

    private void CheckForInteractable()
    {
        RaycastHit hit;
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, interactionDistance))
        {
            NPCStateController npc = hit.collider.GetComponent<NPCStateController>();

            if (npc != null && npc.CurrentState is NPCDrownState)
            {
                if (interactionText != null)
                {
                    interactionText.text = "Presiona [E] para Rescatar";
                    interactionText.gameObject.SetActive(true);
                }
                return;
            }
        }

        if (interactionText != null)
        {
            interactionText.gameObject.SetActive(false);
        }
    }

    private void HandleInteractionInput()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (currentlyCarriedNPC != null)
            {
                DropNPC();
            }
            else
            {
                TryToPickUpNPC();
            }
        }
    }

    private void TryToPickUpNPC()
    {
        RaycastHit hit;
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, interactionDistance))
        {
            NPCStateController npc = hit.collider.GetComponent<NPCStateController>();

            if (npc != null && npc.CurrentState is NPCDrownState)
            {
                PickUpNPC(npc);
            }
        }
    }

    private void PickUpNPC(NPCStateController npcToCarry)
    {
        currentlyCarriedNPC = npcToCarry;
        carriedNpcRigidbody = npcToCarry.GetComponent<Rigidbody>(); // Obtenemos su Rigidbody

        // --- ¡LA MAGIA ESTÁ AQUÍ! ---
        if (carriedNpcRigidbody != null)
        {
            // Lo hacemos Kinematic para que no reaccione a la física
            carriedNpcRigidbody.isKinematic = true;
        }

        npcToCarry.agent.enabled = false;

        npcToCarry.transform.SetParent(carryPosition);
        npcToCarry.transform.localPosition = Vector3.zero;
        npcToCarry.transform.localRotation = Quaternion.identity;
    }

    private void DropNPC()
    {
        currentlyCarriedNPC.transform.SetParent(null);

        // --- Y AQUÍ LO DEVOLVEMOS A LA NORMALIDAD ---
        if (carriedNpcRigidbody != null)
        {
            // Le devolvemos la física para que caiga al suelo de forma realista
            carriedNpcRigidbody.isKinematic = false;
        }

        currentlyCarriedNPC.agent.enabled = true;

        currentlyCarriedNPC = null;
        carriedNpcRigidbody = null;
    }
}