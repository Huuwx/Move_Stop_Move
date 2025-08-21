using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ZombieCity.Abilities
{
    public class PlayerAbilitySystem : MonoBehaviour
    {
        [SerializeField] private List<AbilitySO> abilityPool; // gán trong Inspector
        [SerializeField] private int choicesPerDraft = 3;

        private readonly Dictionary<string, (AbilitySO so, int level, AbilityRuntime runtime)> _owned
            = new();

        private PlayerContext _ctx;

        private void Awake() => _ctx = new PlayerContext(gameObject);

        public bool Has(string id) => _owned.ContainsKey(id);

        public IReadOnlyCollection<(AbilitySO so, int level)> GetOwned()
        {
            var list = new List<(AbilitySO, int)>();
            foreach (var kv in _owned) list.Add((kv.Value.so, kv.Value.level));
            return list;
        }

        /// Rút 3 lựa chọn ngẫu nhiên (có trọng số theo rarity), loại bỏ cái đã max level
        public List<AbilitySO> RollChoices()
        {
            var candidates = abilityPool.FindAll(a =>
            {
                if (_owned.TryGetValue(a.id, out var entry)) return entry.level < a.maxLevel;
                // check prerequisite
                if (a.prerequisites != null)
                    foreach (var pre in a.prerequisites)
                        if (!Has(pre.id)) return false;
                return true;
            });

            List<AbilitySO> result = new();
            for (int i = 0; i < choicesPerDraft && candidates.Count > 0; i++)
            {
                var pick = WeightedPick(candidates);
                result.Add(pick);
                candidates.Remove(pick);
            }
            return result;
        }

        private static AbilitySO WeightedPick(List<AbilitySO> list)
        {
            float Weight(AbilityRarity r) => r switch
            {
                AbilityRarity.Common => 60,
                AbilityRarity.Rare => 30,
                AbilityRarity.Epic => 9,
                AbilityRarity.Legendary => 1,
                _ => 1
            };
            float total = 0; foreach (var a in list) total += Weight(a.rarity);
            float v = Random.value * total;
            foreach (var a in list)
            {
                v -= Weight(a.rarity);
                if (v <= 0) return a;
            }
            return list[0];
        }

        /// Command: áp dụng hoặc nâng cấp
        public void Pick(AbilitySO ability)
        {
            if (!_owned.TryGetValue(ability.id, out var entry))
            {
                var runtime = new AbilityRuntime { instanceId = System.Guid.NewGuid().ToString() };
                ApplyAllEffects(ability, runtime, 1);
                _owned[ability.id] = (ability, 1, runtime);
            }
            else
            {
                int newLv = Mathf.Clamp(entry.level + 1, 1, ability.maxLevel);
                // Remove cũ rồi Apply lại theo level mới (đơn giản & an toàn)
                RemoveAllEffects(entry.so, entry.runtime, entry.level);
                ApplyAllEffects(entry.so, entry.runtime, newLv);
                _owned[ability.id] = (entry.so, newLv, entry.runtime);
            }
        }

        private void ApplyAllEffects(AbilitySO so, AbilityRuntime runtime, int level)
        {
            foreach (var e in so.effects) e.Apply(_ctx, runtime, level);
        }
        private void RemoveAllEffects(AbilitySO so, AbilityRuntime runtime, int level)
        {
            foreach (var e in so.effects) e.Remove(_ctx, runtime, level);
        }
    }
}
