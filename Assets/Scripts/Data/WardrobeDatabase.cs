// WardrobeDatabase.cs
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Wardrobe/Database")]
public class WardrobeDatabase : ScriptableObject
{
    public List<ClothingItem> items = new();
    public List<OutfitSet> outfitSets = new();

    public List<ClothingItem> GetByCategory(OutfitCategory c)
        => items.FindAll(i => i.category == c);

    public ClothingItem GetById(string id)
        => items.Find(i => i && i.id == id);
    
    public OutfitSet GetOutfitSetById(string id)
        => outfitSets.Find(o => o && o.id == id);
}