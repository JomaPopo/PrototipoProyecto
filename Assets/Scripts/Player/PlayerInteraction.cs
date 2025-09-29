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
    [SerializeField] private GameObject crosshair;

    private Camera playerCamera;
    private NPCController currentlyCarriedNPC = null;
    private Rigidbody carriedNpcRigidbody = null;

    void Start()
    {
        playerCamera = GetComponentInChildren<Camera>();
        HideInteractionText();
    }

    void Update()
    {

        Debug.DrawRay(playerCamera.transform.position, playerCamera.transform.forward * interactionDistance, Color.cyan);

        if (currentlyCarriedNPC == null)
        {
            CheckForInteractables();
        }
        else 
        {
            CheckForRescueZone();
        }

        HandleCarryInput();

        HandleUIClickInput();
    }

    private void CheckForInteractables()
    {
        RaycastHit hit;
        bool hitSomethingInteractable = false;

        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, interactionDistance))
        {
            NPCController npc = hit.collider.GetComponent<NPCController>();

            if (npc != null && npc.CurrentState == NPCController.State.Drowning)
            {
                ShowInteractionText("Presiona [E] para Rescatar");
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
            }
        }
        else
        {
            ShowInteractionText("Busca un lugar seguro (una toalla)");
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
        HideInteractionText();
        if (carriedNpcRigidbody != null) carriedNpcRigidbody.isKinematic = true;
        npcToCarry.transform.SetParent(carryPosition);
        npcToCarry.transform.localPosition = Vector3.zero;
        npcToCarry.transform.localRotation = Quaternion.identity;
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
        if (carriedNpcRigidbody != null) carriedNpcRigidbody.isKinematic = false;
        RescueManager.Instance.StartRescueSequence(currentlyCarriedNPC);
        currentlyCarriedNPC.OnRescued();
        currentlyCarriedNPC = null;
        carriedNpcRigidbody = null;
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
                    button.onClick.Invoke();
                    break;
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
}