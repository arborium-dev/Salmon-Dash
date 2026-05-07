using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class SimpleCarController : MonoBehaviour
{
    [Header("Movement (Ground)")]
    public float acceleration = 60f;        
    public float maxSpeed = 20f;
    public float turnSpeed = 120f;          
    [Range(0f, 1f)] public float grip = 0.95f;

    [Header("Air Control")]
    public float airPitchSpeed = 5f;     
    public float airYawSpeed = 4f;       
    public float autoAlignSpeed = 2f;    

    [Header("Gravity & Feel")]
    public float baseGravity = 40f;        
    public float fallMultiplier = 1.5f;    
    public float downforce = 20f;          

    [Header("Ground Check")]
    public float groundCheckDistance = 1.5f; 
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

        // SMART GRAVITY
        float currentGravity = baseGravity;
        if (rb.linearVelocity.y < 0 && !isGrounded)
        {
            currentGravity *= fallMultiplier; 
        }
        rb.AddForce(Vector3.down * currentGravity, ForceMode.Acceleration);

        if (moveAction == null) return;
        Vector2 input = moveAction.action.ReadValue<Vector2>();
        float pitchInput = input.y;
        float turnInput = input.x;

        if (isGrounded)
        {
            // --- GROUND MOVEMENT ---
            
            // 0. KILL LEFTOVER SPIN (Instantly stops the car from continuing to turn after you let go!)
            Vector3 localSpin = transform.InverseTransformDirection(rb.angularVelocity);
            localSpin.y = 0f; // Erase horizontal spinning
            rb.angularVelocity = transform.TransformDirection(localSpin);

            // 1. Acceleration
            rb.AddForce(transform.forward * pitchInput * acceleration, ForceMode.Acceleration);
            
            // 2. SNAPPY ARCADE TURNING
            float currentSpeed = rb.linearVelocity.magnitude;
            if (currentSpeed > 0.5f) 
            {
                // Check if we are driving forward or in reverse
                float direction = Mathf.Sign(Vector3.Dot(rb.linearVelocity, transform.forward));
                
                // Directly rotate the car perfectly
                float turnAmount = turnInput * turnSpeed * direction * Time.fixedDeltaTime;
                Quaternion turnOffset = Quaternion.Euler(0, turnAmount, 0);
                rb.MoveRotation(rb.rotation * turnOffset);
            }

            // 3. Arcade Grip
            Vector3 rightVelocity = transform.right * Vector3.Dot(rb.linearVelocity, transform.right);
            rb.linearVelocity -= rightVelocity * grip;

            // 4. Downforce
            float speedFactor = rb.linearVelocity.magnitude / maxSpeed;
            rb.AddForce(Vector3.down * downforce * speedFactor, ForceMode.Acceleration);
        }
        else
        {
            // --- AIR MOVEMENT ---
            rb.AddRelativeTorque(Vector3.up * turnInput * airYawSpeed, ForceMode.Acceleration);    
            rb.AddRelativeTorque(Vector3.right * pitchInput * airPitchSpeed, ForceMode.Acceleration); 

            // Auto-Uprighting
            Vector3 alignForce = Vector3.Cross(transform.up, Vector3.up);
            rb.AddTorque(alignForce * autoAlignSpeed, ForceMode.Acceleration);
        }

        // --- SPEED LIMIT ---
        Vector3 flatVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        if (flatVelocity.magnitude > maxSpeed)
        {
            Vector3 limitedVelocity = flatVelocity.normalized * maxSpeed;
            rb.linearVelocity = new Vector3(limitedVelocity.x, rb.linearVelocity.y, limitedVelocity.z);
        }
    }

    private void Update()
    {
        Vector3 rayStart = transform.position + (Vector3.up * 0.1f);
        Color lineColor = isGrounded ? Color.green : Color.red;
        Debug.DrawRay(rayStart, Vector3.down * groundCheckDistance, lineColor);
    }
}
