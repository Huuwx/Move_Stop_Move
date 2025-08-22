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

    public class LivesComponent : MonoBehaviour
    {
        [SerializeField] int lives = 1;
        public void AddLife(int v) => lives += v;

        public void Kill()
        {
            lives--;
            if (lives > 0)
            {
                // revive
                EventObserver.OnPlayerRevived?.Invoke();
                // hồi HP, v.v.
            }
            else
            {
                EventObserver.OnPlayerDeath?.Invoke();
                // Game over
            }
        }
    }
}