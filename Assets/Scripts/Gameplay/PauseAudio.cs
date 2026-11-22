using UnityEngine;

public class PauseAudio : MonoBehaviour
{
    GameState last = GameState.Playing;

    void OnEnable() => GameUI.StateChanged += OnState;       // static event: unsubscribe in OnDisable!
    void OnDisable() => GameUI.StateChanged -= OnState;

    void OnState(GameState state)
    {
        if (state == GameState.Paused)
        {
            AudioManager.UI(UISound.Pause);                  // UI source ignores the listener pause, so it is heard
            AudioManager.PausedMix(true);                    // Paused snapshot: music muffled and lower
        }
        else if (last == GameState.Paused)
        {
            AudioManager.UI(UISound.Resume);
            AudioManager.PausedMix(false);
        }
        last = state;
    }
}