//using TMPro;
//using UnityEngine;
//using UnityEngine.UI;

//public class SettingsScreen : MonoBehaviour
//{
//    [Header("Audio")]
//    [SerializeField] Slider volume;
//    [SerializeField] TMP_Text volumeText;
//    [SerializeField] Toggle music;
//    [SerializeField] TMP_Text musicStateText;      // the ON / OFF word
//    [SerializeField] Image musicTrack;             // Toggle_Music image
//    [SerializeField] Image musicKnob;              // the round knob

//    [Header("Graphics")]
//    [SerializeField] TMP_Dropdown quality;

//    [Header("Panels (Audio, Graphics, Controls)")]
//    [SerializeField] RectTransform[] panels;

//    static readonly Color32 Amber = new Color32(0xE8, 0xB7, 0x5A, 255);
//    static readonly Color32 Midnight = new Color32(0x18, 0x2B, 0x36, 255);
//    static readonly Color32 Mist = new Color32(0xA9, 0xBA, 0xC2, 255);
//    static readonly Color32 Ivory = new Color32(0xF3, 0xF0, 0xE8, 255);

//    const float DefaultVolume = 0.8f;
//    Coroutine resetTween, knobTween;

//    void OnEnable()
//    {
//        AudioListener.volume = GameSession.Volume;
//        volume.SetValueWithoutNotify(GameSession.Volume);
//        ShowVolume(GameSession.Volume);

//        music.SetIsOnWithoutNotify(GameSession.Music);
//        ShowMusic(GameSession.Music, false);

//        quality.SetValueWithoutNotify(OptionFromQuality(QualitySettings.GetQualityLevel()));

//        for (int i = 0; i < panels.Length; i++)    // the three groups arrive in order
//            Tween.Reveal(this, panels[i], new Vector2(0, -40), 0.30f, 0.08f * i, Ease.OutCubic);
//    }

//    // ----- Volume -----
//    public void OnVolume(float v)                  // Slider On Value Changed (Dynamic float)
//    {
//        GameSession.Volume = v;
//        AudioListener.volume = v;
//        ShowVolume(v);                             // no tween here: motion while dragging would distract
//    }

//    void ShowVolume(float v) => volumeText.SetText("{0}%", Mathf.RoundToInt(v * 100f));

//    // ----- Music -----
//    public void OnMusic(bool on)                   // Toggle On Value Changed (Dynamic bool)
//    {
//        GameSession.Music = on;
//        ShowMusic(on, true);
//    }

//    void ShowMusic(bool on, bool animate)
//    {
//        musicStateText.text = on ? "ON" : "OFF";
//        musicStateText.color = on ? Ivory : Mist;
//        musicTrack.color = on ? Amber : Midnight;   // light track = ON, dark track = OFF
//        musicKnob.color = on ? Midnight : Mist;    // knob stays visible on both

//        float target = on ? 35f : -35f;
//        RectTransform knob = musicKnob.rectTransform;
//        if (knobTween != null) StopCoroutine(knobTween);
//        if (!animate)
//        {
//            knob.anchoredPosition = new Vector2(target, 0f);
//            return;
//        }
//        float from = knob.anchoredPosition.x;
//        knobTween = Tween.Run(this, 0.15f, Ease.OutCubic, t =>
//            knob.anchoredPosition = new Vector2(Mathf.LerpUnclamped(from, target, t), 0f));
//    }

//    // ----- Quality -----
//    public void OnQuality(int option)              // Dropdown On Value Changed (Dynamic int)
//        => QualitySettings.SetQualityLevel(QualityFromOption(option), true);

//    // Map the 3 options (LOW, MEDIUM, HIGH) onto however many quality levels the project has.
//    static int QualityFromOption(int option)
//    {
//        int max = QualitySettings.names.Length - 1;
//        return option == 0 ? 0 : (option == 1 ? max / 2 : max);
//    }

//    static int OptionFromQuality(int level)
//    {
//        int max = QualitySettings.names.Length - 1;
//        if (max <= 0) return 0;
//        float t = (float)level / max;
//        return t < 0.34f ? 0 : (t < 0.67f ? 1 : 2);
//    }

//    // ----- Reset -----
//    public void ResetDefaults()
//    {
//        if (resetTween != null) StopCoroutine(resetTween);
//        float from = volume.value;
//        resetTween = Tween.Run(this, 0.30f, Ease.InOutQuad,
//            t => volume.value = Mathf.Lerp(from, DefaultVolume, t));   // the handle glides home
//        music.isOn = true;                          // the knob slides, OnMusic runs
//        quality.value = 2;                          // HIGH, OnQuality runs
//    }
//}
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsScreen : MonoBehaviour
{
    [Header("Sound ON / OFF (mute) switch")]
    [SerializeField] Toggle sound;
    [SerializeField] TMP_Text soundStateText;       // the ON / OFF word
    [SerializeField] Image soundTrack;              // Toggle_Music image
    [SerializeField] Image soundKnob;               // the round knob

    [Header("Volume sliders (Master, Music, SFX, UI)")]
    [SerializeField] VolumeSlider[] sliders;

    [Header("Graphics")]
    [SerializeField] TMP_Dropdown quality;

    [Header("Panels (Audio, Graphics)")]
    [SerializeField] RectTransform[] panels;

    static readonly Color32 Amber = new Color32(0xE8, 0xB7, 0x5A, 255);
    static readonly Color32 Midnight = new Color32(0x18, 0x2B, 0x36, 255);
    static readonly Color32 Mist = new Color32(0xA9, 0xBA, 0xC2, 255);
    static readonly Color32 Ivory = new Color32(0xF3, 0xF0, 0xE8, 255);

    Coroutine knobTween;

    void OnEnable()
    {
        bool soundOn = AudioManager.Instance == null || !AudioManager.Instance.Muted;
        if (sound != null) sound.SetIsOnWithoutNotify(soundOn);
        ShowSound(soundOn, false);

        if (quality != null)
            quality.SetValueWithoutNotify(OptionFromQuality(QualitySettings.GetQualityLevel()));

        if (panels != null)
        {
            for (int i = 0; i < panels.Length; i++)     // the groups arrive in order
            {
                if (panels[i] == null) continue;
                Tween.Reveal(this, panels[i], new Vector2(0, -40), 0.30f, 0.08f * i, Ease.OutCubic);
            }
        }
    }

    void OnDisable()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.SaveSettings();
    }

    // ----- Sound ON / OFF (mute) -----
    public void OnSound(bool on)                    // Toggle On Value Changed (Dynamic bool)
    {
        if (AudioManager.Instance != null) AudioManager.Instance.SetMuted(!on);
        ShowSound(on, true);
        if (on) AudioManager.UI(UISound.Click);     // you only hear it when sound is ON
    }

    void ShowSound(bool on, bool animate)
    {
        if (soundStateText == null || soundTrack == null || soundKnob == null)
        {
            Debug.LogWarning("SettingsScreen: Sound State Text, Sound Track or Sound Knob is empty");
            return;
        }

        soundStateText.text = on ? "ON" : "OFF";
        soundStateText.color = on ? Ivory : Mist;
        soundTrack.color = on ? Amber : Midnight;   // light track = ON, dark track = OFF
        soundKnob.color = on ? Midnight : Mist;

        float target = on ? 35f : -35f;
        RectTransform knob = soundKnob.rectTransform;
        if (knobTween != null) StopCoroutine(knobTween);
        if (!animate || !isActiveAndEnabled)
        {
            knob.anchoredPosition = new Vector2(target, 0f);
            return;
        }
        float from = knob.anchoredPosition.x;
        knobTween = Tween.Run(this, 0.15f, Ease.OutCubic, t =>
            knob.anchoredPosition = new Vector2(Mathf.LerpUnclamped(from, target, t), 0f));
    }

    // ----- Quality -----
    public void OnQuality(int option)               // Dropdown On Value Changed (Dynamic int)
        => QualitySettings.SetQualityLevel(QualityFromOption(option), true);

    static int QualityFromOption(int option)
    {
        int max = QualitySettings.names.Length - 1;
        return option == 0 ? 0 : (option == 1 ? max / 2 : max);
    }

    static int OptionFromQuality(int level)
    {
        int max = QualitySettings.names.Length - 1;
        if (max <= 0) return 0;
        float t = (float)level / max;
        return t < 0.34f ? 0 : (t < 0.67f ? 1 : 2);
    }

    // ----- Reset -----
    public void ResetDefaults()
    {
        if (sound == null) Debug.LogWarning("SettingsScreen: the 'Sound' field is empty");
        else sound.isOn = true;                     // runs OnSound: knob slides, mute off

        if (sliders == null || sliders.Length == 0)
        {
            Debug.LogWarning("SettingsScreen: the 'Sliders' list is empty");
        }
        else
        {
            foreach (VolumeSlider vs in sliders)    // each handle glides to its default
            {
                if (vs == null || vs.Slider == null)
                {
                    Debug.LogWarning("SettingsScreen: a Sliders element is None, or its VolumeSlider has no Slider assigned");
                    continue;
                }
                Slider s = vs.Slider;
                float from = s.value;
                float to = AudioManager.DefaultVolume(vs.Channel);
                Tween.Run(this, 0.30f, Ease.InOutQuad, t => s.value = Mathf.Lerp(from, to, t));
            }
        }

        if (quality != null) quality.value = 2;     // HIGH: runs OnQuality
        AudioManager.UI(UISound.Success);
    }
}