using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WeaponSkinSelector : MonoBehaviour
{
    [SerializeField] private WeaponData weaponData;           // vũ khí đang hiển thị trong UI
    [SerializeField] private WeaponSkinApplier applier;       // applier của model đang render ở UI (hoặc ở tay)
    [SerializeField] private WeaponSkinDatabase database;     // có thể lấy từ weaponData.skins
    [SerializeField] private GameObject customPalettePanel;
    [SerializeField] private RectTransform equipBtnReactTransform;
    [SerializeField] private Button equipButton;
    [SerializeField] private TextMeshProUGUI equipBtnText;
    [SerializeField] private List<WeaponSkinDatabase> allDatabases; // tất cả database (để truyền cho grid preset)

    private string currentSelectedSkinId;
    private bool isCustom;
    private Color[] customColors;

    public void Setup(WeaponData data, WeaponSkinApplier a = null)
    {
        weaponData = data;
        database   = data ? data.skins : null;
        applier    = a ? a : applier;
        if (!applier) applier = FindAnyObjectByType<WeaponSkinApplier>();

        string selected = null;
        if(data.isEquipped)
            selected = WeaponSkinSave.LoadSelected(weaponData.id, weaponData.selectedSkinId);
        else
        {
            selected = WeaponSkinSave.LoadSelected(weaponData.id, "default");
        }
        
        if (selected == "custom")
        {
            UpdateCustomLayout();
            customColors = WeaponSkinSave.LoadCustom(weaponData.id, applier.MaterialCount);
            applier.ApplyCustomColors(customColors);
        }
        else
        {
            UpdatePresetLayout();
            var skin = database ? database.GetById(selected) : null;
            if (skin) applier.ApplySkin(skin);
        }
    }

    // gọi khi click 1 preset trong grid
    public void SelectPreset(string skinId)
    {
        if (!weaponData || !database) return;
        UpdatePresetLayout();
        currentSelectedSkinId = skinId;
        UpdateTextEquipButton(currentSelectedSkinId);
        //WeaponSkinSave.SaveSelected(weaponData.id, skinId);
        var skin = database.GetById(skinId);
        if (skin) applier.ApplySkin(skin);
    }
    
    public void OnClickEquip()
    {
        if (!weaponData || !weaponData.isPurchased) return;
        string currentSkinId = WeaponSkinSave.LoadSelected(weaponData.id, weaponData.selectedSkinId);
        if (isCustom)
        {
            if (currentSkinId != "custom")
            {
                WeaponSkinSave.SaveSelected(weaponData.id, "custom");
                equipBtnText.text = "Equipped";
                equipButton.interactable = false;
            }
        }
        else
        {
            var skin = database ? database.GetById(currentSelectedSkinId) : null;
            if (skin != null)
            {
                WeaponSkinSave.SaveSelected(weaponData.id, skin.id);
                equipBtnText.text = "Equipped";
                equipButton.interactable = false;
                currentSelectedSkinId = "default";
            }
        }

        foreach (var weapon in allDatabases)
        {
            if (weaponData.id != weapon.id)
            {
                WeaponSkinSave.SaveSelected(weapon.id, "default");
            }
        }
    }

    public void UpdatePresetLayout()
    {
        equipBtnReactTransform.anchoredPosition = new Vector2(0f, 150f);
        customPalettePanel.SetActive(false);
        isCustom = false;
    }
    
    public void UpdateTextEquipButton(string skinId)
    {
        string currentWeaponId = GameController.Instance.GetData().GetValueByKey(Params.WeaponKey);
        string currentSkinId = WeaponSkinSave.LoadSelected(weaponData.id, weaponData.selectedSkinId);
        if (currentSkinId == skinId && weaponData.id == currentWeaponId)
        {
            equipBtnText.text = "Equipped";
            equipButton.interactable = false;
        }
        else
        {
            equipBtnText.text = "Equip";
            equipButton.interactable = true;
        }
    }

    // chuyển sang tab "Custom" (hiện bảng màu)
    public void SelectCustom()
    {
        if (!weaponData) return;
        UpdateCustomLayout();
        UpdateTextEquipButton("custom");
        //WeaponSkinSave.SaveSelected(weaponData.id, "custom");

        if (customColors == null || customColors.Length != applier.MaterialCount)
            customColors = WeaponSkinSave.LoadCustom(weaponData.id, applier.MaterialCount);

        applier.ApplyCustomColors(customColors);
    }

    public void UpdateCustomLayout()
    {
        equipBtnReactTransform.anchoredPosition = new Vector2(0f, -150f);
        customPalettePanel.SetActive(true);
        isCustom = true;
    }

    // gắn vào color picker của từng phần (slotIndex = 0..MaterialCount-1)
    public void SetCustomSlotColor(int slotIndex, Color c)
    {
        if (!isCustom) SelectCustom();
        if (customColors == null || slotIndex < 0 || slotIndex >= applier.MaterialCount) return;

        customColors[slotIndex] = c;
        applier.ApplyCustomColors(customColors);
        WeaponSkinSave.SaveCustom(weaponData.id, customColors);
    }
}
