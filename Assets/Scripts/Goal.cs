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
                // Inside your Goal.cs where the player wins:
                if (progress.touchedCheckpoints.Count >= progress.totalCheckpointsRequired)
                {
                    Debug.Log("GOAL REACHED! You got all the checkpoints!");
    
                    // Find the UIManager and stop the clock
                    UIManager uiManager = FindFirstObjectByType<UIManager>();
                    if (uiManager != null)
                    {
                        uiManager.StopTimer();
                    }
                }

            }
            else
            {
                Debug.Log($"Can't finish yet! Only got {progress.touchedCheckpoints.Count} / {progress.totalCheckpointsRequired} checkpoints!");
            }
        }
    }
}