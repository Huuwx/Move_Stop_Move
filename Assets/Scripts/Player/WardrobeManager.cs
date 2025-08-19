// WardrobeManager.cs
using System;
using System.Collections.Generic;
using UnityEngine;

public class WardrobeManager : MonoBehaviour
{
    [SerializeField] Material defaultMaterial; // Mặc định nếu không có item nào
    [SerializeField] Material defaultPantsMaterial; // Mặc định nếu không có item nào
    
    [Header("DB")]
    [SerializeField] WardrobeDatabase database;

    [Header("Anchors trên Player (nơi gắn prefab)")]
    [SerializeField] Transform hatAnchor;
    [SerializeField] Transform pantsAnchor;
    [SerializeField] Transform shieldAnchor;
    [SerializeField] Transform fullBodyAnchor;
    [SerializeField] Transform backAnchor;
    [SerializeField] Transform tailAnchor;
    

    private bool isUseOutfitSet = false; // Biến này để xác định có sử dụng OutfitSet hay không
    Dictionary<OutfitCategory, Transform> _anchors;
    readonly Dictionary<OutfitCategory, GameObject> _equipped = new();

    void Awake()
    {
        _anchors = new()
        {
            { OutfitCategory.Hat,    hatAnchor },
            { OutfitCategory.Shield,    shieldAnchor },
            { OutfitCategory.Back, backAnchor },
            { OutfitCategory.Tail, tailAnchor }
        };
        
        // Thêm sẵn mọi key với giá trị null để lần đầu truy cập không bị lỗi
        foreach (OutfitCategory c in Enum.GetValues(typeof(OutfitCategory)))
            _equipped[c] = null;
    }

    private void Start()
    {
        LoadFromSave();
    }

    public void EquipOutfit(OutfitSet outfitSet)
    {
        if (outfitSet == null) return;

        ResetSkin();
        
        foreach (var item in outfitSet.items)
        {
            if (item == null) continue;
            EquipSingleItem(item);
        }
    }
    
    public void EquipSingleItem(ClothingItem item)
    {
        var cat = item.category;
        
        if (cat == OutfitCategory.Pants)
        {
            pantsAnchor.gameObject.SetActive(true);
            SkinnedMeshRenderer bodySkinnedMeshRenderer = fullBodyAnchor.GetComponent<SkinnedMeshRenderer>();
            SkinnedMeshRenderer skinnedMeshRenderer = pantsAnchor.GetComponent<SkinnedMeshRenderer>();
            if (skinnedMeshRenderer && bodySkinnedMeshRenderer)
            {
                bodySkinnedMeshRenderer.material = defaultMaterial;
                skinnedMeshRenderer.material = item.material;
            }

            return;
        } else if (cat == OutfitCategory.SkinFullBody)
        {
            fullBodyAnchor.gameObject.SetActive(true);
            SkinnedMeshRenderer skinnedMeshRenderer = fullBodyAnchor.GetComponent<SkinnedMeshRenderer>();
            if (skinnedMeshRenderer)
            {
                skinnedMeshRenderer.material = item.material;
                pantsAnchor.gameObject.SetActive(false);
            }
            return;
        }

        // gỡ món đang mặc cùng category
        if (_equipped.TryGetValue(cat, out var old) && old)
        {
            Destroy(old);
        }

        // gắn món mới
        if (item.prefab && _anchors.TryGetValue(cat, out var anchor) && anchor)
        {
            var inst = Instantiate(item.prefab, anchor);
            _equipped[cat] = inst;
        }
        else _equipped[cat] = null;
    }

    public void Equip(ClothingItem item)
    {
        if (isUseOutfitSet)
        {
            ResetSkin();
            
            pantsAnchor.gameObject.SetActive(true);
            SkinnedMeshRenderer bodySkinnedMeshRenderer = fullBodyAnchor.GetComponent<SkinnedMeshRenderer>();
            SkinnedMeshRenderer skinnedMeshRenderer = pantsAnchor.GetComponent<SkinnedMeshRenderer>();
            if (skinnedMeshRenderer && bodySkinnedMeshRenderer)
            {
                bodySkinnedMeshRenderer.material = defaultMaterial;
                skinnedMeshRenderer.material = defaultPantsMaterial;
            }
        }

        EquipSingleItem(item);
    }

    public void LoadFromSave()
    {
        ResetSkin();
        
        GameController.Instance.LoadData();
        foreach (OutfitCategory cat in Enum.GetValues(typeof(OutfitCategory)))
        {
            var id = GameController.Instance.GetData().GetValueByKey(cat.ToString());
            if (!string.IsNullOrEmpty(id))
            {
                if (cat == OutfitCategory.OutfitSet)
                {
                    var outfitSet = database.GetOutfitSetById(id);
                    if (outfitSet)
                    {
                        isUseOutfitSet = true;
                        EquipOutfit(outfitSet);
                        break;
                    }
                }
                
                isUseOutfitSet = false;
                var item = database.GetById(id);
                if (item) Equip(item);
            }
            else
            {
                if(cat == OutfitCategory.Pants)
                {
                    pantsAnchor.gameObject.SetActive(true);
                    SkinnedMeshRenderer bodySkinnedMeshRenderer = fullBodyAnchor.GetComponent<SkinnedMeshRenderer>();
                    SkinnedMeshRenderer skinnedMeshRenderer = pantsAnchor.GetComponent<SkinnedMeshRenderer>();
                    if (skinnedMeshRenderer && bodySkinnedMeshRenderer)
                    {
                        bodySkinnedMeshRenderer.material = defaultMaterial;
                        skinnedMeshRenderer.material = defaultPantsMaterial;
                    }
                }
                else if(cat == OutfitCategory.SkinFullBody)
                {
                    SkinnedMeshRenderer skinnedMeshRenderer = fullBodyAnchor.GetComponent<SkinnedMeshRenderer>();
                    if (skinnedMeshRenderer)
                    {
                        skinnedMeshRenderer.material = defaultMaterial;
                        pantsAnchor.gameObject.SetActive(true);
                    }
                }
                else if (cat == OutfitCategory.Hat || cat == OutfitCategory.Shield)
                {
                    if (_anchors.TryGetValue(cat, out var anchor) && anchor)
                    {
                        if (_equipped.TryGetValue(cat, out var inst) && inst)
                            Destroy(inst);

                        _equipped[cat] = null;
                    }
                }

            }
        }
    }

    public void ResetSkin()
    {
        if (_equipped.TryGetValue(OutfitCategory.Back, out var oldBack) && oldBack)
        {
            Destroy(oldBack);
        }
        if (_equipped.TryGetValue(OutfitCategory.Hat, out var oldHat) && oldHat)
        {
            Destroy(oldHat);
        }
        if (_equipped.TryGetValue(OutfitCategory.Shield, out var oldShield) && oldShield)
        {
            Destroy(oldShield);
        }
        if (_equipped.TryGetValue(OutfitCategory.Tail, out var oldTail) && oldTail)
        {
            Destroy(oldTail);
        }
    }
}