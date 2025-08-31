using System;
using UnityEngine;

public class WeaponProjectile : MonoBehaviour
{
    private GameObject actor;            // Chủ thể bắn (Player hoặc Enemy)
    private WeaponData weaponData;       // Data vũ khí
    private LayerMask targetLayer;

    [Header("Variables")]
    [SerializeField] private float range = 10f;          // Tầm bay tối đa (đang dùng để Outbound)
    [SerializeField] private float traveled; 
    [SerializeField] private float rotateSpeed = 360f;
    [SerializeField] private float ultimateSize = 2f;

    private bool isUltimate = false;
    private Vector3 direction;           // Hướng bay hiện tại
    private Rigidbody _rigidbody;
    private float lifeTimer;

    // ===== Boomerang =====
    private enum FlightPhase { Outbound, Returning }
    [Header("Boomerang")]
    [SerializeField] private float catchRadius = 0.75f;       // bán kính “bắt đạn” quanh actor

    private FlightPhase phase = FlightPhase.Outbound;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    /// <summary>
    /// Gọi khi bắn.
    /// </summary>
    public void Launch(
        Vector3 direction,
        LayerMask targetLayer,
        WeaponData weaponData,
        GameObject actor,
        bool isUltimate = false,
        float range = 10f
    )
    {
        this.actor = actor;
        this.direction = direction.normalized;
        this.targetLayer = targetLayer;
        this.weaponData = weaponData;
        this.isUltimate = isUltimate;
        this.range = range;

        traveled = 0f;
        lifeTimer = 0f;
        phase = FlightPhase.Outbound;

        transform.localScale = Vector3.one;

        gameObject.SetActive(true);

        // Thiết lập vận tốc ban đầu theo hướng bắn
        _rigidbody.linearVelocity = this.direction * weaponData.speed;
    }

    private void Update()
    {
        // Tính quãng đường đã đi (theo hướng hiện tại)
        var delta = direction * (weaponData.speed * Time.deltaTime);
        traveled += delta.magnitude;

        // Đổi pha khi đạt điều kiện
        if (weaponData.isBoomerang)
        {
            if (phase == FlightPhase.Outbound)
            {
                // Điều kiện chuyển sang quay về: đi đủ outDistance hoặc chạm range
                if (traveled >= range)
                {
                    if (isUltimate)
                    {
                        Deactivate();
                        return;
                    }
                    phase = FlightPhase.Returning;
                }
            }
            else // Returning
            {
                // Khi về gần actor đủ bắt → tắt đạn
                if (actor != null)
                {
                    float distToActor = Vector3.Distance(transform.position, actor.transform.position);
                    if (distToActor <= catchRadius)
                    {
                        Deactivate();
                        return;
                    }
                }
            }
        }
        else
        {
            // Đạn thẳng thường: hết range thì tắt
            if (traveled >= range)
            {
                Deactivate();
                return;
            }
        }

        // Xoay cosmetic (nếu cần)
        if (weaponData.isRotate && !isUltimate)
        {
            transform.Rotate(0f, -rotateSpeed * Time.deltaTime, 0f, Space.Self);
        }

        // Ultimate: phóng to dần
        if (isUltimate)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one * ultimateSize, Time.deltaTime);
        }
        else
        {
            transform.localScale = Vector3.one;
        }
    }

    private void FixedUpdate()
    {
        // Điều khiển đường bay tại FixedUpdate (ổn định hơn cho Rigidbody)
        if (!weaponData.isBoomerang || phase == FlightPhase.Outbound)
        {
            // Bay thẳng ra
            _rigidbody.linearVelocity = direction * weaponData.speed;
        }
        else if (phase == FlightPhase.Returning)
        {
            if (actor == null || isUltimate)
            {
                // Không còn actor (bị Destroy?) thì thôi bay thẳng tiếp
                _rigidbody.linearVelocity = direction * weaponData.speed;
                return;
            }

            // Hướng mong muốn: về phía actor hiện tại (actor có thể đã di chuyển)
            Vector3 toActor = (actor.transform.position - transform.position).normalized;

            // Tốc độ pha quay về có thể khác (nhanh/chậm hơn)
            _rigidbody.linearVelocity = toActor * (weaponData.speed);
        }
    }

    private void Deactivate()
    {
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Chỉ xử lý khi va chạm với layer mục tiêu
        if (((1 << other.gameObject.layer) & targetLayer) != 0)
        {
            // Đạn boomerang vẫn gây sát thương như thường (tuỳ thiết kế của bạn)
            if (!isUltimate)
            {
                Deactivate();
            }
    
            if (other.CompareTag(Params.PlayerTag))
            {
                PlayerController playerController = other.GetComponent<PlayerController>();
                if (playerController != null)
                {
                    EnemyAI enemyAI = actor != null ? actor.GetComponent<EnemyAI>() : null;
                    if (enemyAI != null) enemyAI.points += 1;
                    GameController.Instance.GetUIController().SetKillerName(enemyAI.nameText.text, enemyAI.nameText.color);
                    playerController.Die();
                }
            }
            else if (other.CompareTag(Params.BotTag))
            {
                PlayerController player = actor != null ? actor.GetComponent<PlayerController>() : null;
                if (player != null) EventObserver.RaiseOnUpgrade();
                else
                {
                    EnemyAI enemyAI = actor != null ? actor.GetComponent<EnemyAI>() : null;
                    if (enemyAI != null) enemyAI.RaiseOnUpgradeEvent();
                }
    
                EnemyBase enemy = other.GetComponent<EnemyBase>();
                if (enemy != null) enemy.Die();
            }
        }
    }
}
