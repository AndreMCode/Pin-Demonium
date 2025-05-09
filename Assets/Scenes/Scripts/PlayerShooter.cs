using UnityEngine;

public class PlayerShooter : MonoBehaviour
{
    [SerializeField] GameObject playerProjectile;
    [SerializeField] Animator animator;
    private GameObject projectile;
    private Vector3 projectileDirection;
    public float projectileForwardOffset;
    public float projectileHeightOffset;

    // Audio
    [SerializeField] AudioSource soundSource;
    [SerializeField] AudioClip projectileLaunchSFX;
    public float projectileLaunchSFXVolume;
    public float projectileLaunchSFXPitch;

    private float fireTime;
    public float fireCooldown;
    private bool firing;

    void Start()
    {
        projectileDirection = Vector3.right;
    }

    void Update()
    {
        float horizontalDirection = Input.GetAxisRaw("Horizontal");
        if (horizontalDirection != 0)
        {
            projectileDirection = horizontalDirection > 0 ? Vector3.right : Vector3.left;
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            projectileDirection = Vector3.right;
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            projectileDirection = Vector3.left;
        }

        float perpendicularDirection = Input.GetAxisRaw("Vertical");
        if (Input.GetKey(KeyCode.UpArrow))
        {
            perpendicularDirection = 1;
        }
        if (perpendicularDirection > 0)
        { // Vertical overrides horizontal so that the player can strafe and fire projectiles
            projectileDirection = Vector3.forward;

            animator.transform.localRotation = Quaternion.Euler(0, 0, 0);
        }

        if (firing && Time.time >= fireTime)
        {
            firing = false;
        }

        if (!firing)
        {
            if (Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.JoystickButton2))
            {
                FireProjectile();
            }
        }
    }

    private void FireProjectile()
    {
        fireTime = Time.time + fireCooldown;
        firing = true;

        soundSource.pitch = projectileLaunchSFXPitch;
        soundSource.PlayOneShot(projectileLaunchSFX, projectileLaunchSFXVolume);

        // Instantiate and rotate projectile
        projectile = Instantiate(playerProjectile);

        Vector3 spawnPosition = transform.position
                          + projectileDirection * projectileForwardOffset
                          + Vector3.up * projectileHeightOffset;

        projectile.transform.SetPositionAndRotation(spawnPosition, Quaternion.LookRotation(projectileDirection));
        PlayerProjectile projectileLook = projectile.GetComponent<PlayerProjectile>();
        projectileLook.SetDirection(projectileDirection);
    }
}
