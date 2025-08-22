using UnityEngine;

namespace ZombieCity.Abilities
{
    [CreateAssetMenu(menuName = "ZombieCity/Effects/ExtraLife")]
    public class ExtraLifeEffectSO : AbilityEffectSO
    {
        public override void Apply(PlayerContext ctx, AbilityRuntime runtime, int level)
        {
            ctx.Lives.AddLife(1);
        }

        public override void Remove(PlayerContext ctx, AbilityRuntime runtime, int level)
        {
            // không remove mạng đã cấp
        }
    }
}
