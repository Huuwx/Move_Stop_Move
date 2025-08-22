using UnityEngine;

namespace ZombieCity.Abilities
{
    [CreateAssetMenu(menuName = "ZombieCity/Effects/ShotPattern/TripleFan")]
    public class TripleFanEffectSO : AbilityEffectSO
    {
        public override void Apply(PlayerContext ctx, AbilityRuntime runtime, int level)
        {
            ctx.Weapon.AddPatternDecorator(new FanExtraProjectilesPattern(new BasicForwardPattern(), 1, 60f));
        }

        public override void Remove(PlayerContext ctx, AbilityRuntime runtime, int level)
        {
        }
    }
}
