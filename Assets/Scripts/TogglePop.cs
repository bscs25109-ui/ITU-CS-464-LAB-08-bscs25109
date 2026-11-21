
using UnityEngine;

public class TogglePop : MonoBehaviour
{
    private Coroutine running;

    public void OnChanged(bool isOn)
    {
        if (running != null)
            StopCoroutine(running);

        running = Tween.Scale(
            this,
            transform,
            isOn ? 1.08f : 1f,
            0.20f,
            Ease.OutBack
        );
    }
}
