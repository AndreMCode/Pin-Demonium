using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField] EnemySight enemySight;
    [SerializeField] EnemyShooter enemyShooter;
    [SerializeField] Rigidbody body;
    private Animator animator;
    public bool isAlive;
    public bool gamePaused;
    public bool isAnimated;
    public float moveSpeedX;
    public int startDirectionX;
    public bool movementY;
    public float moveSpeedY;
    public int startDirectionY;
    public float moveSpeedZ;
    public int startDirectionZ;

    void Start()
    {
        if (isAnimated)
        { // Collect component if animated type
            animator = GetComponentInChildren<Animator>();
        }

        // Notify relevant scripts of direction
        SetEnemyLook(startDirectionX, startDirectionY, startDirectionZ);

        // Apply magnitude
        moveSpeedX *= startDirectionX;
        moveSpeedY *= startDirectionY;
        moveSpeedZ *= startDirectionZ;

        isAlive = true;
        gamePaused = false;
    }

    void FixedUpdate()
    {
        if (isAlive && !gamePaused)
        { // Movement logic
            if (movementY)
            {
                body.linearVelocity = new Vector3(moveSpeedX, moveSpeedY, moveSpeedZ);
            }
            else
            {
                body.linearVelocity = new Vector3(moveSpeedX, body.linearVelocity.y, moveSpeedZ);
            }

            if (isAnimated) // Animate if using animator
            { animator.SetFloat("speed", Mathf.Abs(moveSpeedX)); }
        }
    }

    private void SetEnemyLook(int xDirection, int yDirection, int zDirection)
    {
        enemySight.SetEnemyLook(xDirection, yDirection, zDirection);
        enemyShooter.SetForwardDirection(xDirection, yDirection, zDirection);

        if (isAnimated)
        { // Calculate desired look direction for animated bodies
            Vector3 lookDirection = new(xDirection, yDirection, zDirection);

            // Only rotate if there's a valid direction
            if (lookDirection != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
                animator.transform.localRotation = targetRotation;
            }
        }
    }

    public void SetEnemyDirection(int xDirection, int yDirection, int zDirection)
    { // future use, manipulate movement
        moveSpeedX *= xDirection;
        moveSpeedY *= yDirection;
        moveSpeedZ *= zDirection;

        SetEnemyLook(xDirection, yDirection, zDirection);
    }

    public void ReverseEnemyMovement(int xDirection, int yDirection, int zDirection)
    {
        moveSpeedX *= -1;
        moveSpeedY *= -1;
        moveSpeedZ *= -1;

        SetEnemyLook(-xDirection, -yDirection, -zDirection);
    }

    public void SetEnemyAlive(bool status)
    {
        isAlive = status;
    }

    public void FreezeAnimatorBody()
    {
        if (isAnimated)
        {
            animator.SetBool("jumping", false);
            animator.SetFloat("speed", 0);
        }
    }
}
