
using System;
using System.Collections;
using UnityEngine;

public static class Ease
{
    public static float Linear(float t)
    {
        return t;
    }

    public static float OutQuad(float t)
    {
        return 1f - (1f - t) * (1f - t);
    }

    public static float InCubic(float t)
    {
        return t * t * t;
    }

    public static float OutCubic(float t)
    {
        return 1f - Mathf.Pow(1f - t, 3f);
    }

    public static float InOutQuad(float t)
    {
        return t < 0.5f
            ? 2f * t * t
            : 1f - Mathf.Pow(-2f * t + 2f, 2f) / 2f;
    }

    public static float OutBack(float t)
    {
        const float c1 = 1.70158f;
        const float c3 = c1 + 1f;

        return 1f + c3 * Mathf.Pow(t - 1f, 3f)
            + c1 * Mathf.Pow(t - 1f, 2f);
    }
}

public static class Tween
{
    public static Coroutine Run(
        MonoBehaviour host,
        float duration,
        Func<float, float> ease,
        Action<float> apply,
        Action done = null,
        float delay = 0f)
    {
        return host.StartCoroutine(
            Routine(duration, ease, apply, done, delay)
        );
    }

    static IEnumerator Routine(
        float duration,
        Func<float, float> ease,
        Action<float> apply,
        Action done,
        float delay)
    {
        float wait = delay;

        while (wait > 0f)
        {
            wait -= Time.unscaledDeltaTime;
            yield return null;
        }

        float time = 0f;

        while (time < duration)
        {
            time += Time.unscaledDeltaTime;

            apply(ease(Mathf.Clamp01(time / duration)));

            yield return null;
        }

        apply(ease(1f));

        done?.Invoke();
    }

    public static CanvasGroup Group(Component c)
    {
        CanvasGroup g = c.GetComponent<CanvasGroup>();

        if (g == null)
        {
            g = c.gameObject.AddComponent<CanvasGroup>();
        }

        return g;
    }

    public static Coroutine Fade(
        MonoBehaviour host,
        CanvasGroup g,
        float to,
        float duration,
        Func<float, float> ease,
        float delay = 0f,
        Action done = null)
    {
        float from = g.alpha;

        return Run(
            host,
            duration,
            ease,
            t => g.alpha = Mathf.Clamp01(
                Mathf.LerpUnclamped(from, to, t)
            ),
            done,
            delay
        );
    }

    public static Coroutine Scale(
        MonoBehaviour host,
        Transform tr,
        float to,
        float duration,
        Func<float, float> ease,
        float delay = 0f,
        Action done = null)
    {
        Vector3 from = tr.localScale;
        Vector3 end = Vector3.one * to;

        return Run(
            host,
            duration,
            ease,
            t => tr.localScale = Vector3.LerpUnclamped(
                from, end, t
            ),
            done,
            delay
        );
    }

    public static Coroutine Reveal(
        MonoBehaviour host,
        RectTransform rt,
        Vector2 offset,
        float duration,
        float delay,
        Func<float, float> ease)
    {
        CanvasGroup g = Group(rt);

        Vector2 end = rt.anchoredPosition;
        Vector2 start = end + offset;

        g.alpha = 0f;
        rt.anchoredPosition = start;

        return Run(
            host,
            duration,
            ease,
            t =>
            {
                g.alpha = Mathf.Clamp01(t);

                rt.anchoredPosition =
                    Vector2.LerpUnclamped(start, end, t);
            },
            null,
            delay
        );
    }

    public static Coroutine Punch(
        MonoBehaviour host,
        Transform tr,
        float amount = 0.2f,
        float duration = 0.25f)
    {
        return Run(
            host,
            duration,
            Ease.Linear,
            t => tr.localScale = Vector3.one *
                (1f + amount * Mathf.Sin(t * Mathf.PI))
        );
    }
}
