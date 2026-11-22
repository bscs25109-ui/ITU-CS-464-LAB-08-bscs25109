using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] Health playerHealth;
    [SerializeField] int target = 5;                // scrap needed to win

    public event Action GameStarted;
    public event Action<int, int> Collected;        // (score, target)
    public event Action GameWon;
    public event Action GameLost;

    public int Score { get; private set; }
    public int Target => target;
    public bool IsOver { get; private set; }

    void Awake() => Instance = this;

    void OnEnable() => playerHealth.Died += Lose;
    void OnDisable() => playerHealth.Died -= Lose;

    void Start() => GameStarted?.Invoke();          // after every listener had its OnEnable

    public void Collect(int value = 1)
    {
        if (IsOver) return;
        Score += value;
        Collected?.Invoke(Score, target);
        if (Score >= target)
        {
            IsOver = true;
            GameWon?.Invoke();
        }
    }

    void Lose()
    {
        if (IsOver) return;
        IsOver = true;
        GameLost?.Invoke();
    }
}