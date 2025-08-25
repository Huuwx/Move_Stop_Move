using UnityEngine;

[CreateAssetMenu(menuName = "Game/Skins/Weapon Skin Database", fileName = "WeaponSkinDatabase")]
public class WeaponSkinDatabase : ScriptableObject
{
    public WeaponSkin[] skins;

    public WeaponSkin GetById(string id)
        => System.Array.Find(skins, s => s && s.id == id);

    public WeaponSkin GetDefault()
        => GetById("default") ?? (skins != null && skins.Length > 0 ? skins[0] : null);
}