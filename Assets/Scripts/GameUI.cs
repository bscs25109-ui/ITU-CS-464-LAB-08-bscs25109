using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public enum GameState { Playing, Paused, Over }

public class GameUI : MonoBehaviour
{
    public static GameUI Instance { get; private set; }
    public static bool IsPaused => Instance != null && Instance.State == GameState.Paused;

    [SerializeField] GameObject hudPanel, pausePanel, overPanel;
    [SerializeField] GameObject pauseFirstButton, overFirstButton;
    [SerializeField] HealthBar healthBar;
    [SerializeField] ScoreText scoreText;
    [SerializeField] PauseScreen pauseScreen;     // plays the open/close tween
    [SerializeField] OverScreen overScreen;
    public GameState State { get; private set; }

    int health = 100, score = 0;
    const int MaxHealth = 100, Target = 10;

    void Awake() => Instance = this;

    void Start()
    {
        AudioListener.pause = false;
        SetState(GameState.Playing);
        healthBar.Set(health, MaxHealth);
        scoreText.Show(score, Target);
    }

    public void SetState(GameState next)
    {
        State = next;
        hudPanel.SetActive(true);
        pausePanel.SetActive(next == GameState.Paused);
        overPanel.SetActive(next == GameState.Over);
        Time.timeScale = next == GameState.Playing ? 1f : 0f;
        AudioListener.pause = next != GameState.Playing;

        if (next == GameState.Paused)
            EventSystem.current.SetSelectedGameObject(pauseFirstButton);
        if (next == GameState.Over)
            EventSystem.current.SetSelectedGameObject(overFirstButton);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (State == GameState.Playing) Pause();
            else if (State == GameState.Paused) Resume();
        }
        // test keys: H = take damage, K = score a kill
        if (State == GameState.Playing)
        {
            if (Input.GetKeyDown(KeyCode.H)) Damage(25);
            if (Input.GetKeyDown(KeyCode.K)) AddScore(1);
        }
    }

    public void Pause() => SetState(GameState.Paused);     // PauseScreen.OnEnable plays the opening tween

    public void Resume()
    {
        if (State != GameState.Paused) return;
        // Time stays frozen while the panel closes; the tween runs on unscaled time.
        pauseScreen.Close(() => SetState(GameState.Playing));
    }

    public void Damage(int amount)
    {
        health = Mathf.Max(0, health - amount);
        healthBar.Set(health, MaxHealth);
        if (health == 0) { overScreen.Show(false, score, Target); SetState(GameState.Over); }
    }

    public void AddScore(int n)
    {
        score += n;
        scoreText.Show(score, Target);
        Tween.Punch(this, scoreText.transform, 0.15f, 0.2f);   // score pulses when it changes
        if (score >= Target) { overScreen.Show(true, score, Target); SetState(GameState.Over); }
    }

    public void Restart()
    {
        Time.timeScale = 1f;                 // the clock survives LoadScene, so reset it first
        AudioListener.pause = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitToMenu()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;
        SceneManager.LoadScene("MainMenu");
    }
}
