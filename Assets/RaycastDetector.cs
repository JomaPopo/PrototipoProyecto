using UnityEngine;

public class RaycastDetector : MonoBehaviour
{
    [Header("Raycast Settings")]
    public float rayDistance = 5f;
    public LayerMask raycastLayer;

    [Header("Mouse Look Settings")]
    public float mouseSensitivity = 2f;
    private float verticalRotation = 0f;
    private float horizontalRotation = 0f;

    private StepTriggerReceiver currentReceiver = null;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        RotarCamara();
        LanzarRaycast();

        // Solo activamos el paso si el jugador hace clic y hay un receptor actual
        if (Input.GetMouseButtonDown(0) && currentReceiver != null)
        {
            currentReceiver.ActivarPaso();
        }
    }

    void RotarCamara()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        horizontalRotation += mouseX;
        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -90f, 90f);

        transform.rotation = Quaternion.Euler(verticalRotation, horizontalRotation, 0f);
    }

    void LanzarRaycast()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        currentReceiver = null;

        if (Physics.Raycast(ray, out RaycastHit hit, rayDistance, raycastLayer))
        {
            StepTriggerReceiver trigger = hit.collider.GetComponent<StepTriggerReceiver>();
            if (trigger != null)
            {
                currentReceiver = trigger;
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, transform.forward * rayDistance);
    }
}
