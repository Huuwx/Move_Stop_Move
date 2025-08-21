using UnityEngine;

namespace ZombieCity.Abilities
{
    /// Gom các component quan trọng của player để các hiệu ứng dùng.
    public class PlayerContext
    {
        public GameObject GameObject { get; }
        public Transform Transform { get; }
        public PlayerStats Stats { get; }
        public WeaponAttack Weapon { get; }
        public LivesComponent Lives { get; }

        public PlayerContext(GameObject go)
        {
            GameObject = go;
            Transform = go.transform;
            Stats = go.GetComponent<PlayerStats>();
            Weapon = go.GetComponent<WeaponAttack>();
            Lives  = go.GetComponent<LivesComponent>();
        }
    }
}