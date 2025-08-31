using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponSkinListUI : MonoBehaviour
{
    [SerializeField] WeaponSkinSelector selector;
    [SerializeField] Transform gridRoot;
    [SerializeField] WeaponSkinItemUI itemPrefab; // prefab UI có Image + Button

    List<WeaponSkinItemUI> items = new();

    public void Build(WeaponData weapon)
    {
        if(!weapon || !weapon.isPurchased) return;
        
        // clear cũ
        foreach (var it in items) Destroy(it.gameObject);
        items.Clear();

        var db = weapon.skins;
        if (!db || db.skins == null) return;
        
        foreach (var skin in db.skins)
        {
            if (!skin.isCustom)
            {
                var ui = Instantiate(itemPrefab, gridRoot);
                bool locked = false; // hoặc đọc từ dữ liệu mua/bỏ khóa
                bool selected = (skin.id == WeaponSkinSave.LoadSelected(weapon.id, weapon.selectedSkinId));
                ui.Bind(
                    skin.displayName,
                    skin.preview,
                    click: () => UpdateSelection(skin.id),
                    locked: locked,
                    selected: selected,
                    id: skin.id
                );
                items.Add(ui);
            }
            else
            {
                //Thêm ô Custom
                var custom = Instantiate(itemPrefab, gridRoot);
                bool customSelected = WeaponSkinSave.LoadSelected(weapon.id, weapon.selectedSkinId) == "custom";
                custom.Bind(
                    skin.displayName,
                    skin.preview,
                    click: () => UpdateCustomSelection(),
                    locked: false,
                    selected: customSelected,
                    id: "custom"
                );
                items.Add(custom);
            }

            
        }
    }
    
    public void UpdateSelection(string selectedId)
    {
        selector.SelectPreset(selectedId);
        foreach (var it in items)
        {
            it.SetSelected(it.GetSkinId() == selectedId);
        }
    }

    public void UpdateCustomSelection()
    {
        selector.SelectCustom();
        foreach (var it in items)
        {
            it.SetSelected(it.GetSkinId() == "custom");
        }
    }
}

