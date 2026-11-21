
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NewGameScreen : MonoBehaviour
{
    [Header("Player Information")]
    [SerializeField] private TMP_InputField nameField;

    [Header("Animation")]
    [SerializeField] private RectTransform formPanel;
    [SerializeField] private RectTransform nextButton;

    [Header("Difficulty")]
    [SerializeField] private Toggle easyToggle;
    [SerializeField] private Toggle normalToggle;
    [SerializeField] private Toggle hardToggle;

    private void OnEnable()
    {
        Tween.Reveal(
            this,
            formPanel,
            new Vector2(0, -50),
            0.35f,
            0.10f,
            Ease.OutCubic
        );

        Tween.Reveal(
            this,
            nextButton,
            new Vector2(0, -50),
            0.35f,
            0.22f,
            Ease.OutCubic
        );
    }

    public void Confirm()
    {
        GameSession.PlayerName =
            string.IsNullOrWhiteSpace(nameField.text)
            ? "Survivor"
            : nameField.text.Trim();

        if (easyToggle.isOn)
            GameSession.Difficulty = "Easy";

        else if (hardToggle.isOn)
            GameSession.Difficulty = "Hard";

        else
            GameSession.Difficulty = "Normal";

        Debug.Log(
            "Player: " + GameSession.PlayerName +
            " | Difficulty: " + GameSession.Difficulty
        );
    }
}
