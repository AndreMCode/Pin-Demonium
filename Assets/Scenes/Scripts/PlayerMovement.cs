using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // Bodies, components, effects
    [SerializeField] CharacterController body;
    [SerializeField] PlayerSight sight;
    [SerializeField] ParticleSystem dashSparks;
    [SerializeField] ParticleSystem dashDust;
    [SerializeField] Animator animator;
    public float dashDustParticleHeightOffset;

    // Audio
    [SerializeField] AudioSource dashSFXAudioSource;
    [SerializeField] AudioClip dashSFXClip;
    public float dashSFXVolume;
    public float dashSFXPitch;

    // Environment
    private Vector3 velocity;
    public float gravity;

    // Movement, jump
    private Vector3 moveDirection;
    public bool moveable; // deprecated **
    public bool grounded;
    public float moveSpeed;
    private float movement;
    private float lastMovement;
    public float jumpStrength;
    public float jumpCancelForce;
    public bool canCancelJump;

    // Dash
    public bool dashing;
    public bool dashJumping;
    public float dashSpeed;
    public float dashTime;
    public float lastDashTime;
    public float dashDuration;
    public float dashCooldown;
    private float dashDirection;

    // Wall cling, wall-jump
    public bool isWallAttached;
    public bool isWallJumping;
    public float wallSlideSpeed;
    public float wallJumpTime;
    public float wallJumpCooldown;
    private bool cancelledWallJump;
    private Vector3 wallJumpDirection;

    void Start()
    {
        moveDirection = Vector3.zero;
        moveable = true;
        grounded = true;
        dashing = false;
        dashJumping = false;
        canCancelJump = false;
        isWallAttached = false;
        isWallJumping = false;
        cancelledWallJump = true;
        dashSparks.Stop();
        dashDust.Stop();
    }

    void Update()
    {
        HandleGroundedState();

        HandleWallAttachment();

        HandleMovementInput();

        HandleDirectionChanges();

        HandleUIInput();

        HandleJumpInput();

        HandleDash();

        HandleWallJump();

        ApplyHorizontalMovement();

        ApplyGravity();

        HandleAnimation();
    }

    void HandleGroundedState()
    {
        grounded = body.isGrounded;

        if (grounded && dashJumping)
        {
            dashJumping = false;
            dashDust.Stop();
        }

        if (grounded && isWallJumping)
        {
            isWallJumping = false;
        }
    }

    void HandleWallAttachment()
    {
        if (isWallAttached && (grounded || movement == 0 || movement == -lastMovement))
        {
            isWallAttached = false;
            ReturnIsTouchingWall(isWallAttached);
        }
    }

    void HandleMovementInput()
    {
        movement = Input.GetAxisRaw("Horizontal");

        if (Input.GetKey(KeyCode.RightArrow)) movement = 1;
        if (Input.GetKey(KeyCode.LeftArrow)) movement = -1;

        moveDirection = new Vector3(movement, 0, 0).normalized;
    }

    void HandleDirectionChanges()
    {
        if (movement != 0 && lastMovement == -movement)
        {
            // Send direction out for raycast handler
            UpdateSight(moveDirection);

            // Reset wall jump flag
            cancelledWallJump = true;
        }
        if (movement != 0)
        {
            lastMovement = movement;
        }
    }

    void HandleUIInput()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            Messenger.Broadcast(GameEvent.TOGGLE_GAME_POPUP);
        }
    }

    void HandleJumpInput()
    {
        if (grounded && (Input.GetKeyDown(KeyCode.X) || Input.GetKeyDown(KeyCode.JoystickButton0)))
        {
            velocity.y = Mathf.Sqrt(2 * jumpStrength * gravity);
            canCancelJump = true;
        }

        if (canCancelJump && velocity.y < 0)
        {
            canCancelJump = false;
            isWallJumping = false;
        }

        if (!grounded && canCancelJump && (Input.GetKeyUp(KeyCode.X) || Input.GetKeyUp(KeyCode.JoystickButton0)))
        {
            velocity.y *= 1f / jumpCancelForce;
            canCancelJump = false;
            isWallJumping = false;
        }

        if (dashing)
        {
            if (grounded && (Input.GetKeyDown(KeyCode.X) || Input.GetKeyDown(KeyCode.JoystickButton0)))
            {
                dashing = false;
                dashJumping = true;
                dashSparks.Stop();
                velocity.y = Mathf.Sqrt(2 * jumpStrength * gravity);
                canCancelJump = true;
            }

            // Cancel if release C or timeout
            if (Time.time >= dashTime || Input.GetKeyUp(KeyCode.A)
                || Input.GetKeyUp(KeyCode.JoystickButton5))
            {
                dashing = false;
                dashSparks.Stop();
                dashDust.Stop();
            }
        }
    }

    void HandleDash()
    {
        if (grounded && !dashing && !dashJumping)
        {
            if ((Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.JoystickButton5))
                && Time.time > lastDashTime + dashCooldown && movement != 0)
            {
                dashing = true;
                dashTime = Time.time + dashDuration;
                dashDirection = movement > 0 ? 1 : -1;

                dashSFXAudioSource.pitch = dashSFXPitch;
                dashSFXAudioSource.PlayOneShot(dashSFXClip, dashSFXVolume);
                dashSparks.Play();
                dashDust.Play();

                lastDashTime = Time.time;
            }
        }
    }

    void HandleWallJump()
    {
        if (isWallAttached && (Input.GetKeyDown(KeyCode.X) || Input.GetKeyDown(KeyCode.JoystickButton0)))
        {
            isWallJumping = true;
            wallJumpDirection = movement > 0 ? Vector3.left : Vector3.right;
            wallJumpTime = Time.time + wallJumpCooldown;

            velocity.y = Mathf.Sqrt(2 * (jumpStrength * 0.5f) * (gravity * 1.5f));
            isWallAttached = false;
            cancelledWallJump = false;
            ReturnIsTouchingWall(isWallAttached);
        }
    }

    void ApplyHorizontalMovement()
    {
        if (dashJumping)
        { // Move at dash speed
            body.Move(dashSpeed * Time.deltaTime * moveDirection);
        }
        else if (isWallJumping && !cancelledWallJump && Time.time <= wallJumpTime)
        { // Lerp from moveSpeed to zero unless wall jump has been cancelled by changing direction
            float wallJumpProgress = 1f - ((wallJumpTime - Time.time) / wallJumpCooldown);
            float currentSpeed = Mathf.Lerp(moveSpeed, 2f, wallJumpProgress);
            body.Move(currentSpeed * Time.deltaTime * wallJumpDirection);
        }
        else if (isWallJumping)
        { // Move at boosted speed
            body.Move((moveSpeed + 5f) * Time.deltaTime * moveDirection);
        }
        else if (!dashing)
        { // Apply normal movement speed
            body.Move(moveSpeed * Time.deltaTime * moveDirection);
        }
    }

    void ApplyGravity()
    {
        if (dashing)
        {
            // Allow changing directions while dashing
            dashDirection = movement;

            body.Move(new Vector3(dashDirection * dashSpeed * Time.deltaTime, 0, 0));
        }
        else if (isWallAttached)
        {
            velocity.y = -wallSlideSpeed;
        }
        else
        {
            // Apply gravity if not dashing or attached to wall
            if (moveable)
            {
                if (!grounded)
                {
                    velocity.y -= gravity * Time.deltaTime;
                }
                else if (velocity.y < 0)
                {
                    velocity.y = -2f;
                }
            }
        }

        // Apply vertical movement
        body.Move(velocity * Time.deltaTime);
    }

    void HandleAnimation()
    {
        if (movement > 0)
            animator.transform.localRotation = Quaternion.Euler(0, 90, 0); // Facing right
        else if (movement < 0)
            animator.transform.localRotation = Quaternion.Euler(0, -90, 0); // Facing left

        // Animator parameters
        animator.SetFloat("speed", Mathf.Abs(movement)); // Use abs so it works in both directions
        animator.SetBool("jumping", !grounded && velocity.y > 0);
    }

    private void UpdateSight(Vector3 directionUpdate)
    {
        sight.UpdateDirection(directionUpdate);
    }

    private void ReturnIsTouchingWall(bool update)
    {
        // Debug.Log("Detached from wall.");
        sight.SetIsTouchingWall(update);
    }

    public void FreezeAnimatorBody()
    {
        animator.SetBool("jumping", false);
        animator.SetFloat("speed", 0);
        dashSparks.Stop();
    }

    public void SetIsTouchingWall(bool update)
    {
        // Debug.Log("recv'd wall detect.");
        if (update && movement != 0 && !body.isGrounded && velocity.y < 0)
        {
            if (dashJumping)
            {
                dashJumping = false;
            }
            // Debug.Log("Attached to wall");
            isWallAttached = update;
        }
        else
        { // Continue checking
            isWallAttached = false;
            sight.SetIsTouchingWall(isWallAttached);
        }
    }

    void LateUpdate()
    { // Lock player to z-axis
        Vector3 position = transform.position;
        position.z = 0;
        transform.position = position;
    }
}
