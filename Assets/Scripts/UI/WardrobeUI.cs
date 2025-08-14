using System.Collections.Generic;
using UnityEngine;

public class WardrobeUI : MonoBehaviour
{
    [Header("Refs")]
    public WardrobeDatabase database;
    public WardrobeManager manager;

    [Header("UI")]
    public List<Transform> gridParents;   // Content có GridLayoutGroup
    public ItemSlotUI slotPrefab;
    [SerializeField] List<CategoryBtn> categoryButtons; // Các nút category
    public OutfitCategory defaultCategory = OutfitCategory.Hat;

    OutfitCategory _current;
    readonly List<ItemSlotUI> _slots = new();

    void Start()
    {
        ShowCategory(defaultCategory);
        categoryButtons[0].GetComponent<UnityEngine.UI.Button>().interactable = false;
    }

    public void ShowCategory(OutfitCategory cat)
    {
        _current = cat;

        foreach (var item in categoryButtons)
        {
            if(item.category == cat)
            {
                item.GetComponent<UnityEngine.UI.Button>().interactable = false; // Vô hiệu hóa nút đang chọn
            }
            else
            {
                item.GetComponent<UnityEngine.UI.Button>().interactable = true; // Kích hoạt các nút khác
            }
        }
        
        // Xóa slot cũ
        foreach (var s in _slots) Destroy(s.gameObject);
        _slots.Clear();

        // Tạo slot mới theo category
        var list = database.GetByCategory(cat);

        int index = 0;
        int count = 0;
        foreach (var item in list)
        {
            if(count % 3 == 0 && count > 0) 
            {
                index++;
                count = 0;
            }
            var slot = Instantiate(slotPrefab, gridParents[index]);
            slot.Setup(item, this);
            _slots.Add(slot);
            count++;
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