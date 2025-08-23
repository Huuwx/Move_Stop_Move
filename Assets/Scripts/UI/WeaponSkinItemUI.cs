using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WeaponSkinItemUI : MonoBehaviour
{
    [Header("UI Parts")]
    [Tooltip("Ảnh icon skin trong grid")]
    public Image icon;
    [Tooltip("Tên hiển thị (tùy chọn)")]
    public TMP_Text label;
    [Tooltip("Khung/viền chọn (Enable = đang được chọn)")]
    public GameObject selectFrame;
    [Tooltip("Overlay khóa (padlock / mờ)")]
    public GameObject lockOverlay;
    [Tooltip("Nút bấm chọn")]
    public Button button;
    [Tooltip("Nếu muốn làm mờ khi bị khóa")]
    public CanvasGroup canvasGroup;

    // Data
    private WeaponSkinSO _skin;
    private Action<WeaponSkinSO> _onClick;

    /// <summary>
    /// Gắn dữ liệu cho 1 item UI trong grid.
    /// </summary>
    public void Bind(WeaponSkinSO skin, bool selected, Action<WeaponSkinSO> onClick)
    {
        _skin = skin;
        _onClick = onClick;

        if (icon)  icon.sprite = skin.icon;
        if (label) label.text  = string.IsNullOrEmpty(skin.displayName) ? skin.id : skin.displayName;

        SetSelected(selected);
        SetLocked(skin.isLocked);

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => _onClick?.Invoke(_skin));
    }

    public void SetSelected(bool selected)
    {
        if (selectFrame) selectFrame.SetActive(selected);
        // Có thể đổi màu nền nhẹ để nhấn mạnh:
        // GetComponent<Image>().color = selected ? new Color(1,1,1,0.15f) : Color.clear;
    }

    public void SetLocked(bool locked)
    {
        if (lockOverlay) lockOverlay.SetActive(locked);
        if (canvasGroup) canvasGroup.alpha = locked ? 0.6f : 1f;
        if (button) button.interactable = !locked; // hoặc vẫn cho bấm để mở khoá
    }

    // (Tùy chọn) để button có thể gọi trực tiếp từ Inspector
    public void OnClick()
    {
        if (_skin == null) return;
        _onClick?.Invoke(_skin);
    }
}
