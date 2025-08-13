using System;
using UnityEngine;

public class WeaponProjectile : MonoBehaviour
{
    private GameObject actor;        // Actor bắn ra vũ khí này (Player hoặc Enemy)
    
    private WeaponData weaponData;       //Data của vũ khí hiện tại
    
    private LayerMask targetLayer;
    
    [Header("Variables")]
    [SerializeField] private float maxLifeTime = 1.5f;   // Sau thời gian này sẽ tự hủy (tránh bay mãi)
    private Vector3 direction;      // Hướng bay của vũ khí
    private float timer;        // Đếm cho đến thời gian biến mất
    [SerializeField] private float rotateSpeed;
    
    private Rigidbody _rigidbody;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    public void Launch(Vector3 direction, LayerMask targetLayer, WeaponData weaponData, GameObject actor)
    {
        this.actor = actor; // Lưu actor bắn ra vũ khí này
        
        this.direction = direction.normalized;
        this.targetLayer = targetLayer;
        this.weaponData = weaponData;
        this.timer = 0f;
        gameObject.SetActive(true);
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= maxLifeTime)
        {
            Deactivate();
        }
        
        if(weaponData.isRotate)
            transform.Rotate(0, 0, rotateSpeed * Time.deltaTime);
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
    
                    playerController.Die();
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
                    EnemyAI enemy = actor.GetComponent<EnemyAI>();
                    if (enemy != null)
                    {
                        enemy.SetPoints();
                    }
                }
    
                EnemyAI enemyAI = other.GetComponent<EnemyAI>();
                if (enemyAI != null)
                    enemyAI.Die();
            }
        }
    }
}