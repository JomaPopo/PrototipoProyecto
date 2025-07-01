using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float acceleration = 10f;
    public float deceleration = 15f;

    [Header("References")]
    public Transform cameraTransform; 

    private Rigidbody rb;
    private Vector2 inputVector;
    private bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true; 
        this.enabled = false; 


        if (cameraTransform == null)
        {
            cameraTransform = Camera.main.transform;
        }
    }

    void Update()
    {
        if (Keyboard.current == null) return;

        // Recoger entrada de teclado
        inputVector = new Vector2(
            (Keyboard.current.aKey.isPressed ? -1 : 0) + (Keyboard.current.dKey.isPressed ? 1 : 0),
            (Keyboard.current.sKey.isPressed ? -1 : 0) + (Keyboard.current.wKey.isPressed ? 1 : 0)
        ).normalized;
    }

    void FixedUpdate()
    {

        Vector3 cameraForward = Vector3.ProjectOnPlane(cameraTransform.forward, Vector3.up).normalized;
        Vector3 cameraRight = Vector3.ProjectOnPlane(cameraTransform.right, Vector3.up).normalized;

        Vector3 moveDirection = (cameraForward * inputVector.y + cameraRight * inputVector.x).normalized;

        MovePlayer(moveDirection);
    }

    private void MovePlayer(Vector3 direction)
    {
        
        Vector3 targetVelocity = direction * moveSpeed;

        Vector3 velocityDifference = targetVelocity - new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);

        float accelerationRate = (targetVelocity.magnitude > 0.01f) ? acceleration : deceleration;

        Vector3 force = velocityDifference * accelerationRate;
        rb.AddForce(force, ForceMode.Acceleration);
    }

    public void EnableMovement()
    {
        this.enabled = true;
    }
}