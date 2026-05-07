using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic; // Required for HashSet

public class PlayerProgress : MonoBehaviour
{
    [Header("Input System")]
    [SerializeField] private InputActionReference resetAction;

    [Header("Checkpoint Tracking")]
    public int totalCheckpointsRequired = 3;
    
    // A HashSet keeps a list of unique items. If you touch Checkpoint 1 twice, it still only counts once.
    public HashSet<int> touchedCheckpoints = new HashSet<int>();
    
    private Vector3 lastRespawnPosition;
    private Quaternion lastRespawnRotation;
    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        
        // Use our starting position as the first respawn point
        lastRespawnPosition = transform.position;
        lastRespawnRotation = transform.rotation;
    }

    private void OnEnable()
    {
        if (resetAction != null) resetAction.action.Enable();
    }

    private void OnDisable()
    {
        if (resetAction != null) resetAction.action.Disable();
    }

    private void Update()
    {
        // Check if the reset button was pressed
        if (resetAction != null && resetAction.action.WasPressedThisFrame())
        {
            Respawn();
        }
    }

    public void TouchCheckpoint(int id, Vector3 spawnPos, Quaternion spawnRot)
    {
        touchedCheckpoints.Add(id);
        lastRespawnPosition = spawnPos;
        lastRespawnRotation = spawnRot;
        
        Debug.Log($"Checkpoint {id} Reached! Total unique touched: {touchedCheckpoints.Count}");
    }

    private void Respawn()
    {
        // Move the car
        transform.position = lastRespawnPosition;
        transform.rotation = lastRespawnRotation;
        
        // Kill any momentum so we don't instantly fly away when resetting
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }
}