using UnityEngine;

public class Spinner : MonoBehaviour
{
    [Header("Spin Settings")]
    // Adjust these values in the Inspector to control spin speed and direction.
    // Y = 100 means it will spin around the Y (vertical) axis.
    public Vector3 spinSpeed = new Vector3(0f, 100f, 0f);

    void Update()
    {
        // Rotates the object smoothly every frame based on the spinSpeed
        transform.Rotate(spinSpeed * Time.deltaTime);
    }
}