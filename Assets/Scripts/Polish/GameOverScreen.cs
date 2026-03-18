using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public sealed class GameOverScreen : MonoBehaviour
{
    public static GameOverScreen Instance { get; private set; }

    [SerializeField] Canvas gameOverCanvas;
    [SerializeField] float delay = 1f;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Start()
    {
        gameOverCanvas.enabled = false;
    }

    public void TriggerGameOver()
    {
        StartCoroutine(ShowAfterDelay());
    }

    IEnumerator ShowAfterDelay()
    {
        yield return new WaitForSecondsRealtime(delay);
        Time.timeScale = 0f;
        gameOverCanvas.enabled = true;
    }

    public void RestartScene()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}