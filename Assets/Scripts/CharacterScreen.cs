using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterScreen : MonoBehaviour
{
    [Header("Player Name")]
    [SerializeField] private TMP_Text nameText;

    [Header("Stat Rows")]
    [SerializeField] private RectTransform[] rows;

    [Header("Stat Bars")]
    [SerializeField] private Image[] bars;

    [Header("Stat Values")]
    [SerializeField] private TMP_Text[] valueTexts;

    private readonly int[] values = { 80, 60, 30 };

    private void OnEnable()
    {
        // Display the name from the New Game screen.
        if (nameText != null)
        {
            nameText.text = string.IsNullOrWhiteSpace(
                GameSession.PlayerName)
                ? "Survivor"
                : GameSession.PlayerName;
        }

        // Check that all three stat references exist.
        if (rows == null || bars == null || valueTexts == null ||
            rows.Length != 3 || bars.Length != 3 ||
            valueTexts.Length != 3)
        {
            Debug.LogError(
                "CharacterScreen: Assign all 3 rows, bars and values."
            );
            return;
        }

        for (int i = 0; i < 3; i++)
        {
            if (rows[i] == null ||
                bars[i] == null ||
                valueTexts[i] == null)
            {
                Debug.LogError(
                    "CharacterScreen: A stat reference is missing at index " + i
                );
                continue;
            }

            int n = values[i];

            Image bar = bars[i];
            TMP_Text txt = valueTexts[i];

            float delay = 0.12f * i;

            // Begin with an empty progress bar.
            bar.fillAmount = 0f;
            txt.SetText("0");

            // Reveal rows one by one.
            Tween.Reveal(
                this,
                rows[i],
                new Vector2(0, -30),
                0.25f,
                delay,
                Ease.OutCubic
            );

            // Animate bar and number together.
            Tween.Run(
                this,
                0.6f,
                Ease.OutCubic,
                t =>
                {
                    bar.fillAmount = (float)n / 100f * t;

                    txt.SetText(
                        "{0}",
                        Mathf.RoundToInt(n * t)
                    );
                },
                null,
                delay + 0.1f
            );
        }
    }
}