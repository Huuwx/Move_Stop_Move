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
        public StatValue fireRate  = new() { baseValue = 1.5f }; // shots/sec
        public StatValue projCount = new() { baseValue = 1 };

        private readonly List<StatModifier> _mods = new();

        public void AddModifier(StatModifier m) => _mods.Add(m);
        public void RemoveBySource(string sourceId) => _mods.RemoveAll(m => m.sourceId == sourceId);

        public float Get(StatType type)
        {
            float b = type switch
            {
                StatType.FireRate => fireRate.baseValue,
                StatType.ProjectileCount => projCount.baseValue,
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
