using MEC;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : Singleton<GameManager>
{
    public PlayerEntity player; //nanti pindah ke spawn
    private float baseTimeScale;
    private bool timeBreak;
    private float timeBreakCounter;

    [Title("Game Over")]
    public CanvasGroup gameOverPanel;
    public Button retryButton;

    private void Awake()
    {
        baseTimeScale = Time.timeScale;
    }

    private void Start()
    {
        if(retryButton) retryButton.onClick.AddListener(Retry);
        if(gameOverPanel) gameOverPanel.gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        if(retryButton) retryButton.onClick.RemoveListener(Retry);
    }

    private void Update()
    {
        TimeBreakUpdate();
    }

    public void TimeBreak()
    {
        TimeBreak(0.2f);
    }
    public void TimeBreak(float time)
    {
        TimeBreak(time, 0.1f);
    }
    public void TimeBreak(float time, float targetTimeScale)
    {
        timeBreakCounter = time;
        Time.timeScale = targetTimeScale;
        timeBreak = true;
    }
    private void TimeBreakUpdate()
    {
        if (!timeBreak) return;

        timeBreakCounter -= Time.unscaledDeltaTime;
        if (timeBreakCounter < 0)
        {
            ResetTimeScale();
            timeBreak = false;
        }
    }

    public void ResetTimeScale()
    {
        Time.timeScale = baseTimeScale;
    }

    [HideInInspector] public bool isGameOver;
    public static System.Action OnGameOver;
    public void GameOver()
    {
        isGameOver = true;
        OnGameOver?.Invoke();

        Timing.RunCoroutine(GameOverSequence(), Segment.RealtimeUpdate);
    }

    IEnumerator<float> GameOverSequence()
    {
        //TimeBreak 
        TimeBreak(1);
        yield return Timing.WaitForSeconds(1);
        //Show Game Over UI
        gameOverPanel.gameObject.SetActive(true);
        Utilities.PlayForward(gameOverPanel, 1f);
    }

    public void Retry()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
