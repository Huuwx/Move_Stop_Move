using UnityEngine;
using UnityEngine.UI;

public class WeaponSkinSelector : MonoBehaviour
{
    [SerializeField] private WeaponData weaponData;           // vũ khí đang hiển thị trong UI
    [SerializeField] private WeaponSkinApplier applier;       // applier của model đang render ở UI (hoặc ở tay)
    [SerializeField] private WeaponSkinDatabase database;     // có thể lấy từ weaponData.skins
    [SerializeField] private GameObject customPalettePanel;
    [SerializeField] private RectTransform equipBtn;

    private bool isCustom;
    private Color[] customColors;

    public void Setup(WeaponData data, WeaponSkinApplier a = null)
    {
        weaponData = data;
        database   = data ? data.skins : null;
        applier    = a ? a : applier;
        if (!applier) applier = FindAnyObjectByType<WeaponSkinApplier>();

        string selected = WeaponSkinSave.LoadSelected(weaponData.id, weaponData.selectedSkinId);

        if (selected == "custom")
        {
            isCustom = true;
            customColors = WeaponSkinSave.LoadCustom(weaponData.id, applier.MaterialCount);
            applier.ApplyCustomColors(customColors);
        }
        else
        {
            isCustom = false;
            var skin = database ? database.GetById(selected) : null;
            if (skin) applier.ApplySkin(skin);
        }
    }

    // gọi khi click 1 preset trong grid
    public void SelectPreset(string skinId)
    {
        if (!weaponData || !database) return;
        equipBtn.anchoredPosition = new Vector2(0f, 150f);
        customPalettePanel.SetActive(false);
        isCustom = false;
        WeaponSkinSave.SaveSelected(weaponData.id, skinId);
        var skin = database.GetById(skinId);
        if (skin) applier.ApplySkin(skin);
    }

    // chuyển sang tab "Custom" (hiện bảng màu)
    public void SelectCustom()
    {
        if (!weaponData) return;
        equipBtn.anchoredPosition = new Vector2(0f, -150f);
        customPalettePanel.SetActive(true);
        isCustom = true;
        WeaponSkinSave.SaveSelected(weaponData.id, "custom");

        if (customColors == null || customColors.Length != applier.MaterialCount)
            customColors = WeaponSkinSave.LoadCustom(weaponData.id, applier.MaterialCount);

        applier.ApplyCustomColors(customColors);
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
