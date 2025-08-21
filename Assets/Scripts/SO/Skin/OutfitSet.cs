using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Wardrobe/Outfit Set")]
public class OutfitSet : ScriptableObject
{
    [Header("Identity")]
    public string id;
    public string displayName;
    public Sprite icon;

    [Header("Items")]
    public List<ClothingItem> items;

    [Header("Progression")]
    public bool unlocked = false;
    public bool equipped = false; // đã trang bị hay chưa
    public int upgradeIndex = 0; // chỉ số nâng cấp hiện tại
    public int price = 0;
}