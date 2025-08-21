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
    [SerializeField] private GameObject descriptionPanel;
    [SerializeField] private TextMeshProUGUI descriptionTxt;
    [SerializeField] private TextMeshProUGUI priceTxt;
    [SerializeField] private TextMeshProUGUI equipTxt;
    [SerializeField] private Button buySkinButton;
    [SerializeField] private Button equipSKinButton;
    [SerializeField] private Button watchAdsButton;
    [SerializeField] Transform contents;
    [SerializeField] GameObject columnPrefab; // Prefab của cột chứa các item
    [SerializeField] ItemSlotUI slotPrefab;
    [SerializeField] List<CategoryBtn> categoryButtons; // Các nút category
    public OutfitCategory defaultCategory = OutfitCategory.Hat;

    OutfitCategory _current;
    readonly List<ItemSlotUI> _slots = new();
    List<GameObject> columns = new List<GameObject>();
    private ItemSlotUI _currentItem;
    private GameObject columnItem;

    void OnEnable()
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
        
        // Xóa cột cũ
        foreach (var c in columns) Destroy(c);
        columns.Clear();
        
        descriptionPanel.SetActive(false);
        
        // Tạo slot mới theo category
        if (cat != OutfitCategory.OutfitSet)
        {
            var list = database.GetByCategory(cat);
            int count = -1;
            foreach (var item in list)
            {
                if(count == 2 || count == -1) 
                {
                    count = 0;
                    columnItem = Instantiate(columnPrefab, contents);
                    columns.Add(columnItem);
                }
                var slot = Instantiate(slotPrefab, columnItem.transform);
                slot.Setup(item, this);
                _slots.Add(slot);
                count++;
            }
        }
        else
        {
            var list = database.outfitSets;
            int count = -1;
            foreach (var item in list)
            {
                if(count == 2 || count == -1) 
                {
                    count = 0;
                    columnItem = Instantiate(columnPrefab, contents);
                    columns.Add(columnItem);
                }
                var slot = Instantiate(slotPrefab, columnItem.transform);
                slot.Setup(item, this);
                _slots.Add(slot);
                count++;
            }
        }
        
        // Cập nhật UI
        buySkinButton.gameObject.gameObject.SetActive(false);
        equipSKinButton.gameObject.gameObject.SetActive(false);
        watchAdsButton.gameObject.gameObject.SetActive(false);

        // Bỏ highlight tất cả
        foreach (var s in _slots) s.SetSelected(false);
    }

    public void OnClickItem(ItemSlotUI slot)
    {
        _currentItem = slot;
        
        descriptionPanel.SetActive(true);
        
        //Cập nhật mô tả
        if (slot.outfitSet)
        {
            descriptionTxt.text = slot.outfitSet.upgradeIndex.ToString() + "% Range";
            
            if (slot.outfitSet.unlocked)
            {
                watchAdsButton.gameObject.gameObject.SetActive(false);
                buySkinButton.gameObject.gameObject.SetActive(false);
                equipSKinButton.gameObject.gameObject.SetActive(true);
            
                if (slot.outfitSet.equipped)
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
                watchAdsButton.gameObject.gameObject.SetActive(true);
                buySkinButton.gameObject.gameObject.SetActive(true);
                equipSKinButton.gameObject.gameObject.SetActive(false);
            
                priceTxt.text = slot.outfitSet.price.ToString();
            }
        
            // Equip lên player
            manager.EquipOutfit(slot.outfitSet);
        }
        else
        {
            descriptionTxt.text = slot.item.upgradeIndex.ToString() + "% Range";
            
            if (slot.item.unlocked)
            {
                watchAdsButton.gameObject.gameObject.SetActive(false);
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
                watchAdsButton.gameObject.gameObject.SetActive(true);
                buySkinButton.gameObject.gameObject.SetActive(true);
                equipSKinButton.gameObject.gameObject.SetActive(false);
            
                priceTxt.text = slot.item.price.ToString();
            }
        
            // Equip lên player
            manager.Equip(slot.item);
        }

        // Highlight ô đã chọn
        foreach (var s in _slots) s.SetSelected(s == slot);

        foreach (var s in _slots)
        {
            if (s == slot)
            {
                s.GetComponent<UnityEngine.UI.Button>().interactable = false;
            }
            else
            {
                s.GetComponent<UnityEngine.UI.Button>().interactable = true;
            }
        }
    }
    
    public void OnClickBuySkin()
    {
        if (_currentItem.item)
        {
            if (GameController.Instance.GetData().GetCurrentCoin() >= _currentItem.item.price)
            {
                GameController.Instance.GetData()
                    .SetCurrentCoin(GameController.Instance.GetData().GetCurrentCoin() - _currentItem.item.price);
                _currentItem.item.unlocked = true;
            }
            else
            {
                Debug.Log("Not enough coins to buy this skin!");
            }
        }
        else
        {
            if (GameController.Instance.GetData().GetCurrentCoin() >= _currentItem.outfitSet.price)
            {
                GameController.Instance.GetData()
                    .SetCurrentCoin(GameController.Instance.GetData().GetCurrentCoin() - _currentItem.outfitSet.price);
                _currentItem.outfitSet.unlocked = true;
            }
            else
            {
                Debug.Log("Not enough coins to buy this skin!");
            }
        }
        
        // Cập nhật UI
        OnClickItem(_currentItem);
        GameController.Instance.SaveData();
    }

    public void OnClickWatchAds()
    {
        if (_currentItem.item)
        {
            _currentItem.item.unlocked = true;
        }
        else
        {
            _currentItem.outfitSet.unlocked = true;
        }

        GameController.Instance.SaveData();
        // Cập nhật UI
        OnClickItem(_currentItem);
    }
    
    public void OnClickEquipSkin()
    {
        if (_currentItem.item)
        {
            // Gọi hàm Equip trong WardrobeManager
            manager.Equip(_currentItem.item);

            // Cập nhật trạng thái nút
            equipSKinButton.interactable = false;
            equipTxt.text = "Equipped";

            // Cập nhật trạng thái item
            var list = database.GetByCategory(_currentItem.item.category);
            foreach (var item in list)
            {
                if (item.id == _currentItem.item.id)
                {
                    item.equipped = true; // Đánh dấu là đã mặc
                }
                else
                {
                    item.equipped = false; // Đánh dấu là chưa mặc
                }
            }
            
            var listos = database.outfitSets;
            foreach (var item in listos)
            {
                item.equipped = false; // Đánh dấu là chưa mặc
            }

            GameController.Instance.GetData().AddKeyValue(OutfitCategory.OutfitSet.ToString(), null);
            GameController.Instance.GetData().AddKeyValue(_currentItem.item.category.ToString(), _currentItem.item.id);
        }
        else
        {
             // Gọi hàm Equip trong WardrobeManager
            manager.EquipOutfit(_currentItem.outfitSet);

            // Cập nhật trạng thái nút
            equipSKinButton.interactable = false;
            equipTxt.text = "Equipped";

            // Cập nhật trạng thái item
            var list = database.items;
            foreach (var item in list)
            {
                item.equipped = false; // Đánh dấu là chưa mặc
            }

            var listos = database.outfitSets;
            foreach (var item in listos)
            {
                if (item.id == _currentItem.outfitSet.id)
                {
                    item.equipped = true; // Đánh dấu là đã mặc
                }
                else
                {
                    item.equipped = false; // Đánh dấu là chưa mặc
                }
            }
            
            GameController.Instance.GetData().AddKeyValue(OutfitCategory.Hat.ToString(), null);
            GameController.Instance.GetData().AddKeyValue(OutfitCategory.Pants.ToString(), null);
            GameController.Instance.GetData().AddKeyValue(OutfitCategory.Shield.ToString(), null);
            GameController.Instance.GetData().AddKeyValue(OutfitCategory.OutfitSet.ToString(), _currentItem.outfitSet.id);
        }
        // Cập nhật UI
        OnClickItem(_currentItem);
        GameController.Instance.SaveData();
    }
}