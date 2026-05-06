using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class SimpleCarController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 10f;
    public float turnSpeed = 100f;

    [Header("Input System")]
    [SerializeField] private InputActionReference moveAction;

    private Rigidbody rb;

    private void Start()
    {
        // Grab the Rigidbody attached to the car
        rb = GetComponent<Rigidbody>();
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

    // Use FixedUpdate for Rigidbody physics
    void FixedUpdate() 
    {
        if (moveAction == null) return;

        Vector2 input = moveAction.action.ReadValue<Vector2>();

        float moveInput = input.y;   // W/S or Up/Down
        float turnInput = input.x;   // A/D or Left/Right

        // 1. Calculate how much to move
        Vector3 moveDelta = transform.forward * moveInput * moveSpeed * Time.fixedDeltaTime;
        // Move the Rigidbody securely, checking for collisions along the way
        rb.MovePosition(rb.position + moveDelta);

        // 2. Calculate how much to rotate
        Quaternion turnDelta = Quaternion.Euler(0f, turnInput * turnSpeed * Time.fixedDeltaTime, 0f);
        // Rotate the Rigidbody securely
        rb.MoveRotation(rb.rotation * turnDelta);
    }
}