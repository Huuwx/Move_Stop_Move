using System;
using System.Collections.Generic;
using UnityEngine;

namespace ZombieCity.Abilities
{
    [Serializable] public class StatValue { public float baseValue = 1f; }

    [Serializable]
    public class StatModifier
    {
        public string sourceId;        // để biết modifier đến từ kỹ năng nào
        public StatType stat;          // tác động lên stat nào
        public float value;            // trị số
        public ModifierMode mode;      // Add (cộng thẳng) hay Mult (nhân theo tỉ lệ)
        public enum ModifierMode { Add, Mult }
    }

    /// Tính toán final stat = (base + ΣAdd) * Π(1+Mult)
    public class PlayerStats : MonoBehaviour
    {
        [Header("Base")]
        public StatValue maxHp     = new() { baseValue = 100 };
        public StatValue moveSpeed = new() { baseValue = 5 };
        public StatValue fireRate  = new() { baseValue = 1.5f }; // shots/sec
        public StatValue damage    = new() { baseValue = 10 };
        public StatValue projSpeed = new() { baseValue = 14 };
        public StatValue projCount = new() { baseValue = 1 };
        public StatValue range     = new() { baseValue = 8 };
        public StatValue scale     = new() { baseValue = 1 };

        private readonly List<StatModifier> _mods = new();

        public void AddModifier(StatModifier m) => _mods.Add(m);
        public void RemoveBySource(string sourceId) => _mods.RemoveAll(m => m.sourceId == sourceId);

        public float Get(StatType type)
        {
            float b = type switch
            {
                StatType.MaxHP => maxHp.baseValue,
                StatType.MoveSpeed => moveSpeed.baseValue,
                StatType.FireRate => fireRate.baseValue,
                StatType.Damage => damage.baseValue,
                StatType.ProjectileSpeed => projSpeed.baseValue,
                StatType.ProjectileCount => projCount.baseValue,
                StatType.Range => range.baseValue,
                StatType.Scale => scale.baseValue,
                _ => 0
            };
            float add = 0, mult = 1;
            foreach (var m in _mods)
            {
                if (m.stat != type) continue;
                if (m.mode == StatModifier.ModifierMode.Add) add += m.value;
                else mult *= (1f + m.value);
            }
            return Mathf.Max(0f, (b + add) * mult);
        }
    }
}
