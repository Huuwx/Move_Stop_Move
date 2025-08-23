using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Skins/Weapon Skin (Flexible)", fileName = "WeaponSkin")]
public class WeaponSkinSO : ScriptableObject
{
    [Header("Binding")]
    public string weaponId;                 // phải khớp WeaponData.id

    [Header("Identity")]
    public string id;                       // "default", "redblue", "custom", ...
    public string displayName;
    public Sprite icon;
    public bool isLocked;

    [Header("Colors per material slot (0..n-1)")]
    public List<Color> slotColors = new();  // chiều dài = số slot muốn set
    // (thiếu -> giữ nguyên, thừa -> bỏ qua)

    [Header("Optional: material override theo slot (cùng chiều dài với slotColors hoặc để trống)")]
    public List<Material> slotOverrides = new(); // null/empty = không override
}