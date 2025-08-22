using System;
using UnityEngine;

public class WeaponProjectile : MonoBehaviour
{
    private GameObject actor;        // Actor bắn ra vũ khí này (Player hoặc Enemy)
    
    private WeaponData weaponData;       //Data của vũ khí hiện tại
    
    private LayerMask targetLayer;
    
    [Header("Variables")]
    [SerializeField] private float maxLifeTime = 1.5f;   // Sau thời gian này sẽ tự hủy (tránh bay mãi)
    [SerializeField] private float range;
    [SerializeField] private float traveled; 
    [SerializeField] private float rotateSpeed;
    [SerializeField] private float ultimateSize = 2f; // Kích thước của vũ khí khi Ultimate
    
    private bool isUltimate = false; // Kiểm tra xem có phải là Ultimate hay không
    private Vector3 direction;      // Hướng bay của vũ khí
    
    
    private Rigidbody _rigidbody;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    public void Launch(Vector3 direction, LayerMask targetLayer, WeaponData weaponData, GameObject actor, bool isUltimate = false)
    {
        this.actor = actor; // Lưu actor bắn ra vũ khí này
        
        this.direction = direction.normalized;
        this.targetLayer = targetLayer;
        this.weaponData = weaponData;
        this.isUltimate = isUltimate;
        this.traveled = 0f; // Reset traveled distance
        
        transform.localScale = Vector3.one;
        
        gameObject.SetActive(true);
    }

    void Update()
    {
        var delta = direction * (weaponData.speed * Time.deltaTime);
        traveled += delta.magnitude;
        if (traveled >= range) Deactivate();
        
        if(weaponData.isRotate && !isUltimate)
            transform.Rotate(0, -rotateSpeed * Time.deltaTime, 0);

        if (isUltimate)
        {
            // Nếu là Ultimate, tăng kích thước vũ khí
            transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one * ultimateSize, Time.deltaTime);
        }
        else
        {
            // Nếu không phải Ultimate, giữ kích thước bình thường
            transform.localScale = Vector3.one;
        }
    }

    private void FixedUpdate()
    {
        _rigidbody.linearVelocity = direction * weaponData.speed;
    }
    
    void Deactivate()
    {
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & targetLayer) != 0)
        {
            Deactivate(); // Tắt projectile (pooling)
            if (other.CompareTag(Params.PlayerTag))
            {
                PlayerController playerController = other.GetComponent<PlayerController>();
                if (playerController != null)
                {
                    EnemyAI enemyAI = actor.GetComponent<EnemyAI>();
                    if (enemyAI != null)
                    {
                        enemyAI.points += 1;
                    }
    
                    playerController.KillPlayer();
                }
            } else if (other.CompareTag(Params.BotTag))
            {
                PlayerController player = actor.GetComponent<PlayerController>();
                if (player != null)
                {
                    EventObserver.RaiseOnUpgrade();
                }
                else
                {
                    EnemyAI enemyAI = actor.GetComponent<EnemyAI>();
                    if (enemyAI != null)
                    {
                        enemyAI.RaiseOnUpgradeEvent();
                    }
                }
    
                EnemyBase enemy = other.GetComponent<EnemyBase>();
                if (enemy != null)
                    enemy.Die();
            }
        }
    }
}