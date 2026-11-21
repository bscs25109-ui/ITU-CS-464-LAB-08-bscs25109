//using TMPro;
//using UnityEngine;
//using UnityEngine.EventSystems;
//using UnityEngine.UI;

//// Put on a Slider. It talks to the AudioManager: slider 0.0001-1 -> decibels -> mixer -> PlayerPrefs.
//public class VolumeSlider : MonoBehaviour, IPointerUpHandler
//{
//    [SerializeField] VolumeChannel channel;
//    [SerializeField] Slider slider;          // Min Value 0.0001, Max Value 1
//    [SerializeField] TMP_Text valueText;

//    public Slider Slider => slider;
//    public VolumeChannel Channel => channel;

//    void OnEnable()
//    {
//        Refresh();
//        slider.onValueChanged.AddListener(OnChanged);      // wired in code: nothing to set in the Inspector
//    }

//    void OnDisable()
//    {
//        slider.onValueChanged.RemoveListener(OnChanged);
//        if (AudioManager.Instance != null) AudioManager.Instance.SaveSettings();   // keep the value for next launch
//    }

//    public void Refresh()                                   // show the stored value without triggering a change
//    {
//        if (AudioManager.Instance == null) return;
//        float v = AudioManager.Instance.GetVolume(channel);
//        slider.SetValueWithoutNotify(v);
//        Show(v);
//    }

//    void OnChanged(float v)
//    {
//        if (AudioManager.Instance != null) AudioManager.Instance.SetVolume(channel, v);
//        Show(v);
//    }

//    void Show(float v) => valueText.SetText("{0}%", Mathf.RoundToInt(v * 100f));

//    // When the finger or mouse is released, play a sample so the player hears the new level.
//    public void OnPointerUp(PointerEventData e)
//    {
//        switch (channel)
//        {
//            case VolumeChannel.SFX: AudioManager.SFX(Sfx.Pickup); break;
//            case VolumeChannel.Music: break;                // the music itself is the sample
//            default: AudioManager.UI(UISound.Click); break; // Master and UI
//        }
//    }
//}


using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Put on a Slider. It talks to the AudioManager: slider 0.0001-1 -> decibels -> mixer -> PlayerPrefs.
public class VolumeSlider : MonoBehaviour, IPointerUpHandler
{
    [SerializeField] VolumeChannel channel;
    [SerializeField] Slider slider;          // optional: the script finds the Slider on this object by itself
    [SerializeField] TMP_Text valueText;

    // Fills the Slider field automatically when you press Add Component.
    void Reset() { slider = GetComponent<Slider>(); }

    // Always returns the Slider on this object, even if the field was left empty.
    public Slider Slider
    {
        get
        {
            if (slider == null) slider = GetComponent<Slider>();
            return slider;
        }
    }

    public VolumeChannel Channel => channel;

    void OnEnable()
    {
        if (Slider == null)
        {
            Debug.LogError("VolumeSlider on '" + name + "': this object has no Slider component.");
            return;
        }
        Refresh();
        Slider.onValueChanged.AddListener(OnChanged);       // wired in code: nothing to set in the Inspector
    }

    void OnDisable()
    {
        if (Slider != null) Slider.onValueChanged.RemoveListener(OnChanged);
        if (AudioManager.Instance != null) AudioManager.Instance.SaveSettings();   // keep the value for next launch
    }

    public void Refresh()                                   // show the stored value without triggering a change
    {
        if (AudioManager.Instance == null || Slider == null) return;
        float v = AudioManager.Instance.GetVolume(channel);
        Slider.SetValueWithoutNotify(v);
        Show(v);
    }

    void OnChanged(float v)
    {
        if (AudioManager.Instance != null) AudioManager.Instance.SetVolume(channel, v);
        Show(v);
    }

    void Show(float v)
    {
        if (valueText != null) valueText.SetText("{0}%", Mathf.RoundToInt(v * 100f));
    }

    // When the finger or mouse is released, play a sample so the player hears the new level.
    public void OnPointerUp(PointerEventData e)
    {
        switch (channel)
        {
            case VolumeChannel.SFX: AudioManager.SFX(Sfx.Pickup); break;
            case VolumeChannel.Music: break;                // the music itself is the sample
            default: AudioManager.UI(UISound.Click); break; // Master and UI
        }
    }
}