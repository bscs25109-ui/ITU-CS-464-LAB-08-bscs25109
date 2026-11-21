
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DifficultyToggleVisual : MonoBehaviour
{
    [SerializeField] private Toggle difficultyToggle;
    [SerializeField] private Image background;
    [SerializeField] private TMP_Text label;

    private readonly Color selectedFill =
        new Color32(232, 183, 90, 255);

    private readonly Color normalFill =
        new Color32(52, 76, 89, 255);

    private readonly Color selectedText =
        new Color32(24, 43, 54, 255);

    private readonly Color normalText =
        new Color32(243, 240, 232, 255);

    private void Awake()
    {
        if (difficultyToggle == null)
            difficultyToggle = GetComponent<Toggle>();

        Refresh(difficultyToggle.isOn);
    }

    public void OnChanged(bool isOn)
    {
        Refresh(isOn);
    }

    private void Refresh(bool isOn)
    {
        if (background != null)
            background.color = isOn ? selectedFill : normalFill;

        if (label != null)
            label.color = isOn ? selectedText : normalText;
    }
}
