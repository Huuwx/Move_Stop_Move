using UnityEngine;

[CreateAssetMenu(menuName = "Wardrobe/Outfit Set")]
public class OutfitSet : ScriptableObject
{
    [Header("Identity")]
    public string id;                 // vd: "set_samurai_01"
    public string displayName;
    public Sprite icon;

    [Header("Items")]
    public ClothingItem hat;
    public ClothingItem pants;
    public ClothingItem shield;

    [Header("Progression")]
    public bool unlocked = false;
    public int price = 0;

    // tiện ích: duyệt nhanh từng món trong set
    public ClothingItem GetItem(OutfitCategory slot)
    {
        return slot switch
        {
            OutfitCategory.Hat    => hat,
            OutfitCategory.Pants  => pants,
            OutfitCategory.Shield => shield,
            _ => null
        };
    }
}