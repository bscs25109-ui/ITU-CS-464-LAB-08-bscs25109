using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WeaponScreen : MonoBehaviour
{
    [Header("3D previews (Pistol, Shotgun, Rifle)")]
    [SerializeField] GameObject[] models;

    [Header("Tabs (same order)")]
    [SerializeField] Image[] tabImages;
    [SerializeField] TMP_Text[] tabLabels;

    [Header("Stats (Damage, Fire rate, Range)")]
    [SerializeField] Image[] bars;
    [SerializeField] TMP_Text[] values;

    [Header("Texts")]
    [SerializeField] TMP_Text nameText;
    [SerializeField] TMP_Text equipText;

    static readonly Color32 Amber = new Color32(0xE8, 0xB7, 0x5A, 255);
    static readonly Color32 Steel = new Color32(0x34, 0x4C, 0x59, 255);
    static readonly Color32 Midnight = new Color32(0x18, 0x2B, 0x36, 255);
    static readonly Color32 Ivory = new Color32(0xF3, 0xF0, 0xE8, 255);

    readonly string[] names = { "PISTOL", "SHOTGUN", "RIFLE" };
    // damage, fire rate, range (0-100)
    readonly int[,] stats = { { 40, 70, 50 }, { 85, 30, 25 }, { 60, 80, 85 } };
    readonly Coroutine[] barTweens = new Coroutine[3];
    int pending;

    void OnEnable()
    {
        for (int b = 0; b < bars.Length; b++)        // bars start empty and grow
        {
            bars[b].fillAmount = 0f;
            values[b].SetText("0");
        }
        Show(GameSession.Weapon);                    // the equipped weapon is shown first
    }

    public void Show(int i)                          // each tab calls this with 0, 1 or 2
    {
        pending = i;
        nameText.text = names[i];
        equipText.text = GameSession.Weapon == i ? "EQUIPPED" : "EQUIP";

        for (int k = 0; k < models.Length; k++)
        {
            bool on = k == i;
            models[k].SetActive(on);
            tabImages[k].color = on ? Amber : Steel;       // selected: gold tab
            tabLabels[k].color = on ? Midnight : Ivory;    // selected: dark text
        }

        models[i].transform.localScale = Vector3.one * 0.8f;          // new weapon pops in
        Tween.Scale(this, models[i].transform, 1f, 0.25f, Ease.OutBack);

        for (int b = 0; b < bars.Length; b++)                          // bars and numbers glide together
        {
            if (barTweens[b] != null) StopCoroutine(barTweens[b]);
            Image bar = bars[b];
            TMP_Text val = values[b];
            float from = bar.fillAmount;
            float to = (float)stats[i, b] / 100f;
            barTweens[b] = Tween.Run(this, 0.35f, Ease.InOutQuad, t =>
            {
                float v = Mathf.Lerp(from, to, t);
                bar.fillAmount = v;
                val.SetText("{0}", Mathf.RoundToInt(v * 100f));
            });
        }
    }

    public void Equip()                              // Btn_Equip
    {
        GameSession.Weapon = pending;
        equipText.text = "EQUIPPED";
        Tween.Punch(this, equipText.transform, 0.2f, 0.25f);          // confirmation
    }
}