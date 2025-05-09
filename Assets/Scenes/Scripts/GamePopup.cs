using TMPro;
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GamePopup : MonoBehaviour
{
    [SerializeField] TMP_Text highScoreTXT;
    [SerializeField] BGMManager bgm;

    public void Open()
    {
        gameObject.SetActive(true);

        // Unlock the cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        highScoreTXT.text = PlayerPrefs.GetInt("highScore", 0).ToString();
    }

    public void Close()
    {
        // Lock the cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        gameObject.SetActive(false);
    }

    public void OnNewGameButtonClick()
    {
        // Lock the cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        bgm.OnPlayerReset();

        StartCoroutine(NextLevelTimer());
    }

    public void OnQuitButtonPress()
    {
        Debug.Log("Player quit.");
        Application.Quit();
    }

    private void LoadNextLevel()
    {
        Scene currentScene = SceneManager.GetActiveScene();

        // Load the next scene additively
        SceneManager.LoadSceneAsync("Scene00", LoadSceneMode.Additive).completed += (op) =>
        {
            // Unload the previous scene once the new one is loaded
            SceneManager.UnloadSceneAsync(currentScene);
        };
    }

    private IEnumerator NextLevelTimer()
    {
        yield return new WaitForSeconds(1.0f);

        LoadNextLevel();
    }
}
