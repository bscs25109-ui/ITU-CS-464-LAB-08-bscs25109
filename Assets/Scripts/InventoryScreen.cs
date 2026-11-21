using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryScreen : MonoBehaviour
{
    [Header("Detail panel")]
    [SerializeField] TMP_Text nameText, descText, countText;
    [SerializeField] Transform detailTexts;              // the 'Texts' object (fades when the item changes)

    [Header("Use button")]
    [SerializeField] Button useButton;
    [SerializeField] TMP_Text useLabel;

    [Header("The 4 filled slots (same order as the items)")]
    [SerializeField] RectTransform[] slots;
    [SerializeField] GameObject[] selectedFrames;        // each slot's SelectedFrame
    [SerializeField] TMP_Text[] slotCounts;              // each slot's Count text

    readonly string[] names = { "Medkit", "Ammo", "Canned Food", "Bandage" };
    readonly string[] descs = { "Restores health.", "Pistol rounds.",
                                "Restores stamina.", "Stops bleeding." };
    readonly int[] counts = { 3, 12, 2, 4 };
    int selected;

    void OnEnable()
    {
        for (int i = 0; i < slotCounts.Length; i++)
            slotCounts[i].SetText("x{0}", counts[i]);
        Select(selected);                                // remembers the last chosen item
    }

    public void Select(int i)                            // each slot calls this with 0, 1, 2 or 3
    {
        selected = i;
        nameText.text = names[i];
        descText.text = descs[i];
        countText.SetText("x{0}", counts[i]);
        RefreshUse();

        CanvasGroup g = Tween.Group(detailTexts);        // quick fade so the change is noticed
        g.alpha = 0f;
        Tween.Fade(this, g, 1f, 0.15f, Ease.OutQuad);

        for (int k = 0; k < slots.Length; k++)           // chosen slot: gold frame + grows
        {
            bool on = k == i;
            selectedFrames[k].SetActive(on);
            Tween.Scale(this, slots[k], on ? 1.08f : 1f, 0.18f, Ease.OutBack);
        }
    }

    public void Use()                                    // Btn_Use
    {
        if (counts[selected] <= 0) return;
        counts[selected]--;
        countText.SetText("x{0}", counts[selected]);
        slotCounts[selected].SetText("x{0}", counts[selected]);
        Tween.Punch(this, countText.transform, 0.3f, 0.25f);          // the number that changed pulses
        Tween.Punch(this, slotCounts[selected].transform, 0.3f, 0.25f);
        RefreshUse();
    }

    void RefreshUse()
    {
        bool has = counts[selected] > 0;
        useButton.interactable = has;
        useLabel.text = has ? "USE" : "EMPTY";           // word + colour, not colour only
    }
}