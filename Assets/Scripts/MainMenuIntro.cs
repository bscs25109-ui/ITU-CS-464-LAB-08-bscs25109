
using UnityEngine;

public class MainMenuIntro : MonoBehaviour
{
    [SerializeField] RectTransform title;
    [SerializeField] RectTransform subtitle;

    [SerializeField] RectTransform[] buttons;

    static bool played;

    void OnEnable()
    {
        if (played)
            return;

        played = true;

        Tween.Reveal(
            this,
            title,
            new Vector2(0, 60),
            0.45f,
            0f,
            Ease.OutCubic
        );

        Tween.Reveal(
            this,
            subtitle,
            new Vector2(0, 60),
            0.45f,
            0.10f,
            Ease.OutCubic
        );

        for (int i = 0; i < buttons.Length; i++)
        {
            Tween.Reveal(
                this,
                buttons[i],
                new Vector2(0, -80),
                0.35f,
                0.30f + 0.07f * i,
                Ease.OutCubic
            );
        }
    }
}
