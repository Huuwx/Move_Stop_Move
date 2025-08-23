using System.Collections.Generic;
using UnityEngine;

/// Áp skin/custom cho 1 Renderer nhiều material slots (Element 0..n-1)
[RequireComponent(typeof(Renderer))]
public class WeaponSkinApplier : MonoBehaviour
{
    [SerializeField] private string weaponId;        // khớp WeaponData.id
    [SerializeField] private Renderer targetRenderer;
    [Tooltip("URP Lit: _BaseColor | BiRP/Standard: _Color")]
    public string colorPropertyName = "_BaseColor";

    private int _colorId;
    private readonly Dictionary<int, MaterialPropertyBlock> _blocks = new();

    void Awake()
    {
        if (!targetRenderer) targetRenderer = GetComponent<Renderer>();
        _colorId = Shader.PropertyToID(colorPropertyName);
        EnsureBlocks();
    }

    void EnsureBlocks()
    {
        int n = GetSlotCount();
        for (int i = 0; i < n; i++)
            if (!_blocks.ContainsKey(i)) _blocks[i] = new MaterialPropertyBlock();
    }

    public int GetSlotCount() => targetRenderer ? targetRenderer.sharedMaterials.Length : 0;

    public void ApplySkin(WeaponSkinSO skin)
    {
        if (!targetRenderer || skin == null) return;
        if (!string.IsNullOrEmpty(skin.weaponId) && skin.weaponId != weaponId)
            Debug.LogWarning($"Skin '{skin.id}' không thuộc weapon '{weaponId}'");

        EnsureBlocks();

        // 1) override material theo slot (nếu có)
        if (skin.slotOverrides != null && skin.slotOverrides.Count > 0)
        {
            var mats = targetRenderer.sharedMaterials;
            int n = Mathf.Min(mats.Length, skin.slotOverrides.Count);
            for (int i = 0; i < n; i++)
                if (skin.slotOverrides[i]) mats[i] = skin.slotOverrides[i];
            targetRenderer.sharedMaterials = mats;
        }

        // 2) gán màu theo slot
        ApplyCustom(skin.slotColors);
    }

    public void ApplyCustom(IList<Color> colors)
    {
        if (!targetRenderer || colors == null) return;

        int n = Mathf.Min(GetSlotCount(), colors.Count);
        for (int i = 0; i < n; i++)
        {
            var mpb = _blocks[i];
            targetRenderer.GetPropertyBlock(mpb, i);
            mpb.SetColor(_colorId, colors[i]);
            targetRenderer.SetPropertyBlock(mpb, i);
        }
    }

    // Tiện lợi: set 1 slot
    public void ApplyCustomForSlot(int slotIndex, Color color)
    {
        if (!targetRenderer) return;
        if (slotIndex < 0 || slotIndex >= GetSlotCount()) return;

        var mpb = _blocks[slotIndex];
        targetRenderer.GetPropertyBlock(mpb, slotIndex);
        mpb.SetColor(_colorId, color);
        targetRenderer.SetPropertyBlock(mpb, slotIndex);
    }
}
