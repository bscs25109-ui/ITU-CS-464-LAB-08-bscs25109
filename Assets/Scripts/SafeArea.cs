
using UnityEngine;

public class SafeArea : MonoBehaviour
{
    void Start()
    {
        RectTransform r = (RectTransform)transform;

        Rect s = Screen.safeArea;

        r.anchorMin = new Vector2(
            s.xMin / Screen.width,
            s.yMin / Screen.height
        );

        r.anchorMax = new Vector2(
            s.xMax / Screen.width,
            s.yMax / Screen.height
        );

        r.offsetMin = r.offsetMax = Vector2.zero;
    }
}
