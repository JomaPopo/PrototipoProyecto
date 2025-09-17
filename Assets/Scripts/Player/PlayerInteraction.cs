using System.Collections.Generic; 
using UnityEngine;
using UnityEngine.EventSystems; 
using UnityEngine.UI; 
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
    private Rigidbody carriedNpcRigidbody = null;

    private GraphicRaycaster currentGraphicRaycaster;

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
            CheckForPhysicalInteractable();
        }
        else
        {
            if (interactionText != null && interactionText.gameObject.activeSelf)
            {
                interactionText.gameObject.SetActive(false);
            }
        }

        HandleInteractionInput();

        if (Input.GetMouseButtonDown(0)) 
        {
            TryInteractWithWorldUI();
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

    private void CheckForPhysicalInteractable()
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
        carriedNpcRigidbody = npcToCarry.GetComponent<Rigidbody>();

        if (carriedNpcRigidbody != null)
        {
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

        if (carriedNpcRigidbody != null)
        {
            carriedNpcRigidbody.isKinematic = false;
        }

        currentlyCarriedNPC.agent.enabled = true;

        currentlyCarriedNPC = null;
        carriedNpcRigidbody = null;
    }
}