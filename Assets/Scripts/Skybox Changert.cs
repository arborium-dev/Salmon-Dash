using UnityEngine;

public class SkyboxChanger : MonoBehaviour
{
    [Header("The new Skybox Material")]
    public Material newSkybox;

    private void OnTriggerEnter(Collider other)
    {
        // Check if the object touching it is the car
        if (other.CompareTag("Player"))
        {
            // Change the skybox
            if (newSkybox != null)
            {
                RenderSettings.skybox = newSkybox;
            }
        }
    }
}