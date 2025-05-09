using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ReactivePlayer : MonoBehaviour
{
    // Audio
    [SerializeField] AudioSource soundSource;
    [SerializeField] AudioClip playerHitSFX;
    [SerializeField] AudioClip playerDeathClip;
    public float playerHitSFXVolume;
    public float playerHitSFXPitch;
    public float PlayerDeathClipVolume;

    [SerializeField] PhysicsMaterial bounciness;
    [SerializeField] SkinnedMeshRenderer playerRenderer;
    public float baseHealth;
    public float health;
    public float hitCooldown;
    public float flickerInterval;
    public bool invincible;
    public bool isAlive;

    void Start()
    {
        health = baseHealth;
        isAlive = true;
        invincible = false;

        Messenger<float>.Broadcast("PLAYER_HEALTH_UPDATE", health);
    }

    void Update()
    {
        if (isAlive && health <= 0)
        { // Player defeated logic
            isAlive = false;

            // Update if new highest score
            Messenger.Broadcast(GameEvent.CARRY_OVER_SCORE);

            // Disable controls
            PlayerMovement movement = GetComponent<PlayerMovement>();
            movement.FreezeAnimatorBody();
            PlayerShooter shooter = GetComponent<PlayerShooter>();
            movement.enabled = false;
            shooter.enabled = false;

            // Add a Rigidbody
            if (!TryGetComponent(out Rigidbody body))
            {
                body = gameObject.AddComponent<Rigidbody>();
            }

            // Add a capsule collider
            var collider = gameObject.AddComponent<BoxCollider>();
            collider.size = Vector3.one * 0.75f;
            collider.material = bounciness;

            // Disable CharacterController
            var cc = GetComponent<CharacterController>();
            if (cc != null) cc.enabled = false;

            // Set rigidbody constraints
            body.useGravity = true;
            body.constraints = RigidbodyConstraints.None;
            body.centerOfMass = Vector3.forward * 0.25f;

            // Apply knockback from projectile position
            body.AddExplosionForce(200.0f,
            transform.position + Vector3.forward * 0.6f + Vector3.right * Random.Range(-0.4f, 0.4f), 2.0f);

            Messenger.Broadcast(GameEvent.GAME_OVER_TXT);

            BGMManager bgm = FindFirstObjectByType<BGMManager>();
            bgm.OnPlayerLose();
            soundSource.pitch = 1.0f;
            soundSource.PlayOneShot(playerDeathClip, PlayerDeathClipVolume);

            StartCoroutine(HealthDepleted());
        }

        // TEMPORARY: For wall-jump escapees while this project is still in demo stage
        if (isAlive && transform.position.y < -1.0f)
        {
            health = 0;
            Messenger<float>.Broadcast("PLAYER_HEALTH_UPDATE", health);
        }
    }

    public void ReactToHit(float damage)
    {
        if (!invincible)
        {
            soundSource.pitch = Random.Range(playerHitSFXPitch - 0.1f, playerHitSFXPitch + 0.1f);
            soundSource.PlayOneShot(playerHitSFX, playerHitSFXVolume);

            health -= damage;
            Messenger<float>.Broadcast("PLAYER_HEALTH_UPDATE", health);

            // Debug.Log("Player Received " + damage + " damage");
            StartCoroutine(PlayerFlicker());
        }
    }

    public void ReactToRecovery(float value)
    {
        health += value;
        health = Mathf.Clamp(health, 0, baseHealth);

        Messenger<float>.Broadcast("PLAYER_HEALTH_UPDATE", health);
    }

    private void ResetGame()
    {
        // Unlock the cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        
        Scene currentScene = SceneManager.GetActiveScene();

        // Load the base scene additively
        SceneManager.LoadSceneAsync("Scene00", LoadSceneMode.Additive).completed += (op) =>
        {
            // Unload the previous scene once the new one is loaded
            SceneManager.UnloadSceneAsync(currentScene);
        };
    }

    private IEnumerator PlayerFlicker()
    {
        invincible = true;
        float elapsed = 0f;

        while (elapsed < hitCooldown)
        {
            playerRenderer.enabled = false;
            yield return new WaitForSeconds(flickerInterval);
            playerRenderer.enabled = true;
            yield return new WaitForSeconds(flickerInterval);
            elapsed += flickerInterval * 2;
        }

        invincible = false;
    }

    private IEnumerator HealthDepleted()
    { // Wait and reset
        yield return new WaitForSeconds(4.0f);

        playerRenderer.enabled = false; // move down if implemented cars on the road

        yield return new WaitForSeconds(1.0f);

        ResetGame();
    }
}
