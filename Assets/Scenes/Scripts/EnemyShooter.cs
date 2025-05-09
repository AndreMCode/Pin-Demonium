using System.Collections;
using UnityEngine;

public class EnemyShooter : MonoBehaviour
{
    [SerializeField] EnemySight enemySight;
    [SerializeField] GameObject enemyProjectile;

    // Audio
    [SerializeField] AudioSource soundSource;
    [SerializeField] AudioClip projectileLaunchSFX;
    public float projectileLaunchSFXPitch;
    public float projectileLaunchSFXVolume;

    // Projectile
    private GameObject projectile;
    public float projectileDamage;
    public float projectileSpeed;
    public float projectileHeightOffset;
    public float projectileForwardOffset;
    private float fireTime;
    public float fireCooldown;
    private bool firing;

    // Auto fire
    private Vector3 autoFireDirection;
    public bool autoFire;

    // Burst fire
    public bool burstFire;
    public int burstCount;
    public float burstInterval;

    private bool isAlive;

    void Start()
    {
        isAlive = true;
        firing = false;

        if (autoFire)
        {
            enemySight.SetIsAutoFire(autoFire);
        }
    }

    void Update()
    {
        if (isAlive)
        {
            if (firing && Time.time >= fireTime)
            {
                firing = false;
                enemySight.SetIsFiring(firing);
            }

            if (!firing && autoFire)
            {
                FireProjectileForward(autoFireDirection);
            }
        }
    }

    public void FireProjectileForward(Vector3 direction)
    {
        fireTime = Time.time + fireCooldown;
        firing = true;
        enemySight.SetIsFiring(firing);
        soundSource.pitch = projectileLaunchSFXPitch;

        if (burstFire)
        {
            StartCoroutine(BurstFireForward(direction));
        }
        else
        {
            soundSource.PlayOneShot(projectileLaunchSFX, projectileLaunchSFXVolume);

            // Instantiate and rotate projectile
            projectile = Instantiate(enemyProjectile);
            EnemyProjectile projectileAttributes = projectile.GetComponent<EnemyProjectile>();
            projectileAttributes.SetProjectileDamage(projectileDamage);
            projectileAttributes.SetProjectileSpeed(projectileSpeed);

            Vector3 spawnPosition = transform.position
                              + direction.normalized * projectileForwardOffset
                              + Vector3.up * projectileHeightOffset;

            projectile.transform.SetPositionAndRotation(spawnPosition, Quaternion.LookRotation(direction));
        }
    }

    public void SetForwardDirection(int xDirection, int yDirection, int zDirection)
    {
        autoFireDirection = new Vector3(xDirection, yDirection, zDirection);
    }

    public void SetEnemyShooter(bool status)
    {
        isAlive = status;
    }

    private IEnumerator BurstFireForward(Vector3 direction)
    {
        for (int i = 0; i < burstCount; i++)
        {
            soundSource.PlayOneShot(projectileLaunchSFX, projectileLaunchSFXVolume);

            // Instantiate and rotate projectile
            projectile = Instantiate(enemyProjectile);
            EnemyProjectile projectileAttributes = projectile.GetComponent<EnemyProjectile>();
            projectileAttributes.SetProjectileDamage(projectileDamage);
            projectileAttributes.SetProjectileSpeed(projectileSpeed);

            Vector3 spawnPosition = transform.position
                              + direction.normalized * projectileForwardOffset
                              + Vector3.up * projectileHeightOffset;

            projectile.transform.SetPositionAndRotation(spawnPosition, Quaternion.LookRotation(direction));

            yield return new WaitForSeconds(burstInterval);
        }

        yield return null;
    }
}
