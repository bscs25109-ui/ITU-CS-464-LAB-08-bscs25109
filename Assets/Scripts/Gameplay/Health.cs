using System;
using UnityEngine;

// The player's health. It does NOT know the HUD, the sound, the sparks or the camera. It only raises events.
public class Health : MonoBehaviour
{
    [SerializeField] int max = 100;
    [SerializeField] float iFrames = 0.5f;       // seconds of protection after a hit

    public event Action<int, int> Damaged;       // (current, max)
    public event Action<int, int> Healed;        // (current, max)
    public event Action Died;

    int current;
    float invulnerableUntil;

    public int Current => current;
    public int Max => max;

    void Awake() => current = max;

    public void TakeDamage(int amount)
    {
        if (current <= 0 || Time.time < invulnerableUntil) return;
        invulnerableUntil = Time.time + iFrames;
        current = Mathf.Max(0, current - amount);
        Damaged?.Invoke(current, max);                       // ?. because nobody may be listening
        GameEvents.RaisePlayerDamaged(current, max);         // static event for scripts without a reference
        if (current <= 0) Died?.Invoke();
    }

    public void Heal(int amount)
    {
        if (current <= 0) return;
        current = Mathf.Min(max, current + amount);
        Healed?.Invoke(current, max);
    }
}