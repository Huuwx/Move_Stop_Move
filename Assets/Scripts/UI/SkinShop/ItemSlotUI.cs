using UnityEngine;
using UnityEngine.UI;

public class ItemSlotUI : MonoBehaviour
{
    public Image imgIcon;
    public RawImage rawIcon;
    public GameObject selectedFrame;
    [HideInInspector] public ClothingItem item;
    [HideInInspector] public OutfitSet outfitSet;
    WardrobeUI _ui;

    public void Setup(ClothingItem it, WardrobeUI ui)
    {
        item = it; _ui = ui;
        if (rawIcon) rawIcon.texture = it.icon;
        imgIcon.gameObject.SetActive(false);
        SetSelected(false);
    }
    
    public void Setup(OutfitSet it, WardrobeUI ui)
    {
        outfitSet = it; _ui = ui;
        if (imgIcon) imgIcon.sprite = it.icon;
        rawIcon.gameObject.SetActive(false);
        SetSelected(false);
    }

    public void OnClick()
    {
        _ui.OnClickItem(this);
    }

    public void SetSelected(bool on)
    {
        if (selectedFrame) {selectedFrame.SetActive(on);}
    }
}