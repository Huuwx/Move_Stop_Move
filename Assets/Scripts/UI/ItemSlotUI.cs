using UnityEngine;
using UnityEngine.UI;

public class ItemSlotUI : MonoBehaviour
{
    public Image icon;
    public GameObject selectedFrame;
    [HideInInspector] public ClothingItem item;
    WardrobeUI _ui;

    public void Setup(ClothingItem it, WardrobeUI ui)
    {
        item = it; _ui = ui;
        if (icon) icon.sprite = it.icon;
        SetSelected(false);
    }

    public void OnClick()
    {
        _ui.OnClickItem(this);
    }

    public void SetSelected(bool on)
    {
        if (selectedFrame) selectedFrame.SetActive(on);
    }
}