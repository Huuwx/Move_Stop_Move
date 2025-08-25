using UnityEngine;

[DisallowMultipleComponent]
public class WeaponSkinApplier : MonoBehaviour
{
    [SerializeField] private MeshRenderer meshRenderer;

    void Reset()
    {
        if (!meshRenderer) meshRenderer = GetComponentInChildren<MeshRenderer>();
    }

    public int MaterialCount => meshRenderer ? meshRenderer.sharedMaterials.Length : 0;

    string FindColorProp(Material m)
    {
        if (!m) return null;
        if (m.HasProperty("_BaseColor")) return "_BaseColor"; // URP Lit
        if (m.HasProperty("_Color"))     return "_Color";     // Standard
        return null;
    }

    string FindTexProp(Material m)
    {
        if (!m) return null;
        if (m.HasProperty("_BaseMap")) return "_BaseMap"; // URP Lit
        if (m.HasProperty("_MainTex")) return "_MainTex"; // Standard
        return null;
    }

    public void ApplySkin(WeaponSkin skin)
    {
        if (!meshRenderer || skin == null) return;
        ClearAllPropertyBlocks();

        foreach (var s in skin.slots)
        {
            if (s.materialIndex < 0 || s.materialIndex >= MaterialCount) continue;

            var mat = meshRenderer.sharedMaterials[s.materialIndex];
            var block = new MaterialPropertyBlock();
            meshRenderer.GetPropertyBlock(block, s.materialIndex);

            if (s.overrideColor)
            {
                string colorProp = FindColorProp(mat);
                if (!string.IsNullOrEmpty(colorProp))
                    block.SetColor(colorProp, s.color);
            }

            if (s.overrideTexture && s.albedo)
            {
                string texProp = FindTexProp(mat);
                if (!string.IsNullOrEmpty(texProp))
                    block.SetTexture(texProp, s.albedo);
            }

            meshRenderer.SetPropertyBlock(block, s.materialIndex);
        }
    }

    // Custom: mảng màu theo thứ tự materialIndex (0..n-1)
    public void ApplyCustomColors(Color[] slotColors)
    {
        if (!meshRenderer || slotColors == null) return;
        ClearAllPropertyBlocks();

        int n = Mathf.Min(slotColors.Length, MaterialCount);
        for (int i = 0; i < n; i++)
        {
            var mat = meshRenderer.sharedMaterials[i];
            string colorProp = FindColorProp(mat);
            if (string.IsNullOrEmpty(colorProp)) continue;

            var block = new MaterialPropertyBlock();
            meshRenderer.GetPropertyBlock(block, i);
            block.SetColor(colorProp, slotColors[i]);
            meshRenderer.SetPropertyBlock(block, i);
        }
    }

    public void ClearAllPropertyBlocks()
    {
        if (!meshRenderer) return;
        var block = new MaterialPropertyBlock();
        for (int i = 0; i < MaterialCount; i++)
        {
            block.Clear();
            meshRenderer.SetPropertyBlock(block, i);
        }
    }
}
