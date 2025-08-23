using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Skins/Weapon Skin Database", fileName = "WeaponSkinDB")]
public class WeaponSkinDatabase : ScriptableObject
{
    public string weaponId;
    public List<WeaponSkinSO> skins;

    public WeaponSkinSO GetById(string id) => skins.Find(s => s.id == id);
    public int IndexOf(string id) => skins.FindIndex(s => s.id == id);
}