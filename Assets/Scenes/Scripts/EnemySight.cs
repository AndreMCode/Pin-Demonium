using UnityEngine;

public class EnemySight : MonoBehaviour
{
    [SerializeField] EnemyMovement enemyMovement;
    [SerializeField] EnemyShooter enemyShooter;
    public bool isAlive;
    private bool isFiring;
    private int currentLookX;
    private int currentLookY;
    private int currentLookZ;

    // Raycast
    public float forwardDetectOffset;
    private Vector3 forwardRayOrigin;
    private Vector3 forwardRayDirection;
    private RaycastHit hit;
    private Vector3 raycastDirection;
    private bool isAutoFire;

    // Detection
    public bool checkForPlayer;
    public float playerDetectDistance;
    public float playerDetectHeightOffset;
    public bool checkForEnemy;
    public float enemyDetectDistance;
    public float enemyDetectHeightOffset;
    public bool checkLedges;
    public float ledgeDetectDistance;
    public bool checkWalls;
    public float wallDetectDistance;
    public float wallDetectHeightOffset;

    void Start()
    {
        isAlive = true;
        isFiring = false;
    }

    void FixedUpdate()
    {
        if (isAlive)
        {
            // Set up raycasts
            raycastDirection = new(currentLookX, currentLookY, currentLookZ);
            forwardRayOrigin = transform.position + raycastDirection * forwardDetectOffset;
            forwardRayDirection = raycastDirection;

            int layerMask = ~LayerMask.GetMask("Projectiles", "Enemy");

            if (checkLedges)
            {
                Debug.DrawRay(forwardRayOrigin + Vector3.up, Vector3.down * 100.0f, Color.red);

                if (Physics.Raycast(forwardRayOrigin + Vector3.up, Vector3.down, out hit, 100.0f, layerMask))
                { // Check for ledge
                    if (hit.distance > ledgeDetectDistance)
                    { // Turn around
                        // Debug.Log("Ledge detected");
                        ReverseEnemyMovement();
                    }
                }
            }

            if (checkWalls)
            {
                Debug.DrawRay(forwardRayOrigin + Vector3.up * wallDetectHeightOffset, forwardRayDirection * wallDetectDistance, Color.green);

                if (Physics.Raycast(forwardRayOrigin + Vector3.up * wallDetectHeightOffset, forwardRayDirection, out hit, wallDetectDistance, layerMask))
                { // Check for wall
                    GameObject hitObject = hit.transform.gameObject;

                    if ((hitObject.CompareTag("Wall") || hitObject.CompareTag("Floor"))
                        && hit.distance < wallDetectDistance)
                    { // Turn around
                        // Debug.Log("Wall or Floor detected");
                        ReverseEnemyMovement();
                    }
                }
            }

            if (checkForPlayer)
            {
                Debug.DrawRay(forwardRayOrigin + Vector3.up * playerDetectHeightOffset, forwardRayDirection * playerDetectDistance, Color.blue);

                if (Physics.Raycast(forwardRayOrigin + Vector3.up * playerDetectHeightOffset, forwardRayDirection, out hit, playerDetectDistance, layerMask))
                { // Check for player
                    GameObject hitObject = hit.transform.gameObject;

                    if (!isFiring && !isAutoFire)
                    {
                        if (hitObject.CompareTag("Player") && hit.distance < playerDetectDistance)
                        { // Fire at player
                            // Debug.Log("Player detected");
                            enemyShooter.FireProjectileForward(raycastDirection);
                        }
                    }
                }
            }

            if (checkForEnemy)
            {
                int enemyMask = LayerMask.GetMask("EnemySkin");
                Debug.DrawRay(forwardRayOrigin + Vector3.up * enemyDetectHeightOffset, forwardRayDirection * enemyDetectDistance, Color.yellow);

                if (Physics.Raycast(forwardRayOrigin + Vector3.up * enemyDetectHeightOffset, forwardRayDirection, out hit, enemyDetectDistance, enemyMask))
                { // Check for enemy
                    GameObject hitObject = hit.transform.gameObject;

                    if (hitObject.CompareTag("Enemy") && hit.distance < enemyDetectDistance)
                    { // Turn around
                        // Debug.Log("Enemy detected");
                        ReverseEnemyMovement();
                    }
                }
            }
        }
    }

    private void ReverseEnemyMovement()
    {
        enemyMovement.ReverseEnemyMovement(currentLookX, currentLookY, currentLookZ);
    }

    public void SetIsFiring(bool status)
    {
        isFiring = status;
    }

    public void SetIsAutoFire(bool status)
    {
        isAutoFire = status;
    }

    public void SetEnemyLook(int xDirection, int yDirection, int zDirection)
    {
        currentLookX = xDirection;
        currentLookY = yDirection;
        currentLookZ = zDirection;
    }

    public void SetEnemySight(bool status)
    {
        isAlive = status;
        enemyShooter.SetEnemyShooter(status);
    }
}
