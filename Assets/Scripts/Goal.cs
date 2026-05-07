using UnityEngine;

public class Goal : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        PlayerProgress progress = other.GetComponentInParent<PlayerProgress>();
        
        if (progress != null)
        {
            if (progress.touchedCheckpoints.Count >= progress.totalCheckpointsRequired)
            {
                Debug.Log("GOAL REACHED! You got all the checkpoints!");
                // Put code for winning/next level here
            }
            else
            {
                Debug.Log($"Can't finish yet! Only got {progress.touchedCheckpoints.Count} / {progress.totalCheckpointsRequired} checkpoints!");
            }
        }
    }
}