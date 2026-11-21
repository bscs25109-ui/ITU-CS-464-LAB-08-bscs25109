using UnityEngine;
using UnityEngine.UI;

public class MissionSelect : MonoBehaviour
{
    [SerializeField] GameObject[] selectedFrames;   // one gold frame per card
    [SerializeField] RectTransform[] cards;         // the three cards
    [SerializeField] Button startButton;

    void OnEnable()
    {
        for (int i = 0; i < cards.Length; i++)      // cards fade in one after another
        {
            CanvasGroup g = Tween.Group(cards[i]);  // fade only: the layout controls position
            g.alpha = 0f;
            Tween.Fade(this, g, 1f, 0.25f, Ease.OutQuad, 0.08f * i);
        }
        Pick(GameSession.Mission);
    }

    public void Pick(int mission)                   // each card calls this with 1, 2 or 3
    {
        GameSession.Mission = mission;
        for (int i = 0; i < selectedFrames.Length; i++)
        {
            bool on = i == mission - 1;
            selectedFrames[i].SetActive(on);
            Tween.Scale(this, cards[i], on ? 1.04f : 1f, 0.20f, Ease.OutBack);
        }
        startButton.interactable = true;
    }
}