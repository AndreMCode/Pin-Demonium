using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] private Transform Target;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private Vector3 Offset;
    [SerializeField] private float smoothTime;

    [Header("Camera Boundaries")]
    [SerializeField] private Vector2 xBounds; // Min and Max X boundary
    [SerializeField] private Vector2 yBounds; // Min and Max Y boundary
    [SerializeField] private Vector2 zBounds; // Min and Max Z boundary

    private Vector3 velocity = Vector3.zero;

    private void LateUpdate()
    { // Camera position logic
        if (IsTargetWithinBounds())
        {
            Vector3 targetPosition = Target.position + Offset;
            cameraTransform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);

            transform.LookAt(Target);
        }
    }

    public bool IsTargetWithinBounds()
    { // Bounds check
        return Target.position.x >= xBounds.x && Target.position.x <= xBounds.y &&
               Target.position.y >= yBounds.x && Target.position.y <= yBounds.y &&
               Target.position.z >= zBounds.x && Target.position.z <= zBounds.y;
    }
}
