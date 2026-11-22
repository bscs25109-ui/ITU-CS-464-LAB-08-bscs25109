using UnityEngine;

// Listens to the game's events and answers with sounds and music. No other script plays gameplay sounds.
public class GameAudioListener : MonoBehaviour
{
    [SerializeField] Health health;
    [SerializeField] GameManager game;
    [SerializeField] PlayerAttack attack;

    bool combat;                                    // are we on the combat track?

    void OnEnable()
    {
        health.Damaged += OnDamaged;
        health.Healed += OnHealed;
        health.Died += OnDied;
        game.GameStarted += OnStarted;
        game.Collected += OnCollected;
        game.GameWon += OnWon;
        game.GameLost += OnLost;
        attack.Attacked += OnAttacked;
    }

    void OnDisable()
    {
        health.Damaged -= OnDamaged;
        health.Healed -= OnHealed;
        health.Died -= OnDied;
        game.GameStarted -= OnStarted;
        game.Collected -= OnCollected;
        game.GameWon -= OnWon;
        game.GameLost -= OnLost;
        attack.Attacked -= OnAttacked;
    }

    void OnStarted()
    {
        combat = false;
        AudioManager.Music(MusicTrack.Gameplay);    // menu music crossfades into the gameplay track
    }

    void OnDamaged(int current, int max)
    {
        AudioManager.SFX(Sfx.Damage);
        if (!combat && current > 0 && current <= max * 0.4f)
        {
            combat = true;
            AudioManager.Music(MusicTrack.Combat);  // low health: calm -> combat
        }
    }

    void OnHealed(int current, int max)
    {
        AudioManager.SFX(Sfx.Heal);
        if (combat && current > max * 0.6f)
        {
            combat = false;
            AudioManager.Music(MusicTrack.Gameplay);// healthy again: combat -> calm
        }
    }

    void OnCollected(int score, int target)
    {
        if (score < target) AudioManager.SFX(Sfx.Pickup);   // the last scrap plays the win sound instead
    }

    void OnAttacked() => AudioManager.SFX(Sfx.Attack);
    void OnDied() => AudioManager.SFX(Sfx.Death);

    void OnWon()
    {
        AudioManager.SFX(Sfx.Win);
        AudioManager.Music(MusicTrack.Victory, 0.6f);        // gameplay -> results
    }

    void OnLost() => AudioManager.Music(MusicTrack.Defeat, 0.6f);
}