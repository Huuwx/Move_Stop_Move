using UnityEngine;

namespace ZombieCity.Abilities
{
    /// Strategy: mỗi hiệu ứng là 1 chiến lược áp dụng/thu hồi.
    public abstract class AbilityEffectSO : ScriptableObject
    {
        /// Lưu dữ liệu runtime để có thể gỡ bỏ sau này (spawned objects, modifiers id...)
        public abstract void Apply(PlayerContext ctx, AbilityRuntime runtime, int level);
        public abstract void Remove(PlayerContext ctx, AbilityRuntime runtime, int level);
    }

    /// Dùng để track những gì đã apply
    [System.Serializable]
    public class AbilityRuntime
    {
        public string instanceId; // GUID duy nhất cho lần áp dụng
        public object data;       // nơi effect có thể lưu object runtime (VD: GO vừa spawn)
    }
}