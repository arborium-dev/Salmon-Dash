using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class SimpleCarController : MonoBehaviour
{
    [Header("Movement (Physics)")]
    public float acceleration = 30f;
    public float maxSpeed = 20f;
    public float turnSpeed = 5f;
    
    [Range(0f, 1f)]
    public float grip = 0.95f;

    [Header("Ground Check")]
    public float groundCheckDistance = 1f; // How long the downward laser is
    public LayerMask groundLayer = ~0;     // What counts as ground (~0 means "Everything")

    [Header("Input System")]
    [SerializeField] private InputActionReference moveAction;

    private Rigidbody rb;
    private bool isGrounded;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.interpolation = RigidbodyInterpolation.Interpolate;
    }

    private void OnEnable()
    {
        if (moveAction != null)
            moveAction.action.Enable();
    }

    private void OnDisable()
    {
        if (moveAction != null)
            moveAction.action.Disable();
    }

    void FixedUpdate() 
    {
        // 1. GROUND CHECK
        // Shoot a ray straight down from slightly above the car's origin
        Vector3 rayStart = transform.position + (Vector3.up * 0.1f);
        isGrounded = Physics.Raycast(rayStart, Vector3.down, groundCheckDistance, groundLayer);

        if (moveAction == null) return;

        Vector2 input = moveAction.action.ReadValue<Vector2>();
        float moveInput = input.y;
        float turnInput = input.x;

        // ONLY apply driving forces if the car is touching the ground
        if (isGrounded)
        {
            // Acceleration (Gas/Brake)
            rb.AddForce(transform.forward * moveInput * acceleration, ForceMode.Acceleration);

            // Turning (Steering)
            rb.AddRelativeTorque(Vector3.up * turnInput * turnSpeed, ForceMode.Acceleration);

            // Arcade Grip (Cancels out sideways sliding)
            Vector3 rightVelocity = transform.right * Vector3.Dot(rb.linearVelocity, transform.right);
            rb.linearVelocity -= rightVelocity * grip;
        }

        // Limit Maximum Speed (We do this everywhere so you don't break the sound barrier mid-air)
        Vector3 flatVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        if (flatVelocity.magnitude > maxSpeed)
        {
            Vector3 limitedVelocity = flatVelocity.normalized * maxSpeed;
            rb.linearVelocity = new Vector3(limitedVelocity.x, rb.linearVelocity.y, limitedVelocity.z);
        }
    }

    // This draws a red line in the Unity Editor so you can see your ground check!
    private void OnDrawGizmos()
    {
        Gizmos.color = isGrounded ? Color.green : Color.red;
        Vector3 rayStart = transform.position + (Vector3.up * 0.1f);
        Gizmos.DrawLine(rayStart, rayStart + (Vector3.down * groundCheckDistance));
    }
}
