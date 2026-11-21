
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.EventSystems;

//public class MenuNavigator : MonoBehaviour
//{
//    [SerializeField] GameObject[] screens;

//    [SerializeField] GameObject[] firstButtons;

//    readonly Stack<int> history = new Stack<int>();

//    int current = -1;

//    Coroutine running;

//    void Start()
//    {
//        Show(0, 0);
//    }

//    public void Go(int index)
//    {
//        if (index < 0 || index >= screens.Length)
//            return;

//        if (current == index)
//            return;

//        history.Push(current);

//        Show(index, +1);
//    }

//    public void Back()
//    {
//        if (history.Count > 0)
//        {
//            Show(history.Pop(), -1);
//        }
//    }

//    void Show(int index, int direction)
//    {
//        if (running != null)
//        {
//            StopCoroutine(running);
//        }

//        for (int i = 0; i < screens.Length; i++)
//        {
//            screens[i].SetActive(i == index);
//        }

//        current = index;

//        GameObject s = screens[index];

//        CanvasGroup cg = Tween.Group(s.transform);

//        RectTransform rt = (RectTransform)s.transform;

//        if (direction == 0)
//        {
//            cg.alpha = 1f;
//            cg.interactable = true;
//            cg.blocksRaycasts = true;

//            rt.anchoredPosition = Vector2.zero;

//            EventSystem.current.SetSelectedGameObject(
//                firstButtons[index]
//            );

//            return;
//        }

//        cg.alpha = 0f;
//        cg.interactable = false;
//        cg.blocksRaycasts = false;

//        float startX = 120f * direction;

//        running = Tween.Run(
//            this,
//            0.30f,
//            Ease.OutCubic,
//            t =>
//            {
//                cg.alpha = Mathf.Clamp01(t);

//                rt.anchoredPosition = new Vector2(
//                    Mathf.LerpUnclamped(startX, 0f, t),
//                    0f
//                );
//            },
//            () =>
//            {
//                cg.interactable = true;
//                cg.blocksRaycasts = true;

//                EventSystem.current.SetSelectedGameObject(
//                    firstButtons[index]
//                );
//            }
//        );
//    }

//    void Update()
//    {
//        if (Input.GetKeyDown(KeyCode.Escape))
//        {
//            Back();
//        }
//    }
//}

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
public class MenuNavigator : MonoBehaviour
{
    [SerializeField] GameObject[] screens;

    [SerializeField] GameObject[] firstButtons;

    readonly Stack<int> history = new Stack<int>();

    int current = -1;

    Coroutine running;

    void Start()
    {
        Show(0, 0);
    }

    public void Go(int index)
    {
        if (index < 0 || index >= screens.Length)
            return;

        if (current == index)
            return;

        history.Push(current);

        Show(index, +1);
    }
    public void LoadGameScene(string sceneName)
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(sceneName);
    }
    public void Back()
    {
        if (history.Count > 0)
        {
            Show(history.Pop(), -1);
        }
    }

    void Show(int index, int direction)
    {
        if (running != null)
        {
            StopCoroutine(running);
        }

        for (int i = 0; i < screens.Length; i++)
        {
            screens[i].SetActive(i == index);
        }

        current = index;

        GameObject s = screens[index];

        CanvasGroup cg = Tween.Group(s.transform);

        RectTransform rt = (RectTransform)s.transform;

        if (direction == 0)
        {
            cg.alpha = 1f;
            cg.interactable = true;
            cg.blocksRaycasts = true;

            rt.anchoredPosition = Vector2.zero;

            EventSystem.current.SetSelectedGameObject(
                firstButtons[index]
            );

            return;
        }

        cg.alpha = 0f;
        cg.interactable = false;
        cg.blocksRaycasts = false;

        float startX = 120f * direction;

        running = Tween.Run(
            this,
            0.30f,
            Ease.OutCubic,
            t =>
            {
                cg.alpha = Mathf.Clamp01(t);

                rt.anchoredPosition = new Vector2(
                    Mathf.LerpUnclamped(startX, 0f, t),
                    0f
                );
            },
            () =>
            {
                cg.interactable = true;
                cg.blocksRaycasts = true;

                EventSystem.current.SetSelectedGameObject(
                    firstButtons[index]
                );
            }
        );
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Back();
        }
    }
}
