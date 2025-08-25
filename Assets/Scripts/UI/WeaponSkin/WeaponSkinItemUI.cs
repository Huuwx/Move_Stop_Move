using System;
using UnityEngine;
using UnityEngine.UI;
// Nếu bạn dùng TMP thì bỏ comment dòng dưới và đổi Text -> TMP_Text
// using TMPro;

public class WeaponSkinItemUI : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private Button button;            // Button gốc để bấm chọn
    [SerializeField] private Image previewImage;       // Ảnh preview skin (Sprite)
    [SerializeField] private GameObject lockIcon;      // icon/nhãn khóa (nếu skin chưa mở)
    [SerializeField] private GameObject selectedFrame; // khung highlight khi đang chọn

    [Header("State (read-only)")]
    [SerializeField] private string skinId;            // id nội bộ (preset id hoặc "custom")
    [SerializeField] private bool isLocked;
    [SerializeField] private bool isSelected;

    private Action onClick;

    void Reset()
    {
        if (!button)        button        = GetComponentInChildren<Button>(true);
        if (!previewImage)  previewImage  = GetComponentInChildren<Image>(true);
        // lockIcon & selectedFrame: tự kéo thả trong Inspector
    }

    /// <summary>
    /// Bind dữ liệu cho 1 ô skin.
    /// </summary>
    /// <param name="displayName">Tên hiển thị (vd "Gold", "(Custom)")</param>
    /// <param name="preview">Sprite preview (có thể null với "(Custom)")</param>
    /// <param name="click">Callback khi click</param>
    /// <param name="locked">Skin đang bị khóa?</param>
    /// <param name="selected">Skin hiện đang được chọn?</param>
    /// <param name="id">Id nội bộ của skin (vd: "gold", "custom")</param>
    public void Bind(string displayName, Sprite preview, Action click,
                     bool locked = false, bool selected = false, string id = "")
    {
        skinId = id;
        onClick = click;
        
        if (previewImage)
        {
            previewImage.sprite = preview;
            previewImage.enabled = preview != null;
        }

        SetLocked(locked);
        SetSelected(selected);

        if (button)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() =>
            {
                if (isLocked) return;
                SetSelected(true);
                onClick?.Invoke();
            });
        }
    }

    /// <summary> Chỉ dùng khi muốn set id sau Bind (không bắt buộc). </summary>
    public void SetSkinId(string id) => skinId = id;

    public string GetSkinId() => skinId;

    public void SetLocked(bool value)
    {
        isLocked = value;
        if (lockIcon) lockIcon.SetActive(value);
        if (button)   button.interactable = !value;
    }

    public void SetSelected(bool value)
    {
        isSelected = value;
        if (selectedFrame) selectedFrame.SetActive(value);
    }

    public bool IsSelected() => isSelected;

    // Helper nếu cần cập nhật preview/name động:
    public void SetPreview(Sprite s)
    {
        if (!previewImage) return;
        previewImage.sprite = s;
        previewImage.enabled = s != null;
    }
}
