using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    private bool _isLoading;

    private void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;
    private void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode) => _isLoading = false;

    public void RestartScene(float delay = 1f) => LoadSceneWithDelay(SceneManager.GetActiveScene().name, delay);

    public void LoadScene(string sceneName)
    {
        if (_isLoading) return;
        _isLoading = true;
        SceneManager.LoadScene(sceneName);
    }

    public void LoadSceneWithDelay(string sceneName, float delay = 1f)
    {
        if (_isLoading) return;
        StartCoroutine(LoadSceneCoroutine(sceneName, delay));
    }

    public void ExitGame(float delay = 1f)
    {
        if (_isLoading) return;
        StartCoroutine(ExitGameCoroutine(delay));
    }

    private IEnumerator LoadSceneCoroutine(string sceneName, float delay)
    {
        _isLoading = true;
        yield return new WaitForSeconds(delay);

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        while (!asyncLoad.isDone)
            yield return null;

        _isLoading = false;
    }

    private IEnumerator ExitGameCoroutine(float delay)
    {
        _isLoading = true;
        yield return new WaitForSeconds(delay);
        Application.Quit();
    }
}
