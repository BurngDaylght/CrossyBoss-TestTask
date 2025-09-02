using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    private bool _isLoading = false;

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        _isLoading = false;
    }

    public void RestartSceneWithDelay(float delay)
    {
        if (_isLoading) return;
        StartCoroutine(LoadSceneCoroutine(SceneManager.GetActiveScene().name, delay));
    }

    public void LoadScene(string sceneName)
    {
        if (_isLoading) return;
        _isLoading = true;
        SceneManager.LoadScene(sceneName);
    }

    public void LoadSceneWithDelay(string sceneName, float delay)
    {
        if (_isLoading) return;
        StartCoroutine(LoadSceneCoroutine(sceneName, delay));
    }

    private IEnumerator LoadSceneCoroutine(string sceneName, float delay = 1f)
    {
        if (_isLoading) yield break;
        _isLoading = true;

        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(sceneName);
    }
    
    public void ExitGameWithDelay(float delay)
    {
        if (_isLoading) return;
        StartCoroutine(ExitGameCoroutine(delay));
    }
    
    private IEnumerator ExitGameCoroutine(float delay = 1f)
    {
        if (_isLoading) yield break;
        _isLoading = true;

        yield return new WaitForSeconds(delay);
        Application.Quit();
    }
}
