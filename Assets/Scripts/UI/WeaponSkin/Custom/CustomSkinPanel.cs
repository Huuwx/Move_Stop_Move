using System.Collections.Generic;
using UnityEngine;

public class CustomSkinPanel : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private WeaponSkinSelector selector;    // đã có sẵn từ trước
    [SerializeField] private RectTransform slotRoot;         // Content sinh SlotItemUI
    [SerializeField] private SlotItemUI slotPrefab;          // Prefab nút Part
    [SerializeField] private CustomPaletteUI palette;        // Bảng nút màu

    private WeaponData weapon;
    private WeaponSkinApplier applier;
    private readonly List<SlotItemUI> slots = new();
    private int currentIndex = 0;
    private Color[] currentColors;

    /// <summary>
    /// Gọi khi mở panel hoặc đổi vũ khí trong shop.
    /// </summary>
    public void OpenFor(WeaponData w, WeaponSkinApplier a)
    {
        weapon  = w;
        applier = a;

        // 1) Setup selector và chuyển sang chế độ Custom
        selector.Setup(weapon, applier);
        //selector.SelectCustom(); // Custom = chỉ chọn màu

        // 2) Nạp màu custom đã lưu (fit theo số material)
        currentColors = WeaponSkinSave.LoadCustom(weapon.id, applier.MaterialCount);

        // 3) Dựng danh sách Part
        BuildSlots();

        // 4) Chọn Part đầu tiên
        //SelectSlot(0);

        // 5) Lắng nghe click từ palette
        if (palette)
        {
            palette.onPick.RemoveAllListeners();
            palette.onPick.AddListener(OnColorPicked);
            palette.Build(); // đảm bảo đã dựng palette
        }
    }

    private void BuildSlots()
    {
        foreach (Transform t in slotRoot) Destroy(t.gameObject);
        slots.Clear();

        int count = applier.MaterialCount;
        for (int i = 0; i < count; i++)
        {
            var ui = Instantiate(slotPrefab, slotRoot);
            int idx = i;
            ui.Bind(idx, currentColors[idx], OnSlotClicked);
            ui.SetSelected(i == 0);
            slots.Add(ui);
        }
    }

    private void OnSlotClicked(int idx) => SelectSlot(idx);

    private void SelectSlot(int idx)
    {
        if (applier.MaterialCount == 0) return;
        currentIndex = Mathf.Clamp(idx, 0, applier.MaterialCount - 1);

        for (int i = 0; i < slots.Count; i++)
            slots[i].SetSelected(i == currentIndex);

        // Đảm bảo đang ở Custom
        selector.SelectCustom();
    }

    private void OnColorPicked(Color c)
    {
        // 1) Cập nhật cache + chấm màu trên nút Part
        currentColors[currentIndex] = c;
        if (currentIndex < slots.Count) slots[currentIndex].SetColor(c);

        // 2) Áp ngay lên model preview (PropertyBlock)
        selector.SetCustomSlotColor(currentIndex, c);

        // 3) Lưu toàn bộ mảng màu cho vũ khí
        WeaponSkinSave.SaveCustom(weapon.id, currentColors);
    }
}
