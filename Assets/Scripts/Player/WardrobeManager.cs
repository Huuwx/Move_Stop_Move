// WardrobeManager.cs
using System;
using System.Collections.Generic;
using UnityEngine;

public class WardrobeManager : MonoBehaviour
{
    [SerializeField] Material defaultMaterial; // Mặc định nếu không có item nào
    
    [Header("DB")]
    [SerializeField] WardrobeDatabase database;

    [Header("Anchors trên Player (nơi gắn prefab)")]
    [SerializeField] Transform hatAnchor;
    [SerializeField] Transform pantsAnchor;
    [SerializeField] Transform shieldAnchor;
    [SerializeField] Transform fullBodyAnchor;

    Dictionary<OutfitCategory, Transform> _anchors;
    readonly Dictionary<OutfitCategory, GameObject> _equipped = new();

    void Awake()
    {
        _anchors = new()
        {
            { OutfitCategory.Hat,    hatAnchor },
            { OutfitCategory.Shield,    shieldAnchor },
        };
    }

    private void Start()
    {
        LoadFromSave();
    }

    public void Equip(ClothingItem item)
    {
        var cat = item.category;
        
        if (cat == OutfitCategory.Pants)
        {
            pantsAnchor.gameObject.SetActive(true);
            SkinnedMeshRenderer bodySkinnedMeshRenderer = fullBodyAnchor.GetComponent<SkinnedMeshRenderer>();
            SkinnedMeshRenderer skinnedMeshRenderer = pantsAnchor.GetComponent<SkinnedMeshRenderer>();
            if (skinnedMeshRenderer)
            {
                bodySkinnedMeshRenderer.material = defaultMaterial;
                skinnedMeshRenderer.material = item.material;
            }
            GameController.Instance.GetData().AddKeyValue(cat.ToString(), item.id);
            //GameController.Instance.SaveData();
            return;
        } else if (cat == OutfitCategory.FullBody)
        {
            fullBodyAnchor.gameObject.SetActive(true);
            SkinnedMeshRenderer skinnedMeshRenderer = fullBodyAnchor.GetComponent<SkinnedMeshRenderer>();
            if (skinnedMeshRenderer)
            {
                skinnedMeshRenderer.material = item.material;
                pantsAnchor.gameObject.SetActive(false);
            }
            GameController.Instance.GetData().AddKeyValue(cat.ToString(), item.id);
            //GameController.Instance.SaveData();
            return;
        }

        // gỡ món đang mặc cùng category
        if (_equipped.TryGetValue(cat, out var old) && old) Destroy(old);

        // gắn món mới
        if (item.prefab && _anchors.TryGetValue(cat, out var anchor) && anchor)
        {
            var inst = Instantiate(item.prefab, anchor);
            // inst.transform.localPosition = Vector3.zero;
            // inst.transform.localRotation = Quaternion.identity;
            //inst.transform.localScale    = Vector3.one;
            _equipped[cat] = inst;
        }
        else _equipped[cat] = null;

        // lưu lại
        GameController.Instance.GetData().AddKeyValue(cat.ToString(), item.id);
        //GameController.Instance.SaveData();
    }

    public void LoadFromSave()
    {
        foreach (OutfitCategory cat in Enum.GetValues(typeof(OutfitCategory)))
        {
            var id = GameController.Instance.GetData().GetValueByKey(cat.ToString());
            if (!string.IsNullOrEmpty(id))
            {
                var item = database.GetById(id);
                if (item) Equip(item);
            }
        }
    }
}