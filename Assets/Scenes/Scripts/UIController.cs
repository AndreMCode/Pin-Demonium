using TMPro;
using UnityEngine;

public class UIController : MonoBehaviour
{
    // Audio
    [SerializeField] AudioSource soundSource;
    [SerializeField] AudioClip playerLowHealthSFX;
    public float playerLowHealthSFXVolume;
    public float playerLowHealthSFXPitch;

    [SerializeField] GamePopup gamePopup;
    [SerializeField] TMP_Text scoreCounter;
    [SerializeField] TMP_Text levelCompleteTXT;
    [SerializeField] TMP_Text getReadyTXT;
    [SerializeField] TMP_Text gameOverTXT;
    public UnityEngine.UI.Image[] hearts;
    private readonly int healthPerHeart = 10;
    private readonly int maxHealth = 50;
    private int currentHealth;
    private int score;

    void Start()
    {
        gamePopup.Close();
        levelCompleteTXT.enabled = false;
        getReadyTXT.enabled = false;
        gameOverTXT.enabled = false;

        currentHealth = maxHealth;
        UpdateHearts();

        scoreCounter.text = PlayerPrefs.GetInt("runningScore", 0).ToString();
        score = PlayerPrefs.GetInt("runningScore", 0);
    }

    void OnEnable()
    {
        Messenger.AddListener(GameEvent.TOGGLE_GAME_POPUP, ToggleGamePopup);
        Messenger<float>.AddListener(GameEvent.PLAYER_HEALTH_UPDATE, PlayerHealthUpdate);
        Messenger<int>.AddListener(GameEvent.PLAYER_SCORE_UPDATE, PlayerScoreUpdate);
        Messenger.AddListener(GameEvent.CARRY_OVER_SCORE, PlayerScoreLevelCarryover);
        Messenger.AddListener(GameEvent.LEVEL_COMPLETE_TXT, LevelCompleteTXT);
        Messenger.AddListener(GameEvent.GET_READY_TXT, GetReadyTXT);
        Messenger.AddListener(GameEvent.GAME_OVER_TXT, GameOverTXT);
    }

    void OnDisable()
    {
        Messenger.RemoveListener(GameEvent.TOGGLE_GAME_POPUP, ToggleGamePopup);
        Messenger<float>.RemoveListener(GameEvent.PLAYER_HEALTH_UPDATE, PlayerHealthUpdate);
        Messenger<int>.RemoveListener(GameEvent.PLAYER_SCORE_UPDATE, PlayerScoreUpdate);
        Messenger.RemoveListener(GameEvent.CARRY_OVER_SCORE, PlayerScoreLevelCarryover);
        Messenger.RemoveListener(GameEvent.LEVEL_COMPLETE_TXT, LevelCompleteTXT);
        Messenger.RemoveListener(GameEvent.GET_READY_TXT, GetReadyTXT);
        Messenger.RemoveListener(GameEvent.GAME_OVER_TXT, GameOverTXT);
    }

    private void UpdateHearts()
    {
        int filledHearts = currentHealth / healthPerHeart;

        for (int i = 0; i < hearts.Length; i++)
        {
            if (i < filledHearts)
            {
                hearts[i].color = new Color(1f, 0f, 0f, 1f);
            }
            else
            {
                hearts[i].color = new Color(1f, 1f, 1f, 1f);
            }
        }
    }

    void PlayerHealthUpdate(float value)
    {
        currentHealth = Mathf.Clamp((int)value, 0, maxHealth);
        UpdateHearts();

        if (currentHealth == 20)
        {
            soundSource.pitch = playerLowHealthSFXPitch;
            soundSource.PlayOneShot(playerLowHealthSFX, playerLowHealthSFXVolume);
        }

        if (currentHealth == 10)
        {
            soundSource.pitch = playerLowHealthSFXPitch + 0.1f;
            soundSource.PlayOneShot(playerLowHealthSFX, playerLowHealthSFXVolume + 0.1f);
        }
    }

    void PlayerScoreUpdate(int value)
    {
        score += value;
        scoreCounter.text = score.ToString();
    }

    public void ToggleGamePopup()
    {
        if (gamePopup.isActiveAndEnabled)
        {
            gamePopup.Close();
        }
        else
        {
            gamePopup.Open();
        }
    }

    public void PlayerScoreLevelCarryover()
    {
        PlayerPrefs.SetInt("runningScore", score);

        if (score > PlayerPrefs.GetInt("highScore"))
        {
            PlayerPrefs.SetInt("highScore", score);
        }
    }

    private void LevelCompleteTXT()
    {
        if (levelCompleteTXT.enabled)
        {
            levelCompleteTXT.enabled = false;
        }
        else
        {
            levelCompleteTXT.enabled = true;
        }
    }

    private void GetReadyTXT()
    {
        if (getReadyTXT.enabled)
        {
            getReadyTXT.enabled = false;
        }
        else
        {
            getReadyTXT.enabled = true;
        }
    }

    private void GameOverTXT()
    {
        if (gameOverTXT.enabled)
        {
            gameOverTXT.enabled = false;
        }
        else
        {
            gameOverTXT.enabled = true;
        }
    }
}
