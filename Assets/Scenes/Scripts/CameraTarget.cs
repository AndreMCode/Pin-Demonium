using UnityEngine;

public class CameraTarget : MonoBehaviour
{
    public Transform player; // The player entity
    public Vector3 boundarySize = new(7f, 5f, 1f); // Size of the boundary relative to the player

    void Update()
    {
        // Calculate the boundaries relative to the player
        Vector3 minBoundary = player.position - boundarySize / 2;
        Vector3 maxBoundary = player.position + boundarySize / 2;

        // Check if the sphere is outside the boundaries
        Vector3 newPosition = transform.position;

        if (transform.position.x < minBoundary.x)
            newPosition.x = minBoundary.x;
        if (transform.position.x > maxBoundary.x)
            newPosition.x = maxBoundary.x;
        if (transform.position.y < minBoundary.y)
            newPosition.y = minBoundary.y;
        if (transform.position.y > maxBoundary.y)
            newPosition.y = maxBoundary.y;
        if (transform.position.z < minBoundary.z)
            newPosition.z = minBoundary.z;
        if (transform.position.z > maxBoundary.z)
            newPosition.z = maxBoundary.z;

        // Update the sphere's position if it moved
        if (newPosition != transform.position)
        {
            transform.position = newPosition;
        }
    }
}
