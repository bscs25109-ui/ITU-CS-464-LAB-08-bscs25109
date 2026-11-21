using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Added to every Button by UIButtonSoundInstaller. It owns no AudioSource: it asks the AudioManager.
public class UIButtonSound : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, ISelectHandler
{
    Button button;
    bool silent;                       // these buttons get their sound from somewhere else
    UISound clickSound = UISound.Click;

    void Awake()
    {
        button = GetComponent<Button>();
        string n = name;
        // MenuNavigator plays the Back sound, PauseAudio plays Pause/Resume, other scripts play Success/Error.
        if (n.Contains("Back") || n.Contains("Pause") || n.Contains("Resume") ||
            n.Contains("Equip") || n.Contains("Use") || n.Contains("Locked") || n.Contains("Attack"))
            silent = true;
        if (n.Contains("Quit") || n.Contains("Menu"))    // leaving the game = cancel sound
            clickSound = UISound.Back;
    }

    public void OnPointerEnter(PointerEventData e)
    {
        if (e.pointerId >= 0) return;                    // fingers have no hover: skip touches
        if (button != null && button.interactable) AudioManager.UI(UISound.Hover);
    }

    public void OnSelect(BaseEventData e)
    {
        // Only keyboard / gamepad moves send AxisEventData. The automatic "first button" selection does not.
        if (e is AxisEventData && button != null && button.interactable) AudioManager.UI(UISound.Hover);
    }

    public void OnPointerClick(PointerEventData e)
    {
        if (silent || button == null || !button.interactable) return;
        AudioManager.UI(clickSound);
    }
}