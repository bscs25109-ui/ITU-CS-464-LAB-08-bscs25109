using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingScreen : MonoBehaviour
{
    [SerializeField] Image fill;                    // Image Type: Filled, Horizontal, Origin Left
    [SerializeField] TMP_Text percentText, statusText, tipText, missionText;
    [SerializeField] CanvasGroup content;
    [SerializeField] float fillSpeed = 0.8f;        // the bar moves at most this much (0-1) per second

    static readonly string[] Missions = { "GAS STATION", "QUIET SUBURB", "DEAD HOSPITAL" };
    static readonly string[] Tips =
    {
        "TIP: WASD or the arrow keys move the survivor.",
        "TIP: Press SPACE (or the HIT button) to push zombies away.",
        "TIP: Collect 5 scrap to win. Medkits heal you.",
        "TIP: Press ESC to pause. Music and menu sounds keep playing.",
        "TIP: Low health changes the music to the combat track."
    };

    IEnumerator Start()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;

        int m = Mathf.Clamp(GameSession.Mission, 1, Missions.Length);
        missionText.text = "MISSION " + m + ": " + Missions[m - 1];
        statusText.text = "Loading...";
        fill.fillAmount = 0f;
        percentText.SetText("0%");

        content.alpha = 0f;
        Tween.Fade(this, content, 1f, 0.3f, Ease.OutQuad);
        StartCoroutine(RotateTips());

        // Load the next scene in the background. It will NOT start until we allow it.
        AsyncOperation op = SceneManager.LoadSceneAsync(SceneLoader.Target);
        op.allowSceneActivation = false;

        float shown = 0f;
        while (shown < 1f)
        {
            float real = Mathf.Clamp01(op.progress / 0.9f);                         // 0..0.9 -> 0..1
            shown = Mathf.MoveTowards(shown, real, fillSpeed * Time.unscaledDeltaTime);
            fill.fillAmount = shown;
            percentText.SetText("{0}%", Mathf.RoundToInt(shown * 100f));
            yield return null;
        }

        statusText.text = "READY";
        AudioManager.UI(UISound.Success);
        yield return new WaitForSecondsRealtime(0.5f);
        op.allowSceneActivation = true;             // only now does the Game scene start
    }

    IEnumerator RotateTips()
    {
        int i = Random.Range(0, Tips.Length);
        while (true)
        {
            tipText.text = Tips[i];
            CanvasGroup g = Tween.Group(tipText.transform);
            g.alpha = 0f;
            Tween.Fade(this, g, 1f, 0.3f, Ease.OutQuad);
            yield return new WaitForSecondsRealtime(2.5f);
            i = (i + 1) % Tips.Length;
        }
    }
}