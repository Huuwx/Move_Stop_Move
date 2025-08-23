using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WeaponSkinPanel : MonoBehaviour
{
    [Header("Target")]
    public WeaponData weaponData;
    public WeaponSkinApplier applier;
    public string customSkinId = "custom";

    [Header("UI skins")]
    public Transform gridRoot;
    public WeaponSkinItemUI itemPrefab;
    public TMP_Text title;

    [Header("UI custom (dynamic)")]
    public Transform colorSlotRoot;     // chỗ spawn các nút màu theo slot
    public Button colorSlotButtonPrefab;// 1 button hiển thị màu hiện tại
    public Transform paletteRoot;       // grid các nút màu gợi ý
    public Button paletteColorPrefab;   // 1 ô màu trong bảng
    public List<Color> paletteColors;   // gợi ý (đỏ, xanh, ...)

    private int _index;
    private readonly List<Button> _slotButtons = new();
    private List<Color> _customColors = new(); // danh sách màu hiện hành theo slot
    private int _activeSlot = 0;

    void Start()
    {
        if (!weaponData || weaponData.skins == null)
        { Debug.LogError("WeaponData.skins chưa gán"); return; }

        BuildSkinGrid();
        BuildPalette();
        SetupFromSave();
        ApplyCurrent();
        BuildSlotButtons(); // tuỳ theo số slot của renderer
        RefreshSlotButtons();
    }

    void SetupFromSave()
    {
        string sel = WeaponSkinSave.LoadSelected(weaponData.id, weaponData.selectedSkinId);
        _index = Mathf.Clamp(weaponData.skins.IndexOf(sel), 0, weaponData.skins.skins.Count - 1);

        int slotCount = applier ? applier.GetSlotCount() : 0;
        _customColors = WeaponSkinSave.LoadCustom(weaponData.id, slotCount, Color.white);
        if (_customColors.Count < slotCount)
            for (int i = _customColors.Count; i < slotCount; i++) _customColors.Add(Color.white);
    }

    // ==== GRID SKINS ====
    void BuildSkinGrid()
    {
        foreach (Transform c in gridRoot) Destroy(c.gameObject);
        for (int i = 0; i < weaponData.skins.skins.Count; i++)
        {
            var ui = Instantiate(itemPrefab, gridRoot);
            ui.Bind(weaponData.skins.skins[i], i == _index, OnSelectSkin);
        }
    }
    void OnSelectSkin(WeaponSkinSO skin)
    {
        _index = weaponData.skins.IndexOf(skin.id);
        ApplyCurrent();
        BuildSkinGrid();
    }

    void ApplyCurrent()
    {
        var skin = weaponData.skins.skins[_index];
        title.text = $"{weaponData.name} - {skin.displayName}".ToUpper();
        if (skin.isLocked) return;

        if (skin.id != customSkinId)
        {
            applier.ApplySkin(skin);
        }
        else
        {
            applier.ApplyCustom(_customColors);
        }

        WeaponSkinSave.SaveSelected(weaponData.id, skin.id);
        weaponData.selectedSkinId = skin.id;

        RefreshSlotButtons(); // phản ánh màu hiện tại
    }

    // ==== CUSTOM UI ====
    void BuildSlotButtons()
    {
        foreach (Transform c in colorSlotRoot) Destroy(c.gameObject);
        _slotButtons.Clear();

        int n = applier ? applier.GetSlotCount() : 0;
        for (int i = 0; i < n; i++)
        {
            int slot = i;
            var btn = Instantiate(colorSlotButtonPrefab, colorSlotRoot);
            _slotButtons.Add(btn);
            btn.onClick.AddListener(() => _activeSlot = slot);
        }
    }

    void RefreshSlotButtons()
    {
        for (int i = 0; i < _slotButtons.Count; i++)
        {
            var img = _slotButtons[i].GetComponent<Image>();
            if (img) img.color = _customColors[i];
        }
    }

    void BuildPalette()
    {
        foreach (Transform c in paletteRoot) Destroy(c.gameObject);
        foreach (var col in paletteColors)
        {
            var b = Instantiate(paletteColorPrefab, paletteRoot);
            var img = b.GetComponent<Image>(); if (img) img.color = col;
            b.onClick.AddListener(() =>
            {
                if (_activeSlot < 0 || _activeSlot >= _customColors.Count) return;
                _customColors[_activeSlot] = col;
                applier.ApplyCustomForSlot(_activeSlot, col);
                RefreshSlotButtons();
                if (IsCustomSelected()) WeaponSkinSave.SaveCustom(weaponData.id, _customColors);
            });
        }
    }

    bool IsCustomSelected() => weaponData.skins.skins[_index].id == customSkinId;
}
