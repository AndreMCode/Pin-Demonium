using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class LevelTransition : MonoBehaviour
{
    [SerializeField] private string sceneToLoad;
    [SerializeField] private string sceneToUnload;
    [SerializeField] private string playerTag;

    private bool hasTriggered = false;

    private void OnTriggerEnter(Collider other)
    { // Transition logic
        if (hasTriggered) return;
        if (!other.CompareTag(playerTag)) return;

        hasTriggered = true;

        Messenger.Broadcast(GameEvent.LEVEL_COMPLETE_TXT);
        Messenger.Broadcast(GameEvent.CARRY_OVER_SCORE);

        // Set player attributes
        PlayerMovement movement = other.gameObject.GetComponent<PlayerMovement>();
        movement.FreezeAnimatorBody();
        movement.enabled = false;

        if (sceneToLoad != "Scene00")
        {
            StartCoroutine(GetReadyText());
        }
        StartCoroutine(NextLevelTimer());

        BGMManager bgm = FindFirstObjectByType<BGMManager>();
        bgm.OnPlayerWin();
    }

    private void LoadNextLevel()
    {
        // Load the next scene additively
        SceneManager.LoadSceneAsync(sceneToLoad, LoadSceneMode.Additive).completed += (op) =>
        {
            // Unload the previous scene once the new one is loaded
            SceneManager.UnloadSceneAsync(sceneToUnload);
        };
    }

    private IEnumerator GetReadyText()
    { // Next level visual cue
        yield return new WaitForSeconds(3.0f);

        for (int i = 0; i < 3; i++)
        {
            Messenger.Broadcast(GameEvent.GET_READY_TXT);
            yield return new WaitForSeconds(0.4f);
            Messenger.Broadcast(GameEvent.GET_READY_TXT);
            yield return new WaitForSeconds(0.1f);
        }

        Messenger.Broadcast(GameEvent.GET_READY_TXT);
    }

    private IEnumerator NextLevelTimer()
    {
        yield return new WaitForSeconds(5.0f);

        LoadNextLevel();
    }
}
