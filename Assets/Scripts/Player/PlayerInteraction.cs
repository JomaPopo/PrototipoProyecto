using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Parámetros de Interacción")]
    [SerializeField] private float interactionDistance = 3f;
    [SerializeField] private Transform carryPosition;
    [SerializeField] private float dropCheckDistance = 2f;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI interactionText;

    private Camera playerCamera;
    private NPCController currentlyCarriedNPC = null;
    private Rigidbody carriedNpcRigidbody = null;

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
            CheckForInteractables();
        }
        else
        {
            CheckForRescueZone();
        }

        HandleInteractionInput();

        if (Input.GetMouseButtonDown(0))
        {
            TryInteractWithWorldUI();
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
            }
        }
        else
        {
            ShowInteractionText("Busca un lugar seguro (una toalla)");
        }
    }

    private void HandleInteractionInput()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (currentlyCarriedNPC != null)
            {
                TryToDropNPC();
            }
            else
            {
                TryToInteract();
            }
        }
    }

    private void TryToDropNPC()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, dropCheckDistance))
        {
            RescueZone zone = hit.collider.GetComponent<RescueZone>();
            if (zone != null)
            {
                DropNPC(zone);
            }
        }
    }

    private void DropNPC(RescueZone zone)
    {
        currentlyCarriedNPC.transform.SetParent(null);

        currentlyCarriedNPC.transform.position = zone.dropPoint.position;
        currentlyCarriedNPC.transform.rotation = zone.dropPoint.rotation;

        if (carriedNpcRigidbody != null)
        {
            carriedNpcRigidbody.isKinematic = false;
        }

        // Aquí es donde en el futuro le diremos que entre en el estado "Listo para RCP"
        Debug.Log($"NPC {currentlyCarriedNPC.name} dejado en la zona de rescate {zone.name}. Listo para RCP.");

        RescueManager.Instance.StartRescueSequence(currentlyCarriedNPC);

        currentlyCarriedNPC = null;
        carriedNpcRigidbody = null;
    }

    // --- El resto de funciones no necesitan cambios ---
    private void PickUpNPC(NPCController npcToCarry)
    {
        currentlyCarriedNPC = npcToCarry;
        carriedNpcRigidbody = npcToCarry.GetComponent<Rigidbody>();

        HideInteractionText();

        if (carriedNpcRigidbody != null)
            carriedNpcRigidbody.isKinematic = true;

        npcToCarry.transform.SetParent(carryPosition);
        npcToCarry.transform.localPosition = Vector3.zero;
        npcToCarry.transform.localRotation = Quaternion.identity;
    }

    private void CheckForInteractables()
    {
        RaycastHit hit;
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, interactionDistance))
        {
            NPCController npc = hit.collider.GetComponent<NPCController>();
            if (npc != null)
            {
                // Si el NPC se está ahogando
                if (npc.CurrentState == NPCController.State.Drowning)
                {
                    ShowInteractionText("Presiona [E] para Rescatar");
                    return;
                }
                // Si el NPC está en la toalla y estamos en el paso correcto
                if (RescueManager.Instance.currentState == RescueManager.RescueState.VictimRescued)
                {
                    ShowInteractionText("Presiona [E] para comprobar si responde");
                    return;
                }
            }
        }
        HideInteractionText();
    }

    private void TryToInteract()
    {
        RaycastHit hit;
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, interactionDistance))
        {
            NPCController npc = hit.collider.GetComponent<NPCController>();
            if (npc != null)
            {
                if (npc.CurrentState == NPCController.State.Drowning)
                {
                    PickUpNPC(npc);
                }
                else if (RescueManager.Instance.currentState == RescueManager.RescueState.VictimRescued)
                {
                    // ¡NUEVO! Le decimos al manager que realice la acción
                    RescueManager.Instance.PerformConsciousnessCheck();
                }
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

    private void TryInteractWithWorldUI()
    {
        PointerEventData pointerData = new PointerEventData(EventSystem.current);
        pointerData.position = new Vector2(Screen.width / 2f, Screen.height / 2f);
        List<RaycastResult> results = new List<RaycastResult>();
        GraphicRaycaster[] raycasters = FindObjectsOfType<GraphicRaycaster>();
        foreach (var raycaster in raycasters)
        {
            raycaster.Raycast(pointerData, results);
        }

        if (results.Count > 0)
        {
            foreach (var result in results)
            {
                Button button = result.gameObject.GetComponent<Button>();
                if (button != null)
                {
                    Debug.Log("¡Clic en el botón del mundo: " + button.name);
                    button.onClick.Invoke();
                    break;
                }
            }
        }
    }
}