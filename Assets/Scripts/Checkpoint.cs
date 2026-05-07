using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public int checkpointID; // Set this to 1, 2, or 3 in the inspector

    private void OnTriggerEnter(Collider other)
    {
        // Try to find the PlayerProgress script on the car
        PlayerProgress progress = other.GetComponentInParent<PlayerProgress>();
        
        if (progress != null)
        {
            // Give the player this checkpoint's ID, and use this transform for respawning
            progress.TouchCheckpoint(checkpointID, transform.position, transform.rotation);
        }
    }
}