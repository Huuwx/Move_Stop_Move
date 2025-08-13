// WardrobeManager.cs
using System;
using System.Collections.Generic;
using UnityEngine;

public class WardrobeManager : MonoBehaviour
{
    [Header("DB")]
    public WardrobeDatabase database;

    [Header("Anchors trên Player (nơi gắn prefab)")]
    public Transform hatAnchor;
    public Transform pantsAnchor;
    public Transform shirtAnchor;

    Dictionary<OutfitCategory, Transform> _anchors;
    readonly Dictionary<OutfitCategory, GameObject> _equipped = new();

    void Awake()
    {
        _anchors = new()
        {
            { OutfitCategory.Hat,    hatAnchor },
            { OutfitCategory.Bottom, pantsAnchor },
            { OutfitCategory.Top,    shirtAnchor },
        };
    }

    public void Equip(ClothingItem item)
    {
        var cat = item.category;

        // gỡ món đang mặc cùng category
        if (_equipped.TryGetValue(cat, out var old) && old) Destroy(old);

        // gắn món mới
        if (item.prefab && _anchors.TryGetValue(cat, out var anchor) && anchor)
        {
            var inst = Instantiate(item.prefab, anchor);
            inst.transform.localPosition = Vector3.zero;
            inst.transform.localRotation = Quaternion.identity;
            inst.transform.localScale    = Vector3.one;
            _equipped[cat] = inst;
        }
        else _equipped[cat] = null;

        // lưu lại
        PlayerPrefs.SetString($"wardrobe.equipped.{cat}", item.id);
        PlayerPrefs.Save();
    }

    public void LoadFromSave()
    {
        foreach (OutfitCategory cat in Enum.GetValues(typeof(OutfitCategory)))
        {
            var id = PlayerPrefs.GetString($"wardrobe.equipped.{cat}", string.Empty);
            if (!string.IsNullOrEmpty(id))
            {
                var item = database.GetById(id);
                if (item) Equip(item);
            }
        }
    }
}