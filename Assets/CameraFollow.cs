using UnityEngine;

public class CameraFollow2D : MonoBehaviour
{
    [Header("Camera Settings")]
    [SerializeField] Transform target;  // The object the camera will follow (e.g., car)
    [SerializeField] float smoothSpeed = 0.125f; // Damping factor for smooth follow
    [SerializeField] Vector3 offset; // Offset for camera position (use this to adjust distance)
    [SerializeField] float height = 5f; // Optional height for the camera (if needed)

    void Update()
    {
        if (target != null)
        {
            // Define the desired position, taking the target position and applying the offset
            Vector3 desiredPosition = target.position + offset;

            // Keep the camera's height constant (for 2D view, set y position directly)
            desiredPosition.z = transform.position.z;  // Keep the z position unchanged

            // Smoothly interpolate between the camera's current position and the desired position
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

            // Set the camera's position to the smoothed position
            transform.position = smoothedPosition;
        }
    }
}
