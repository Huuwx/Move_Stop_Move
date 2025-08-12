using System;
using System.Collections.Generic;
using UnityEngine;

public class WeaponAttack : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private WeaponData currentWeapon;      //Vũ khí hiện tại
    [SerializeField] private GameObject weaponHandVisual;
    [SerializeField] private Transform handHoldWeaponTransform;
    
    [SerializeField] private LayerMask targetLayer;                  // Layer của đối thủ (Bots hoặc Player)
    [SerializeField] private AnimationController animationController;       // Animator, gọi animation attack

    [SerializeField] private Transform weaponInstantiateTransform;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Transform throwOrigin;
    
    [Header("Variables")]
    [SerializeField] private float maxAttackCooldown = 0.9f;          // Thời gian cooldown
    private float attackCooldown = 0f;           // Thời gian cooldown hiện tại
    [SerializeField] private float attackRadius = 7f;                // Bán kính vùng attack
    
    private bool canAttack = false;                // Đang ở trạng thái "stop", sẵn sàng attack
    private EnemyAI targetEnemyAI; // Đối thủ hiện tại (nếu có)

    private void Awake()
    {
        attackCooldown = 0f;
        canAttack = false;
    }

    private void Start()
    {
        if(weaponInstantiateTransform == null)
            weaponInstantiateTransform = GameObject.Find("PoolManager").transform.GetChild(0).transform;
        
        if(playerTransform == null)
        {
            playerTransform = transform.GetComponentInParent<Transform>();
        }

        if (!gameObject.CompareTag(Params.PlayerTag)) return;

        if (GameController.Instance.GetData().GetCurrentWeaponData() == null)
        {
            GameController.Instance.GetData().SetCurrentWeaponData(currentWeapon);
            GameController.Instance.GetData().SetCurrentWeaponShopData(currentWeapon);
            GameController.Instance.SaveData();
        }
        else
        {
            currentWeapon = GameController.Instance.GetData().GetCurrentWeaponData();
        }
        
        ChangeWeapon(currentWeapon);
    }

    void Update()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, attackRadius, targetLayer);
        Collider target = null; // Biến để lưu đối thủ mục tiêu
        float minDist = float.MaxValue;
        
        if (hits.Length == 0)
        {
            if (targetEnemyAI != null)
            {
                targetEnemyAI.SetTargetOutlineActive(false);
                targetEnemyAI = null;
            }
        }
        else
        {
            foreach (Collider hit in hits)
            {
                if (hit.gameObject == playerTransform.gameObject) continue; // bỏ qua chính mình
                if (!hit.gameObject.activeSelf) continue;   // bỏ qua đối thủ đã chết
                
                // Nếu là đối thủ (có thể check thêm tag, hoặc component BotController)
                if (hit.CompareTag(Params.BotTag) || hit.CompareTag(Params.PlayerTag))
                {
                    float dist = Vector3.Distance(transform.position, hit.transform.position);
                    if (dist < minDist)
                    {
                        minDist = dist;
                        target = hit;
                    }
                }
            }
            
            if (target == null) return; // Không có đối thủ, không attack
            if (target.CompareTag(Params.BotTag) && gameObject.CompareTag(Params.PlayerTag))
            {
                if (targetEnemyAI != null)
                {
                    targetEnemyAI.SetTargetOutlineActive(false);
                    targetEnemyAI = null;
                }
                        
                targetEnemyAI = target.GetComponent<EnemyAI>();
                targetEnemyAI.SetTargetOutlineActive(true);
            }
        }
        
        if (canAttack && attackCooldown <= 0f)
        {
            if (target == null) return; // Không có đối thủ, không attack
            
            playerTransform.LookAt(target.transform);
            
            if (animationController != null)
            {
                animationController.OnAttack += () => FireProjectile(target);
                        
                animationController.SetAttackAnimation();
            }
            
            // Nếu chỉ muốn kill 1 đối thủ/lần attack, break
            attackCooldown = maxAttackCooldown;
        }
        else
        {
            attackCooldown -= Time.deltaTime; // Giảm cooldown
            if(attackCooldown <= 0f)
            {
                weaponHandVisual.SetActive(true);
                attackCooldown = 0f; // Đảm bảo cooldown không âm
            }
        }
    }

    public void SetCanAttack(bool can)
    {
        canAttack = can;
    }

    public void FireProjectile(Collider collider)
    {
        weaponHandVisual.SetActive(false);
        
        // Tính hướng ném
        Vector3 direction = new Vector3(collider.transform.position.x, 0, collider.transform.position.z) - new Vector3(throwOrigin.position.x, 0, throwOrigin.position.z);
        Vector3 dir = direction.normalized;

        // Tạo projectile
        GameObject projectile = PoolManager.Instance.GetObj(currentWeapon.modelPrefab);
        projectile.transform.position = throwOrigin.position;
        projectile.transform.SetParent(weaponInstantiateTransform);
        WeaponProjectile weaponProjectile = projectile.GetComponent<WeaponProjectile>();
        weaponProjectile.Launch(dir, targetLayer, currentWeapon);
    }

    public void ChangeWeapon(WeaponData newWeapon)
    {
        currentWeapon = newWeapon;
        Vector3 weaponHandPosition = weaponHandVisual.gameObject.transform.position;
        Quaternion weaponHandRotation = weaponHandVisual.gameObject.transform.rotation;
        Destroy(weaponHandVisual.gameObject);
        weaponHandVisual = Instantiate(currentWeapon.visual, 
            weaponHandPosition, 
            weaponHandRotation, 
            handHoldWeaponTransform);
        weaponHandVisual.SetActive(true);
    }
    
    public float GetAttackCooldown()
    {
        return attackCooldown;
    }
    
    public float GetAttackRadius()
    {
        return attackRadius;
    }
    
    public LayerMask GetTargetLayer()
    {
        return targetLayer;
    }
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRadius);
    }
}