using System.Collections;
using UnityEngine;

public class BGMManager : MonoBehaviour
{
    // Objects with BGM loops attached
    [SerializeField] AudioSource bgm000Loop;
    [SerializeField] AudioSource bgm00Loop;
    [SerializeField] AudioSource bgm01Loop;
    [SerializeField] AudioSource bgm02Loop;

    // BGM reference
    private AudioSource currentTrack;

    // Used to set desired BGM track
    public int currentLevel;
    
    private bool isFading = false;

    void Start()
    {
        // Assign BGM tracks to array
        AudioSource[] bgmTracks = {bgm000Loop, bgm00Loop, bgm01Loop, bgm02Loop};

        // Custom BGM track start point
        float startTime = currentLevel < 1 ? 0 : 20.600f;

        // Choose which BGM track to play dependent on level
        int index = currentLevel;
        bgmTracks[index].time = startTime;

        // Play BGM and set reference for fading
        bgmTracks[index].Play();
        currentTrack = bgmTracks[index];
    }

    public void OnPlayerWin()
    { // Long fade on win
        if (!isFading)
        {
            StartCoroutine(FadeOutTracks(3.5f));
        }
    }

    public void OnPlayerLose()
    { // Short fade on fail
        if (!isFading)
        {
            StartCoroutine(FadeOutTracks(0.5f));
        }
    }

    public void OnPlayerReset()
    { // Short fade on reset
        if (!isFading)
        {
            StartCoroutine(FadeOutTracks(1.0f));
        }
    }

    private IEnumerator FadeOutTracks(float fadeDuration)
    { // Fade function
        isFading = true;

        float startVolume = currentTrack.volume;
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float newVolume = Mathf.Lerp(startVolume, 0, elapsedTime / fadeDuration);

            currentTrack.volume = newVolume;

            yield return null;
        }

        currentTrack.Stop();

        isFading = false;
    }
}
