using System.Collections.Generic;
using UnityEngine;

namespace ZombieCity.Abilities
{
    [CreateAssetMenu(menuName = "ZombieCity/Ability", fileName = "Upgrade Abilities")]
    public class CanUpgradeAbilities : ScriptableObject
    {
        [Header("Abilities")]
        public string id;                       // unique (ex: "triple_shot")
        public string displayName;
        [TextArea] public string description;
        public Sprite icon;
        public AbilityRarity rarity = AbilityRarity.Common;
        public int maxLevel = 1;

        [Header("Dependencies")]
        public List<AbilitySO> prerequisites;   // phải có trước (tuỳ chọn)
        public List<AbilityEffectSO> effects;   // 1 kỹ năng có thể gồm nhiều hiệu ứng

        public bool CanStack => maxLevel > 1;
    }
}
