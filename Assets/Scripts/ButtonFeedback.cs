
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonFeedback : MonoBehaviour,
    IPointerDownHandler,
    IPointerUpHandler,
    IPointerExitHandler,
    ISelectHandler,
    IDeselectHandler
{
    [SerializeField] GameObject selectFrame;

    Coroutine c;

    void Go(
        float to,
        float d,
        System.Func<float, float> e)
    {
        if (c != null)
        {
            StopCoroutine(c);
        }

        c = Tween.Scale(this, transform, to, d, e);
    }

    public void OnPointerDown(PointerEventData _)
    {
        Go(0.94f, 0.08f, Ease.OutQuad);
    }

    public void OnPointerUp(PointerEventData _)
    {
        Go(1f, 0.18f, Ease.OutBack);
    }

    public void OnPointerExit(PointerEventData _)
    {
        Go(1f, 0.10f, Ease.OutQuad);
    }

    public void OnSelect(BaseEventData _)
    {
        if (selectFrame)
        {
            selectFrame.SetActive(true);
        }
    }

    public void OnDeselect(BaseEventData _)
    {
        if (selectFrame)
        {
            selectFrame.SetActive(false);
        }
    }

    void OnDisable()
    {
        transform.localScale = Vector3.one;

        if (selectFrame)
        {
            selectFrame.SetActive(false);
        }
    }
}
