using UnityEngine;
using ZombieCity.Abilities;

namespace ZombieCity.Abilities
{
    [CreateAssetMenu(menuName = "ZombieCity/Effects/ShotPattern/TripleFan")]
    public class TripleFanEffectSO : AbilityEffectSO
    {
        public override void Apply(PlayerContext ctx, AbilityRuntime runtime, int level)
            => ctx.Weapon.AddPatternDecorator(new FanExtraProjectilesPattern(new BasicForwardPattern(), 2, 30f));

        public override void Remove(PlayerContext ctx, AbilityRuntime runtime, int level)
        {
        }
    }
}

