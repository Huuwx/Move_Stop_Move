using UnityEngine;

public class WeaponProjectile : MonoBehaviour
{
    [SerializeField] private float speed = 12f;
    [SerializeField] private float maxLifeTime = 2f;   // Sau thời gian này sẽ tự hủy (tránh bay mãi)
    [SerializeField] private ParticleSystem hitEffect;  // Hiệu ứng khi trúng

    private Vector3 _direction;
    private float _timer;
    [SerializeField] private LayerMask _targetLayer;
    [SerializeField] private float _damage;

    public void Launch(Vector3 direction, LayerMask targetLayer)
    {
        _direction = direction.normalized;
        _targetLayer = targetLayer;
        _timer = 0f;
        gameObject.SetActive(true);
    }

    void Update()
    {
        // Di chuyển theo hướng đã set
        transform.position += _direction * speed * Time.deltaTime;
        _timer += Time.deltaTime;
        if (_timer >= maxLifeTime)
        {
            Deactivate();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & _targetLayer) != 0)
        {
            // Gây damage nếu cần (hoặc Destroy/ẩn đối tượng)
            // Destroy(other.gameObject); // hoặc gọi hàm nhận damage

            // Gọi hiệu ứng trúng
            if (hitEffect != null) hitEffect.Play();

            Deactivate(); // Tắt projectile (pooling)
        }
    }

    void Deactivate()
    {
        gameObject.SetActive(false);
    }
}