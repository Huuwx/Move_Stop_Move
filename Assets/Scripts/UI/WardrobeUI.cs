using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WardrobeUI : MonoBehaviour
{
    [Header("Refs")]
    public WardrobeDatabase database;
    public WardrobeManager manager;

    [Header("UI")] 
    [SerializeField] private TextMeshProUGUI priceTxt;
    [SerializeField] private TextMeshProUGUI equipTxt;
    [SerializeField] private Button buySkinButton;
    [SerializeField] private Button equipSKinButton;
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
        if (slot.item.unlocked)
        {
            buySkinButton.gameObject.gameObject.SetActive(false);
            equipSKinButton.gameObject.gameObject.SetActive(true);
            
            if (slot.item.equipped)
            {
                equipTxt.text = "Equipped";
                equipSKinButton.interactable = false; // Vô hiệu hóa nút nếu đã mặc
            }
            else
            {
                equipTxt.text = "Equip";
                equipSKinButton.interactable = true; // Kích hoạt nút nếu chưa mặc
            }
        }
        else
        {
            buySkinButton.gameObject.gameObject.SetActive(true);
            equipSKinButton.gameObject.gameObject.SetActive(false);
            
            priceTxt.text = slot.item.price.ToString();
        }
        
        // Equip lên player
        manager.Equip(slot.item);

        // Highlight ô đã chọn
        foreach (var s in _slots) s.SetSelected(s == slot);
    }
    
    public void OnClickBuySkin(ItemSlotUI slot)
    {
        if (GameController.Instance.GetData().GetCurrentCoin() >= slot.item.price)
        {
            GameController.Instance.GetData().
                SetCurrentCoin(GameController.Instance.GetData().GetCurrentCoin() - slot.item.price);
            slot.item.unlocked = true;
            //GameController.Instance.GetData().AddKeyValue(slot.item.category.ToString(), slot.item.id);
            GameController.Instance.SaveData();
            
            // Cập nhật UI
            OnClickItem(slot);
        }
        else
        {
            Debug.Log("Not enough coins to buy this skin!");
        }
    }
    
    public void OnClickEquipSkin(ItemSlotUI slot)
    {
        // Gọi hàm Equip trong WardrobeManager
        manager.Equip(slot.item);
        
        // Cập nhật trạng thái nút
        equipSKinButton.interactable = false;
        equipTxt.text = "Equipped";
        
        // Cập nhật UI
        OnClickItem(slot);
        
        GameController.Instance.SaveData();
    }
}