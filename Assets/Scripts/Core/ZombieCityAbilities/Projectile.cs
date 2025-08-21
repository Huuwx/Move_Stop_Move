using UnityEngine;

namespace ZombieCity.Abilities
{
    public class Projectile : MonoBehaviour
    {
        private Vector2 _dir; private float _speed; private float _damage; private float _range;
        private bool _homing; private float _traveled;

        public void Init(Vector2 dir, float speed, float damage, float range, bool homing)
        { _dir = dir; _speed = speed; _damage = damage; _range = range; _homing = homing; }

        private void Update()
        {
            if (_homing)
            {
                // Homing đơn giản: xoay dần về hướng zombie gần nhất
                var t = FindClosestEnemy();
                if (t != null)
                    _dir = Vector2.Lerp(_dir, ((Vector2)t.position - (Vector2)transform.position).normalized, 0.08f).normalized;
            }

            var delta = _dir * (_speed * Time.deltaTime);
            transform.position += (Vector3)delta;
            _traveled += delta.magnitude;
            if (_traveled >= _range) Destroy(gameObject);
        }

        private Transform FindClosestEnemy()
        {
            // Demo: tag "Enemy"
            var enemies = GameObject.FindGameObjectsWithTag("Enemy");
            Transform best = null; float min = float.MaxValue;
            foreach (var e in enemies)
            {
                float d = (e.transform.position - transform.position).sqrMagnitude;
                if (d < min) { min = d; best = e.transform; }
            }
            return best;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Enemy")) return;
            // TODO: trừ máu enemy theo _damage
            Destroy(gameObject);
            EventObserver.OnEnemyKilled?.Invoke(other.gameObject);
        }
    }
}