using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] Image fill;            // Image Type: Filled, Horizontal, Origin Left
    [SerializeField] Image lag;             // the same, behind it, paler
    [SerializeField] TMPro.TMP_Text hpText; // 'HP 75/100' so the amount is not colour only
    [SerializeField] Gradient colour;       // red at 0, amber at 0.5, teal at 1
    [SerializeField] float lagDelay = 0.4f; // seconds before the pale bar catches up
    [SerializeField] float lagSpeed = 0.8f; // fill per second
    float target = 1f;
    float lagStartsAt;

    public void Set(int current, int max)
    {
        float next = Mathf.Clamp01((float)current / max);   // the cast matters
        if (next < target)
        {
            lagStartsAt = Time.unscaledTime + lagDelay;
            Tween.Punch(this, transform, 0.06f, 0.2f);      // the whole bar jolts when you are hit
        }
        else lag.fillAmount = next;                         // healing: lag moves up with it
        target = next;
        if (hpText) hpText.SetText("HP {0}/{1}", current, max);
        fill.fillAmount = target;
        fill.color = colour.Evaluate(target);
    }

    void Update()
    {
        if (Time.unscaledTime < lagStartsAt) return;
        lag.fillAmount = Mathf.MoveTowards(lag.fillAmount, target,
            lagSpeed * Time.unscaledDeltaTime);             // constant speed on purpose
    }
}
