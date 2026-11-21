using TMPro;
using UnityEngine;

public class ScoreText : MonoBehaviour
{
    [SerializeField] TMP_Text label;
    public void Show(int score, int target) => label.SetText("Score: {0} / {1}", score, target);
}
