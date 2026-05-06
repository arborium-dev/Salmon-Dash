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

    [Header("Gravity & Feel")]
    public float extraGravity = 40f;       // Makes the car fall faster (fixes floatiness)
    public float downforce = 20f;          // Pushes the car into the ground when driving fast

    [Header("Ground Check")]
    public float groundCheckDistance = 1f; 
    public LayerMask groundLayer = ~0;     

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
        if (moveAction != null) moveAction.action.Enable();
    }

    private void OnDisable()
    {
        if (moveAction != null) moveAction.action.Disable();
    }

    void FixedUpdate() 
    {
        Vector3 rayStart = transform.position + (Vector3.up * 0.1f);
        isGrounded = Physics.Raycast(rayStart, Vector3.down, groundCheckDistance, groundLayer);

        // 1. CUSTOM GRAVITY (Fixes the moon-jump floaty feel)
        rb.AddForce(Vector3.down * extraGravity, ForceMode.Acceleration);

        if (moveAction == null) return;

        Vector2 input = moveAction.action.ReadValue<Vector2>();
        float moveInput = input.y;
        float turnInput = input.x;

        if (isGrounded)
        {
            // Acceleration
            rb.AddForce(transform.forward * moveInput * acceleration, ForceMode.Acceleration);
            // Turning
            rb.AddRelativeTorque(Vector3.up * turnInput * turnSpeed, ForceMode.Acceleration);

            // Arcade Grip
            Vector3 rightVelocity = transform.right * Vector3.Dot(rb.linearVelocity, transform.right);
            rb.linearVelocity -= rightVelocity * grip;

            // 2. DOWNFORCE (The faster you go, the more it pushes you into the road)
            float speedFactor = rb.linearVelocity.magnitude / maxSpeed;
            rb.AddForce(Vector3.down * downforce * speedFactor, ForceMode.Acceleration);
        }

        // Limit Maximum Speed
        Vector3 flatVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        if (flatVelocity.magnitude > maxSpeed)
        {
            Vector3 limitedVelocity = flatVelocity.normalized * maxSpeed;
            rb.linearVelocity = new Vector3(limitedVelocity.x, rb.linearVelocity.y, limitedVelocity.z);
        }
    }

    private void Update()
    {
        // View the ground check in the Scene View while playing
        Vector3 rayStart = transform.position + (Vector3.up * 0.1f);
        Color lineColor = isGrounded ? Color.green : Color.red;
        Debug.DrawRay(rayStart, Vector3.down * groundCheckDistance, lineColor);
    }
}
