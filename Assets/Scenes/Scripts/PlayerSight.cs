using UnityEngine;

public class PlayerSight : MonoBehaviour
{
    // Raycast
    [SerializeField] PlayerMovement movement;
    private RaycastHit hit;
    private Vector3 currentDirection;
    private Vector3 forwardRayOrigin;
    private Vector3 forwardRayDirection;
    public float forwardDetectOffset;
    public float wallDetectDistance;
    public float wallDetectHeightOffset;
    public bool canWallJump;
    public bool isTouchingWall;

    void Start()
    {
        currentDirection = Vector3.right;
        isTouchingWall = false;
    }

    void FixedUpdate()
    {
        // Set up raycasts
        forwardRayOrigin = transform.position + currentDirection * forwardDetectOffset;
        forwardRayDirection = currentDirection;

        int layerMask = ~LayerMask.GetMask("Projectiles", "Enemy");

        if (canWallJump)
        {
            Debug.DrawRay(forwardRayOrigin + Vector3.up * wallDetectHeightOffset, forwardRayDirection * wallDetectDistance, Color.green);

            if (Physics.Raycast(forwardRayOrigin + Vector3.up * wallDetectHeightOffset, forwardRayDirection, out hit, wallDetectDistance, layerMask))
            { // Check for wall
                GameObject hitObject = hit.transform.gameObject;

                if (hitObject.CompareTag("Wall") || hitObject.CompareTag("Floor"))
                {
                    // Debug.Log("Wall detected, update movement script.");
                    isTouchingWall = true;
                    movement.SetIsTouchingWall(isTouchingWall);
                }
            }
            else
            {
                // Debug.Log("Stopped touching wall.");
                isTouchingWall = false;
                movement.SetIsTouchingWall(isTouchingWall);
            }
        }
    }

    public void UpdateDirection(Vector3 directionUpdate)
    {
        if (isTouchingWall)
        {
            isTouchingWall = false;
        }
        // Debug.Log("Playersight script direction updated: (" + directionUpdate.x + ", " + directionUpdate.y + ", " + directionUpdate.z + ")");
        currentDirection = directionUpdate;
    }

    public void SetIsTouchingWall(bool update)
    {
        isTouchingWall = update;
    }
}
