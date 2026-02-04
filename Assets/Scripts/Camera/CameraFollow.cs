using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target; // Assign your player in the Inspector

    [Header("Follow Settings")]
    [SerializeField] private Vector3 offset = new Vector3(0, 2, -10);
    [SerializeField] private float smoothSpeed = 0.125f;
    [SerializeField] private bool useSmoothing = true;

    [Header("Bounds (Optional)")]
    [SerializeField] private bool useBounds = false;
    [SerializeField] private Vector2 minBounds;
    [SerializeField] private Vector2 maxBounds;

    private void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPosition = target.position + offset;

        // Apply smoothing or instant follow
        Vector3 smoothedPosition = useSmoothing
            ? Vector3.Lerp(transform.position, desiredPosition, smoothSpeed)
            : desiredPosition;

        // Clamp to bounds if enabled
        if (useBounds)
        {
            smoothedPosition.x = Mathf.Clamp(smoothedPosition.x, minBounds.x, maxBounds.x);
            smoothedPosition.y = Mathf.Clamp(smoothedPosition.y, minBounds.y, maxBounds.y);
        }

        transform.position = smoothedPosition;
    }
}