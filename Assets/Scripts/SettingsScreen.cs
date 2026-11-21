using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsScreen : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] Slider volume;
    [SerializeField] TMP_Text volumeText;
    [SerializeField] Toggle music;
    [SerializeField] TMP_Text musicStateText;      // the ON / OFF word
    [SerializeField] Image musicTrack;             // Toggle_Music image
    [SerializeField] Image musicKnob;              // the round knob

    [Header("Graphics")]
    [SerializeField] TMP_Dropdown quality;

    [Header("Panels (Audio, Graphics, Controls)")]
    [SerializeField] RectTransform[] panels;

    static readonly Color32 Amber = new Color32(0xE8, 0xB7, 0x5A, 255);
    static readonly Color32 Midnight = new Color32(0x18, 0x2B, 0x36, 255);
    static readonly Color32 Mist = new Color32(0xA9, 0xBA, 0xC2, 255);
    static readonly Color32 Ivory = new Color32(0xF3, 0xF0, 0xE8, 255);

    const float DefaultVolume = 0.8f;
    Coroutine resetTween, knobTween;

    void OnEnable()
    {
        AudioListener.volume = GameSession.Volume;
        volume.SetValueWithoutNotify(GameSession.Volume);
        ShowVolume(GameSession.Volume);

        music.SetIsOnWithoutNotify(GameSession.Music);
        ShowMusic(GameSession.Music, false);

        quality.SetValueWithoutNotify(OptionFromQuality(QualitySettings.GetQualityLevel()));

        for (int i = 0; i < panels.Length; i++)    // the three groups arrive in order
            Tween.Reveal(this, panels[i], new Vector2(0, -40), 0.30f, 0.08f * i, Ease.OutCubic);
    }

    // ----- Volume -----
    public void OnVolume(float v)                  // Slider On Value Changed (Dynamic float)
    {
        GameSession.Volume = v;
        AudioListener.volume = v;
        ShowVolume(v);                             // no tween here: motion while dragging would distract
    }

    void ShowVolume(float v) => volumeText.SetText("{0}%", Mathf.RoundToInt(v * 100f));

    // ----- Music -----
    public void OnMusic(bool on)                   // Toggle On Value Changed (Dynamic bool)
    {
        GameSession.Music = on;
        ShowMusic(on, true);
    }

    void ShowMusic(bool on, bool animate)
    {
        musicStateText.text = on ? "ON" : "OFF";
        musicStateText.color = on ? Ivory : Mist;
        musicTrack.color = on ? Amber : Midnight;   // light track = ON, dark track = OFF
        musicKnob.color = on ? Midnight : Mist;    // knob stays visible on both

        float target = on ? 35f : -35f;
        RectTransform knob = musicKnob.rectTransform;
        if (knobTween != null) StopCoroutine(knobTween);
        if (!animate)
        {
            knob.anchoredPosition = new Vector2(target, 0f);
            return;
        }
        float from = knob.anchoredPosition.x;
        knobTween = Tween.Run(this, 0.15f, Ease.OutCubic, t =>
            knob.anchoredPosition = new Vector2(Mathf.LerpUnclamped(from, target, t), 0f));
    }

    // ----- Quality -----
    public void OnQuality(int option)              // Dropdown On Value Changed (Dynamic int)
        => QualitySettings.SetQualityLevel(QualityFromOption(option), true);

    // Map the 3 options (LOW, MEDIUM, HIGH) onto however many quality levels the project has.
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
        if (resetTween != null) StopCoroutine(resetTween);
        float from = volume.value;
        resetTween = Tween.Run(this, 0.30f, Ease.InOutQuad,
            t => volume.value = Mathf.Lerp(from, DefaultVolume, t));   // the handle glides home
        music.isOn = true;                          // the knob slides, OnMusic runs
        quality.value = 2;                          // HIGH, OnQuality runs
    }
}
