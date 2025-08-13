// ClothingItem.cs
using UnityEngine;

[CreateAssetMenu(menuName = "Wardrobe/Clothing Item")]
public class ClothingItem : ScriptableObject
{
    public string id;                       // duy nhất (vd: "hat_viking_01")
    public string displayName;
    public OutfitCategory category;
    public Sprite icon;                     // icon hiện trong ô xanh
    public GameObject prefab;               // prefab đội/mặc/cầm
    public bool unlockedByDefault = true;
    public int price;
}