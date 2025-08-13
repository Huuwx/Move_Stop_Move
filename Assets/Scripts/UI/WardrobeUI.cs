using System.Collections.Generic;
using UnityEngine;

public class WardrobeUI : MonoBehaviour
{
    [Header("Refs")]
    public WardrobeDatabase database;
    public WardrobeManager manager;

    [Header("UI")]
    public Transform gridParent;   // Content có GridLayoutGroup
    public ItemSlotUI slotPrefab;
    public OutfitCategory defaultCategory = OutfitCategory.Hat;

    OutfitCategory _current;
    readonly List<ItemSlotUI> _slots = new();

    void Start()
    {
        ShowCategory(defaultCategory);
    }

    // Gán hàm này cho 4 nút category (tham số 0..3)
    public void OnClickCategory(int index)
    {
        ShowCategory((OutfitCategory)index);
    }

    public void ShowCategory(OutfitCategory cat)
    {
        _current = cat;

        // Xóa slot cũ
        foreach (var s in _slots) Destroy(s.gameObject);
        _slots.Clear();

        // Tạo slot mới theo category
        var list = database.GetByCategory(cat);
        foreach (var item in list)
        {
            var slot = Instantiate(slotPrefab, gridParent);
            slot.Setup(item, this);
            _slots.Add(slot);
        }

        // Bỏ highlight tất cả
        foreach (var s in _slots) s.SetSelected(false);
    }

    public void OnClickItem(ItemSlotUI slot)
    {
        // Equip lên player
        manager.Equip(slot.item);

        // Highlight ô đã chọn
        foreach (var s in _slots) s.SetSelected(s == slot);
    }
}