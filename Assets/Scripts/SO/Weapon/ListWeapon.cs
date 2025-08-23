using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/ListWeapon")]
public class ListWeapon : ScriptableObject
{
   public List<WeaponData> weaponList;
   
   public WeaponData GetWeaponById(string id)
      => weaponList.Find(o => o && o.id == id);
}
