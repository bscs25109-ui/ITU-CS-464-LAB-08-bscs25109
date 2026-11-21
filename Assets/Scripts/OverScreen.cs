using System;
using TMPro;
using UnityEngine;

public class OverScreen : MonoBehaviour
{
    [SerializeField] TMP_Text title, scoreText, iconText, hintText;
    [SerializeField] CanvasGroup dim, panelGroup;
    [SerializeField] RectTransform panel;

    static readonly Color32 Teal = new Color32(0x4F, 0xB3, 0xA5, 255);
    static readonly Color32 Alert = new Color32(0xEE, 0x7D, 0x75, 255);

    bool victory; int score, target;

    public void Show(bool victory, int score, int target)    // called by GameUI before the panel is activated
    {
        this.victory = victory; this.score = score; this.target = target;
        title.text = victory ? "SURVIVED!" : "YOU DIED";
        title.color = victory ? Teal : Alert;
        iconText.text = victory ? "OK" : "X";
        iconText.color = title.color;
        hintText.text = victory ? "The area is safe. Well done, survivor."
                                 : "The dead got you. Try again.";
        scoreText.SetText("Score: 0 / {0}", target);
    }

    void OnEnable()                                           // plays when GameUI shows the panel
    {
        dim.alpha = 0f; panelGroup.alpha = 0f;
        Tween.Fade(this, dim, 1f, 0.30f, Ease.OutQuad);
        Tween.Fade(this, panelGroup, 1f, 0.30f, Ease.OutQuad);

        // Victory rises with a bounce; defeat sinks slowly and heavily.
        Vector2 start = victory ? new Vector2(0, -150) : new Vector2(0, 150);
        Func<float, float> ease;
        if (victory) ease = Ease.OutBack; else ease = Ease.InOutQuad;
        Tween.Run(this, victory ? 0.45f : 0.60f, ease,
            t => panel.anchoredPosition = Vector2.LerpUnclamped(start, Vector2.zero, t));

        title.transform.localScale = Vector3.one * 0.6f;
        Tween.Scale(this, title.transform, 1f, 0.40f, Ease.OutBack, 0.25f);

        Tween.Run(this, 0.8f, Ease.OutCubic,                  // score counts up
            t => scoreText.SetText("Score: {0} / {1}", Mathf.RoundToInt(score * t), target),
            null, 0.35f);
    }
}
