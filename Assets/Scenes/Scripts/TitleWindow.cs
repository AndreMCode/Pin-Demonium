using TMPro;
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class TitleWindow : MonoBehaviour
{
    [SerializeField] TMP_Text highScoreTXT;
    [SerializeField] TMP_Text getReadyTXT;
    [SerializeField] BGMManager bgm;

    void Start()
    {
        // Unlock the cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        
        getReadyTXT.enabled = false;
        highScoreTXT.text = PlayerPrefs.GetInt("highScore", 0).ToString();
    }

    public void OnStartButtonClick()
    {
        // Lock the cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        PlayerPrefs.SetInt("runningScore", 0);

        bgm.OnPlayerWin();

        StartCoroutine(GetReadyText());
        StartCoroutine(NextLevelTimer());
    }

    public void OnQuitButtonPress()
    {
        Debug.Log("Player quit.");
        Application.Quit();
    }

    public void OnResetScoreButtonPress()
    {
        PlayerPrefs.SetInt("highScore", 0);
        highScoreTXT.text = PlayerPrefs.GetInt("highScore", 0).ToString();
    }

    private void LoadNextLevel()
    {
        // Load the next scene additively
        SceneManager.LoadSceneAsync("Scene01", LoadSceneMode.Additive).completed += (op) =>
        {
            // Unload the previous scene once the new one is loaded
            SceneManager.UnloadSceneAsync("Scene00");
        };
    }

    private IEnumerator GetReadyText()
    { // Next level visual cue
        for (int i = 0; i < 6; i++)
        {
            getReadyTXT.enabled = true;
            yield return new WaitForSeconds(0.4f);
            getReadyTXT.enabled = false;
            yield return new WaitForSeconds(0.1f);
        }

        getReadyTXT.enabled = true;
    }

    private IEnumerator NextLevelTimer()
    {
        yield return new WaitForSeconds(3.5f);

        LoadNextLevel();
    }
}
