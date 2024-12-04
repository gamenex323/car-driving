using UnityEngine;

public class CameraBoundsController : MonoBehaviour
{
    public Camera cameraToBound;  // Reference to the camera
    public BoxCollider2D boundingBox;  // The 2D collider acting as bounds

    private Vector3 _minBounds;
    private Vector3 _maxBounds;
    private float _cameraHalfHeight;
    private float _cameraHalfWidth;

    void Start()
    {
        if (cameraToBound == null)
        {
            cameraToBound = Camera.main;
        }

        if (boundingBox != null)
        {
            // Get the bounds of the BoxCollider2D
            _minBounds = boundingBox.bounds.min;
            _maxBounds = boundingBox.bounds.max;

            // Calculate camera dimensions in world units
            _cameraHalfHeight = cameraToBound.orthographicSize;
            _cameraHalfWidth = _cameraHalfHeight * cameraToBound.aspect;
        }
        else
        {
            Debug.LogError("Bounding Box is not assigned!");
        }
    }

    void LateUpdate()
    {
        if (boundingBox == null) return;

        // Get the current camera position
        Vector3 cameraPosition = cameraToBound.transform.position;

        // Clamp the camera position to the bounds
        float clampedX = Mathf.Clamp(cameraPosition.x, _minBounds.x + _cameraHalfWidth, _maxBounds.x - _cameraHalfWidth);
        float clampedY = Mathf.Clamp(cameraPosition.y, _minBounds.y + _cameraHalfHeight, _maxBounds.y - _cameraHalfHeight);

        // Apply the clamped position
        cameraToBound.transform.position = new Vector3(clampedX, clampedY, cameraPosition.z);
    }

    private void OnDrawGizmos()
    {
        if (boundingBox != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(boundingBox.bounds.center, boundingBox.bounds.size);
        }
    }
}
