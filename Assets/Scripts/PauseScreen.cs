using System;
using UnityEngine;

public class PauseScreen : MonoBehaviour
{
    [SerializeField] CanvasGroup dim, panelGroup;
    [SerializeField] RectTransform panel;
    bool closing;

    void OnEnable()                                   // GameUI.SetState(Paused) activates this object
    {
        closing = false;
        panelGroup.interactable = true;
        dim.alpha = 0f; panelGroup.alpha = 0f;
        panel.localScale = Vector3.one * 0.9f;
        Tween.Fade(this, dim, 1f, 0.20f, Ease.OutQuad);
        Tween.Fade(this, panelGroup, 1f, 0.20f, Ease.OutQuad);
        Tween.Scale(this, panel, 1f, 0.28f, Ease.OutBack);
    }

    public void Close(Action done)
    {
        if (closing) return;                          // ignore a second Escape/tap while closing
        closing = true;
        panelGroup.interactable = false;
        Tween.Fade(this, dim, 0f, 0.15f, Ease.InCubic);
        Tween.Fade(this, panelGroup, 0f, 0.15f, Ease.InCubic);
        Tween.Scale(this, panel, 0.95f, 0.15f, Ease.InCubic, 0f, done);
    }
}
