using UnityEngine;

public class HudListener : MonoBehaviour
{
    [SerializeField] Health health;
    [SerializeField] GameManager game;
    [SerializeField] HealthBar healthBar;
    [SerializeField] ScoreText scoreText;

    void OnEnable()
    {
        health.Damaged += OnHealthChanged;
        health.Healed += OnHealthChanged;
        game.GameStarted += Refresh;
        game.Collected += OnCollected;
    }

    void OnDisable()
    {
        health.Damaged -= OnHealthChanged;
        health.Healed -= OnHealthChanged;
        game.GameStarted -= Refresh;
        game.Collected -= OnCollected;
    }

    void Refresh()
    {
        healthBar.Set(health.Current, health.Max);
        scoreText.Show(game.Score, game.Target);
    }

    void OnHealthChanged(int current, int max) => healthBar.Set(current, max);

    void OnCollected(int score, int target)
    {
        scoreText.Show(score, target);
        Tween.Punch(this, scoreText.transform, 0.15f, 0.2f);   // the score pulses
    }
}