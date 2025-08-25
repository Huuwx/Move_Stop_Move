using UnityEngine;

[CreateAssetMenu(menuName = "Game/Skins/Weapon Skin", fileName = "WeaponSkin")]
public class WeaponSkin : ScriptableObject
{
    public string id = "default";
    public string displayName;
    public Sprite preview;
    public bool isCustom = false; // preset: false, skin đặc biệt "custom": true

    [System.Serializable]
    public struct SlotOverride
    {
        [Tooltip("Material index trong MeshRenderer")]
        public int materialIndex;

        [Header("Color")]
        public bool overrideColor;
        public Color color;

        [Header("Texture (tuỳ chọn)")]
        public bool overrideTexture;
        public Texture2D albedo; // nếu muốn đổi albedo
    }

    public SlotOverride[] slots;
}