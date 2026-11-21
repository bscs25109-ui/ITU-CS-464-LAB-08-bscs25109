using UnityEngine;
using UnityEngine.UI;

// Put this on a Canvas. At start it gives every Button (even hidden ones) a UIButtonSound.
public class UIButtonSoundInstaller : MonoBehaviour
{
    void Awake()
    {
        foreach (Button b in GetComponentsInChildren<Button>(true))
            if (b.GetComponent<UIButtonSound>() == null)
                b.gameObject.AddComponent<UIButtonSound>();
    }
}