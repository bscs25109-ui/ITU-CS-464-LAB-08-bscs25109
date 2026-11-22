# DEAD ZONE: OUTBREAK — Lab 08
**ITU CS-464** | **Muhammad Abu bakar (BSCS25109)** 

Built directly on top of my Lab 07 10-screen mobile UI project. All the old screens, navigation, tweens, and responsive layouts still work—Lab 08 adds an async loading screen, UI and gameplay audio, an Audio Mixer with saved volume controls, and a C# event system.

- **Demo Video:** [PASTE LINK HERE]
- **Windows Build:** [PASTE RELEASE LINK HERE OR DELETE LINE]

![Audio Mixer](docs/screenshots/audio-mixer.png)

---

## Setup & Controls
1. Open the project in Unity Hub (**Add > Add project from disk**).
2. Open `Assets/Scenes/MainMenu.unity` and hit **Play**.
   - Scene order in Build Profiles: `MainMenu` (0), `Loading` (1), `Game` (2).
3. **Controls:** `WASD` to move, `Space` (or **HIT**) to attack, `Esc` to pause.
   - Test keys: `H` (damage), `J` (heal), `K` (collect scrap).

---

## Loading Flow
Hitting **START MISSION** (or **Restart**) calls `SceneLoader.Load("Game")`, which opens the `Loading` scene. `LoadingScreen` loads the game scene asynchronously with `allowSceneActivation = false` so the player can't enter early. The progress bar and percentage track real progress (`op.progress / 0.9f`), rotate through tips, and only activate the scene once it hits 100% and plays the **READY** sound.

---

## Audio & Mixer Setup
- **One `AudioManager`:** A `DontDestroyOnLoad` singleton loaded from `Assets/Resources/AudioManager.prefab`. It creates and routes all 8 `AudioSource` components in code (2 Music, 4 SFX, 2 UI). No buttons or gameplay objects have their own `AudioSource`.
- **UI Sounds:** `UIButtonSoundInstaller` attaches a lightweight `UIButtonSound` to every button for hover, click, back/cancel, panel open, success, error (locked mission), and pause/resume—all routed to the **UI** group.
- **Gameplay SFX:** `GameAudioListener` plays pickup, heal, damage, attack, death, and win sounds using `PlayOneShot` across 4 pooled SFX sources, with random pitch (±2 semitones) and volume (`0.85`–`1.0`) variation.

**Mixer (`MainMixer`):** `Master` → `Music`, `SFX`, `UI`  
**Exposed Parameters:** `MasterVolume`, `MusicVolume`, `SFXVolume`, `UIVolume`  
**Snapshots:** `Normal` and `Paused` (Update Mode: Unscaled Time)

---

## Volume Controls & Persistence
- Sliders run from `0.0001` to `1.0` and convert to decibels using `Mathf.Log10(value) * 20f` (never linear, and safe from `Log10(0)`).
- **Mute** drops `MasterVolume` to `-80 dB` without overwriting the slider values, so unmuting brings back the exact same mix.
- All volumes and the mute toggle save to `PlayerPrefs` and reload automatically on restart.

---

## Music Transitions & Pause Behavior
Two looping Music sources handle equal-power crossfades (`cos` / `sin`) on unscaled time:
- **Menu → Gameplay:** `GameManager.GameStarted`
- **Gameplay → Combat:** `Health.Damaged` when HP ≤ 40%
- **Combat → Gameplay:** `Health.Healed` when HP > 60%
- **Gameplay / Combat → Victory or Defeat:** `GameManager.GameWon` / `GameLost`
- **Back to Menu:** `SceneMusic` on `MainMenu`

**While Paused:**
- **SFX:** Paused via `AudioListener.pause = true`.
- **Music:** Keeps playing (`ignoreListenerPause = true`), muffled using the `Paused` snapshot (Low-Pass 900 Hz, -8 dB) over 0.3s.
- **UI Sounds:** Keep playing (`ignoreListenerPause = true`) so pause menus still have feedback.

---

## C# Events
Gameplay systems raise C# events instead of directly calling UI, audio, or VFX scripts. All listeners subscribe in `OnEnable` (`+=`), unsubscribe in `OnDisable` (`-=`), and publishers invoke with `?.Invoke(...)`.

| Publisher | Event | Listeners |
|---|---|---|
| `Health` | `Damaged(current, max)` | `HudListener`, `ShakeOnHit`, `GameAudioListener`, `ParticleFeedback`, `HitFlash` |
| `Health` | `Healed(current, max)` | `HudListener`, `GameAudioListener`, `ParticleFeedback` |
| `Health` | `Died` | `GameManager`, `GameAudioListener` |
| `GameManager` | `GameStarted` | `HudListener`, `GameAudioListener` |
| `GameManager` | `Collected(score, target)` | `HudListener`, `GameAudioListener`, `ParticleFeedback` |
| `GameManager` | `GameWon` / `GameLost` | `GameUI`, `GameAudioListener` |
| `PlayerAttack` | `Attacked` | `GameAudioListener` |
| `GameUI` | `StateChanged(state)` | `PauseAudio` |

### Code Excerpt
```csharp
// Publisher: Health.cs
public event Action<int, int> Damaged;

public void TakeDamage(int amount)
{
    if (current <= 0 || Time.time < invulnerableUntil) return;
    invulnerableUntil = Time.time + iFrames;
    current = Mathf.Max(0, current - amount);

    Damaged?.Invoke(current, max);
    GameEvents.RaisePlayerDamaged(current, max);

    if (current <= 0) Died?.Invoke();
}

// Subscriber: ShakeOnHit.cs
void OnEnable()  => health.Damaged += Shake;
void OnDisable() => health.Damaged -= Shake;

void Shake(int current, int max) => shake.AddTrauma(0.55f);