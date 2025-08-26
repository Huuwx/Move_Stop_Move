// ClothingItem.cs
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "Wardrobe/Clothing Item")]
public class ClothingItem : ScriptableObject
{
    public string id;                       // duy nhất (vd: "hat_viking_01")
    public string displayName;
    public OutfitCategory category;
    public RenderTexture icon;                     // icon hiện trong ô xanh
    public GameObject prefab;               // prefab đội/mặc/cầm
    public Material material;
    public bool unlocked = false;
    public bool equipped = false; // đã trang bị hay chưa
    public int price;
    public int upgradeIndex = 0;
}