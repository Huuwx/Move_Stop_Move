using System;
using UnityEngine;

public class WeaponAttack : MonoBehaviour
{
    [SerializeField] private WeaponData currentWeapon;      //Vũ khí hiện tại
    [SerializeField] private GameObject weaponHandVisual;
    [SerializeField] private Transform handHoldWeaponTransform;
    
    [SerializeField] private float maxAttackCooldown = 5f;          // Thời gian cooldown
    private float attackCooldown = 0f;           // Thời gian cooldown hiện tại
    [SerializeField] private float attackRadius = 7f;                // Bán kính vùng attack
    
    
    [SerializeField] private LayerMask targetLayer;                  // Layer của đối thủ (Bots hoặc Player)
    [SerializeField] private AnimationController animationController;       // Animator, gọi animation attack

    [SerializeField] private Transform weaponInstantiateTransform;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Transform throwOrigin;
    

    private bool canAttack = false;                // Đang ở trạng thái "stop", sẵn sàng attack

    private void Awake()
    {
        attackCooldown = 0f;
        canAttack = false;
    }

    void Update()
    {
        if (canAttack && attackCooldown <= 0f)
        {
            Collider[] hits = Physics.OverlapSphere(transform.position, attackRadius, targetLayer);
            foreach (Collider hit in hits)
            {
                // Nếu là đối thủ (có thể check thêm tag, hoặc component BotController)
                if (hit.CompareTag("Bot") || hit.CompareTag("Player"))
                {
                    playerTransform.LookAt(hit.transform);
                    
                    // Gây damage hoặc loại đối thủ (destroy/ẩn)
                    //Destroy(hit.gameObject);
                    
                    Debug.Log(hit.name + " bị attack!");

                    if (animationController != null)
                    {
                        animationController.OnAttack += () => FireProjectile(hit);
                        
                        animationController.SetAttackAnimation();
                    }

                    // Nếu chỉ muốn kill 1 đối thủ/lần attack, break
                    attackCooldown = maxAttackCooldown;
                    break;
                }
            }
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

        // var projectile = Instantiate(currentWeapon.modelPrefab.GetComponent<WeaponProjectile>(), throwOrigin.position, currentWeapon.modelPrefab.transform.rotation, weaponInstanceTransform);
        // projectile.Launch(dir, targetLayer, currentWeapon);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRadius);
    }
}