using UnityEngine;

/// <summary>
/// Smoothly follows the player with a fixed offset and rotation.
/// </summary>
public class CameraFollow : MonoBehaviour
{
    [Header("Target Settings")]
    [SerializeField] private Transform target;

    [Header("Position Settings")]
    [SerializeField] private Vector3 offset = new Vector3(0f, 5f, -5f);
    [SerializeField, Range(0f, 10f)] private float smoothSpeed = 0.125f;

    [Header("Rotation Settings")]
    [SerializeField] private Vector3 rotation = new Vector3(45f, 0f, 0f);

    private void Start()
    {
        // Find player if not assigned
        if (target == null)
        {
            target = FindFirstObjectByType<Character>()?.transform;
            if (target == null)
            {
                Debug.LogWarning("Target not found in CameraFollow!", this);
                enabled = false;
            }
        }
    }

    private void LateUpdate()
    {
        if (target == null) return;

        // Smoothly position the camera
        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        transform.position = smoothedPosition;
        transform.rotation = Quaternion.Euler(rotation);
    }

    /// <summary>
    /// Sets a new target for the camera to follow.
    /// </summary>
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }
}