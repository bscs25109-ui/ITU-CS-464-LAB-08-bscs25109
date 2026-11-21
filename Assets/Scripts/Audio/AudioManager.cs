using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public enum VolumeChannel { Master, Music, SFX, UI }
public enum MusicTrack { None, Menu, Gameplay, Combat, Victory, Defeat }
public enum UISound { Hover, Click, Back, Open, Success, Error, Pause, Resume }
public enum Sfx { Pickup, Heal, Damage, Attack, Death, Win }

// One object that plays every sound, survives scene loads, and owns the volume settings.
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Mixer (MainMixer)")]
    [SerializeField] AudioMixer mixer;

    [Header("Music loops")]
    [SerializeField] AudioClip musicMenu, musicGameplay, musicCombat, musicVictory, musicDefeat;

    [Header("UI sounds")]
    [SerializeField] AudioClip uiHover, uiClick, uiBack, uiOpen, uiSuccess, uiError, uiPause, uiResume;

    [Header("Gameplay sounds")]
    [SerializeField] AudioClip sfxPickup, sfxHeal, sfxDamage, sfxAttack, sfxDeath, sfxWin;

    [Header("Variation (Lecture 09)")]
    [SerializeField] float pitchRange = 2f;        // plus or minus semitones
    [SerializeField] float minVolume = 0.85f;      // random volume between this and 1
    [SerializeField] float crossfadeSeconds = 1.5f;

    // The names you gave the exposed parameters in the mixer, in VolumeChannel order.
    static readonly string[] Params = { "MasterVolume", "MusicVolume", "SFXVolume", "UIVolume" };
    static readonly float[] Defaults = { 0.8f, 0.7f, 0.9f, 0.9f };
    const string MuteKey = "Muted";
    const float MinSlider = 0.0001f;               // log10(0) is minus infinity, so never go to 0

    readonly AudioSource[] music = new AudioSource[2];
    readonly AudioSource[] sfx = new AudioSource[4];
    readonly AudioSource[] ui = new AudioSource[2];
    readonly float[] volumes = new float[4];
    int activeMusic, nextSfx, nextUi;
    bool muted;
    MusicTrack currentTrack = MusicTrack.None;
    Coroutine fade;

    // Runs before the first scene, so the manager exists even if you press Play from the Game scene.
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Boot()
    {
        if (Instance != null) return;
        AudioManager prefab = Resources.Load<AudioManager>("AudioManager");
        if (prefab == null)
        {
            Debug.LogError("AudioManager prefab not found. It must be at Assets/Resources/AudioManager.prefab");
            return;
        }
        Instantiate(prefab);
    }

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Every AudioSource in the game is made here and routed to its mixer group.
        AudioMixerGroup gMusic = mixer.FindMatchingGroups("Master/Music")[0];
        AudioMixerGroup gSfx = mixer.FindMatchingGroups("Master/SFX")[0];
        AudioMixerGroup gUI = mixer.FindMatchingGroups("Master/UI")[0];
        for (int i = 0; i < music.Length; i++) music[i] = MakeSource(gMusic, true, true);   // keeps playing when paused
        for (int i = 0; i < sfx.Length; i++) sfx[i] = MakeSource(gSfx, false, false);  // freezes when paused
        for (int i = 0; i < ui.Length; i++) ui[i] = MakeSource(gUI, false, true);   // keeps playing when paused

        for (int i = 0; i < volumes.Length; i++)
            volumes[i] = Mathf.Clamp(PlayerPrefs.GetFloat(Params[i], Defaults[i]), MinSlider, 1f);
        muted = PlayerPrefs.GetInt(MuteKey, 0) == 1;
        ApplyAll();
    }

    void Start()
    {
        ApplyAll();                                  // some Unity versions ignore SetFloat inside Awake
        AudioMixerSnapshot normal = mixer.FindSnapshot("Normal");
        if (normal != null) normal.TransitionTo(0f);
    }

    AudioSource MakeSource(AudioMixerGroup group, bool loop, bool ignorePause)
    {
        AudioSource s = gameObject.AddComponent<AudioSource>();
        s.outputAudioMixerGroup = group;
        s.playOnAwake = false;
        s.loop = loop;
        s.spatialBlend = 0f;                          // 2D
        s.ignoreListenerPause = ignorePause;
        return s;
    }

    // ---------------------------------------------------------------- volumes (decibels)
    // 1 -> 0 dB, 0.5 -> -6 dB, 0.0001 -> -80 dB (silence). Never log10(0).
    public static float ToDecibels(float value01) => Mathf.Log10(Mathf.Clamp(value01, MinSlider, 1f)) * 20f;

    public float GetVolume(VolumeChannel c) => volumes[(int)c];
    public static float DefaultVolume(VolumeChannel c) => Defaults[(int)c];
    public bool Muted => muted;

    public void SetVolume(VolumeChannel c, float value01)
    {
        int i = (int)c;
        volumes[i] = Mathf.Clamp(value01, MinSlider, 1f);
        Apply(i);
        PlayerPrefs.SetFloat(Params[i], volumes[i]);
    }

    public void SetMuted(bool value)
    {
        muted = value;
        Apply(0);                                     // mute is applied on Master only
        PlayerPrefs.SetInt(MuteKey, value ? 1 : 0);
    }

    public void ResetVolumes()
    {
        for (int i = 0; i < volumes.Length; i++)
        {
            volumes[i] = Defaults[i];
            PlayerPrefs.SetFloat(Params[i], volumes[i]);
        }
        muted = false;
        PlayerPrefs.SetInt(MuteKey, 0);
        ApplyAll();
    }

    public void SaveSettings() => PlayerPrefs.Save();
    void OnApplicationQuit() => PlayerPrefs.Save();
    void OnApplicationPause(bool paused) { if (paused) PlayerPrefs.Save(); }   // phones

    void ApplyAll() { for (int i = 0; i < volumes.Length; i++) Apply(i); }

    void Apply(int i)
    {
        float v = (i == 0 && muted) ? 0f : volumes[i];   // muted = -80 dB, safely
        mixer.SetFloat(Params[i], ToDecibels(v));
    }

    // ---------------------------------------------------------------- UI and effects (PlayOneShot)
    public void PlayUI(UISound s)
    {
        AudioClip clip = ClipFor(s);
        if (clip == null) return;
        AudioSource src = ui[nextUi];
        nextUi = (nextUi + 1) % ui.Length;
        float semitones = (s == UISound.Hover || s == UISound.Click) ? Random.Range(-1f, 1f) : 0f;
        src.pitch = Mathf.Pow(2f, semitones / 12f);
        float vol = (s == UISound.Hover || s == UISound.Error) ? 0.6f : 1f;
        src.PlayOneShot(clip, vol);
    }

    public void PlaySfx(Sfx s)
    {
        AudioClip clip = ClipFor(s);
        if (clip == null) return;
        AudioSource src = sfx[nextSfx];               // pitch belongs to the source, so take the sources in turn
        nextSfx = (nextSfx + 1) % sfx.Length;
        src.pitch = Mathf.Pow(2f, Random.Range(-pitchRange, pitchRange) / 12f);
        src.PlayOneShot(clip, Random.Range(minVolume, 1f));
    }

    // ---------------------------------------------------------------- music (equal-power crossfade)
    public void PlayMusic(MusicTrack track, float fadeSeconds = -1f)
    {
        if (track == currentTrack) return;
        currentTrack = track;
        if (fadeSeconds < 0f) fadeSeconds = crossfadeSeconds;

        AudioClip clip = ClipFor(track);
        AudioSource from = music[activeMusic];
        AudioSource to = music[1 - activeMusic];
        activeMusic = 1 - activeMusic;

        if (fade != null) StopCoroutine(fade);
        if (clip != null)
        {
            to.clip = clip;
            to.volume = 0f;
            to.Play();                                 // looping source
        }
        fade = StartCoroutine(Fade(from, to, clip != null, fadeSeconds));
    }

    IEnumerator Fade(AudioSource from, AudioSource to, bool fadeIn, float seconds)
    {
        float start = from.volume;
        for (float t = 0f; t < seconds; t += Time.unscaledDeltaTime)   // unscaled: works under the pause menu
        {
            float u = t / seconds;
            from.volume = start * Mathf.Cos(u * Mathf.PI / 2f);        // equal power: cos and sin
            if (fadeIn) to.volume = Mathf.Sin(u * Mathf.PI / 2f);
            yield return null;
        }
        from.Stop();
        from.volume = 0f;
        if (fadeIn) to.volume = 1f;
    }

    // Pause menu: the Paused snapshot muffles and lowers the music (SFX are stopped by AudioListener.pause)
    public void SetPausedMix(bool paused, float seconds = 0.3f)
    {
        AudioMixerSnapshot snap = mixer.FindSnapshot(paused ? "Paused" : "Normal");
        if (snap != null) snap.TransitionTo(seconds);
    }

    AudioClip ClipFor(UISound s)
    {
        switch (s)
        {
            case UISound.Hover: return uiHover;
            case UISound.Click: return uiClick;
            case UISound.Back: return uiBack;
            case UISound.Open: return uiOpen;
            case UISound.Success: return uiSuccess;
            case UISound.Error: return uiError;
            case UISound.Pause: return uiPause;
            default: return uiResume;
        }
    }

    AudioClip ClipFor(Sfx s)
    {
        switch (s)
        {
            case Sfx.Pickup: return sfxPickup;
            case Sfx.Heal: return sfxHeal;
            case Sfx.Damage: return sfxDamage;
            case Sfx.Attack: return sfxAttack;
            case Sfx.Death: return sfxDeath;
            default: return sfxWin;
        }
    }

    AudioClip ClipFor(MusicTrack t)
    {
        switch (t)
        {
            case MusicTrack.Menu: return musicMenu;
            case MusicTrack.Gameplay: return musicGameplay;
            case MusicTrack.Combat: return musicCombat;
            case MusicTrack.Victory: return musicVictory;
            case MusicTrack.Defeat: return musicDefeat;
            default: return null;
        }
    }

    // ---------------------------------------------------------------- one-line helpers for every other script
    public static void UI(UISound s) { if (Instance != null) Instance.PlayUI(s); }
    public static void SFX(Sfx s) { if (Instance != null) Instance.PlaySfx(s); }
    public static void Music(MusicTrack t, float fadeSeconds = -1f) { if (Instance != null) Instance.PlayMusic(t, fadeSeconds); }
    public static void PausedMix(bool paused) { if (Instance != null) Instance.SetPausedMix(paused); }
}