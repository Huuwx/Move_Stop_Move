using System;
using UnityEngine;

public class WeaponAttack : MonoBehaviour
{
    [SerializeField] private float maxAttackCooldown = 5f;          // Thời gian cooldown
    [SerializeField] private float attackCooldown = 0f;           // Thời gian cooldown hiện tại
    [SerializeField] private float attackRadius = 2f;                // Bán kính vùng attack
    
    
    [SerializeField] private LayerMask targetLayer;                  // Layer của đối thủ (Bots hoặc Player)
    [SerializeField] private ParticleSystem attackEffect;            // Hiệu ứng attack (nếu có)
    [SerializeField] private AnimationController animationController;       // Animator, gọi animation attack
    [SerializeField] private WeaponProjectile projectilePrefab; // Nếu dùng projectile, kéo vào đây

    [SerializeField] private Transform weaponInstanceTransform;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Transform throwOrigin;
    

    private bool canAttack = false;                // Đang ở trạng thái "stop", sẵn sàng attack

    void Update()
    {
        if (canAttack && attackCooldown <= 0f)
        {
            Collider[] hits = Physics.OverlapSphere(transform.position, attackRadius, targetLayer);
            foreach (Collider hit in hits)
            {
                attackCooldown = maxAttackCooldown;
                // Nếu là đối thủ (có thể check thêm tag, hoặc component BotController)
                if (hit.CompareTag("Bot") || hit.CompareTag("Player"))
                {
                    playerTransform.LookAt(hit.transform);
                    
                    // Gây damage hoặc loại đối thủ (destroy/ẩn)
                    //Destroy(hit.gameObject);
                    
                    Debug.Log(hit.name + " trong tầm bị attack!");

                    // Gọi hiệu ứng
                    if (attackEffect != null) attackEffect.Play();

                    // Gọi animation (nếu có)
                    if (animationController != null) animationController.SetAttackAnimation();
                    
                    // Tính hướng ném
                    Vector3 direction = new Vector3(hit.transform.position.x, 0, hit.transform.position.z) - new Vector3(throwOrigin.position.x, 0, throwOrigin.position.z);
                    Vector3 dir = direction.normalized;

                    // Tạo projectile
                    var projectile = Instantiate(projectilePrefab, throwOrigin.position, Quaternion.identity, weaponInstanceTransform);
                    projectile.Launch(dir, targetLayer);

                    // Nếu chỉ muốn kill 1 đối thủ/lần attack, break
                    break;
                }
            }
        }
        else
        {
            attackCooldown -= Time.deltaTime; // Giảm cooldown
            if(attackCooldown < 0f)
            {
                attackCooldown = 0f; // Đảm bảo cooldown không âm
            }
        }
    }

    public void SetCanAttack(bool can)
    {
        canAttack = can;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRadius);
    }
}