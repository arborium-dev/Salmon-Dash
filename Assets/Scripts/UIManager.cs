using UnityEngine;
using TMPro; // Required for TextMeshPro

public class UIManager : MonoBehaviour
{
    [Header("UI Text Elements")]
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI checkpointText;

    [Header("References")]
    public PlayerProgress playerProgress; // Drag your Car here in the Inspector

    private float trackTime = 0f;
    private bool isFinished = false;

    private void Update()
    {
        // 1. Handle Time
        if (!isFinished)
        {
            trackTime += Time.deltaTime; // Add time every frame
            
            // Math to convert total seconds into minutes, seconds, and milliseconds
            int minutes = Mathf.FloorToInt(trackTime / 60f);
            int seconds = Mathf.FloorToInt(trackTime % 60f);
            int milliseconds = Mathf.FloorToInt((trackTime * 1000f) % 1000f);
            
            // Format to look like a racing clock (e.g., 01:23.456)
            timeText.text = string.Format("Time: {0:00}:{1:00}.{2:000}", minutes, seconds, milliseconds);
        }

        // 2. Handle Checkpoint Count
        if (playerProgress != null)
        {
            checkpointText.text = $"{playerProgress.touchedCheckpoints.Count}/{playerProgress.totalCheckpointsRequired}";
        }
    }

    // You can call this method from your Goal.cs script when the player finishes!
    public void StopTimer()
    {
        isFinished = true;
    }
}