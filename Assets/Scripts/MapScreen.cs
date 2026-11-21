
using TMPro;
using UnityEngine;

public class MapScreen : MonoBehaviour
{
    [SerializeField] TMP_Text info;

    [SerializeField] RectTransform[] markers;

    [SerializeField]
    string[] missionNames =
    {
        "Abandoned Gas Station",
        "Quiet Suburb",
        "Dead Hospital"
    };

    void OnEnable()
    {
        for (int i = 0; i < markers.Length; i++)
        {
            markers[i].localScale = Vector3.zero;

            Tween.Scale(
                this,
                markers[i],
                1f,
                0.30f,
                Ease.OutBack,
                0.10f * i
            );
        }

        Select(GameSession.Mission);
    }

    public void Select(int mission)
    {
        if (mission < 1 || mission > missionNames.Length)
            return;

        GameSession.Mission = mission;

        info.text = "Mission " + mission + ": "
            + missionNames[mission - 1];

        CanvasGroup g = Tween.Group(info.transform);

        g.alpha = 0f;

        Tween.Fade(
            this,
            g,
            1f,
            0.15f,
            Ease.OutQuad
        );

        for (int i = 0; i < markers.Length; i++)
        {
            Tween.Scale(
                this,
                markers[i],
                i == mission - 1 ? 1.25f : 1f,
                0.20f,
                Ease.OutBack,
                0.35f
            );
        }
    }
}
