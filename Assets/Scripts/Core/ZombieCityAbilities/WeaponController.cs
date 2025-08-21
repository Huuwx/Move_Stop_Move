using System.Collections.Generic;
using UnityEngine;

namespace ZombieCity.Abilities
{
    public class WeaponController : MonoBehaviour
    {
        [Header("Refs")]
        public GameObject projectilePrefab;
        public Transform shootOrigin;

        private float _cooldown;
        private IShotPattern _pattern = new BasicForwardPattern();
        private PlayerStats _stats;
        private bool _homing;

        private void Awake() => _stats = GetComponent<PlayerStats>();

        public void SetHoming(bool on) => _homing = on;

        public void AddPatternDecorator(IShotPattern decorator) => _pattern = decorator;

        public void RemoveDecoratorById(string id)
        {
            // Gỡ theo id (đơn giản: rebuild chuỗi pattern khi cần — ở demo mình bỏ qua phần rebuild chi tiết)
            // Cách “an toàn” hơn: lưu stack decorator trong list, khi remove thì dựng lại chain.
        }

        private void Update()
        {
            _cooldown -= Time.deltaTime;
            if (_cooldown > 0f) return;

            // Fire theo FireRate
            float fireRate = Mathf.Max(0.1f, _stats.Get(StatType.FireRate));
            _cooldown = 1f / fireRate;

            Shoot();
        }

        private void Shoot()
        {
            Vector2 forward = (Vector2)transform.right; // ví dụ dùng hướng X+
            IShotPattern effective = _pattern;                       // dùng pattern hiện tại
            int extraByStat = Mathf.Max(0, Mathf.RoundToInt(_stats.Get(StatType.ProjectileCount) - 1));
            if (extraByStat > 0) effective = new FanExtraProjectilesPattern(effective, extraByStat, 25f);

            var dirs = effective.GetDirections(forward);


            foreach (var dir in dirs)
            {
                var go = Instantiate(projectilePrefab, shootOrigin.position, Quaternion.identity);
                var proj = go.GetComponent<Projectile>();
                proj.Init(dir.normalized,
                          speed: _stats.Get(StatType.ProjectileSpeed),
                          damage: _stats.Get(StatType.Damage),
                          range: _stats.Get(StatType.Range),
                          homing: _homing);
            }
        }
    }
}
