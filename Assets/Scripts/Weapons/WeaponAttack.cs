using System;
using System.Collections.Generic;
using UnityEngine;
using ZombieCity.Abilities;
using Random = UnityEngine.Random;

public class WeaponAttack : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private WeaponData currentWeapon;      //Vũ khí hiện tại
    [SerializeField] private GameObject weaponHandVisual;   // Hiển thị vũ khí trên tay
    [SerializeField] private Transform handHoldWeaponTransform; // Vị trí tay cầm vũ khí
    
    [SerializeField] private LayerMask targetLayer;                  // Layer của đối thủ (Bots hoặc Player)
    [SerializeField] private AnimationController animationController;       // Animator, gọi animation attack

    [SerializeField] private Transform weaponInstantiateTransform; // Vị trí instantiate vũ khí (để quản lý pool)
    [SerializeField] private Transform playerTransform; // Vị trí của Player (để nhìn về phía đối thủ)
    [SerializeField] private Transform throwOrigin; // Vị trí ném vũ khí
    
    [Header("Variables")]
    [SerializeField] private float maxAttackCooldown = 0.9f;          // Thời gian cooldown
    private float attackCooldown = 0f;           // Thời gian cooldown hiện tại
    [SerializeField] private float attackRadius = 7f;                // Bán kính vùng attack
    
    private bool ultimate = false;
    private bool canAttack = false;                // Đang ở trạng thái "stop", sẵn sàng attack
    private EnemyBase targetEnemy; // Đối thủ hiện tại (nếu có)
    private IShotPattern _pattern = new BasicForwardPattern();
    private PlayerStats _stats;
    private bool _homing;
    
    //enemy only
    public int attackCount = 1;

    private void Awake()
    {
        attackCooldown = 0f;
        canAttack = false;
        _stats = GetComponentInParent<PlayerStats>();
        
        attackCount = Random.Range(1, 3);
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

        var id = GameController.Instance.GetData().GetValueByKey(Params.WeaponKey);
        if (string.IsNullOrEmpty(id))
        {
            GameController.Instance.GetData().AddKeyValue(Params.WeaponKey, currentWeapon.id);
            GameController.Instance.SaveData();
        }
        else
        {
            var weapon = GameController.Instance.GetListWeapon().GetWeaponById(id);
            currentWeapon = weapon;
        }
        
        // if (GameController.Instance.GetData().GetCurrentWeaponData() == null)
        // {
        //     GameController.Instance.GetData().SetCurrentWeaponData(currentWeapon);
        //     GameController.Instance.SaveData();
        // }
        // else
        // {
        //     currentWeapon = GameController.Instance.GetData().GetCurrentWeaponData();
        // }
        
        ChangeWeapon(currentWeapon);
    }

    void Update()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, attackRadius, targetLayer);
        Collider target = null; // Biến để lưu đối thủ mục tiêu
        float minDist = float.MaxValue;
        
        if (hits.Length == 0)
        {
            if (targetEnemy != null)
            {
                targetEnemy.SetTargetOutlineActive(false);
                targetEnemy = null;
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
                if (targetEnemy != null)
                {
                    targetEnemy.SetTargetOutlineActive(false);
                    targetEnemy = null;
                }
                        
                targetEnemy = target.GetComponent<EnemyBase>();
                targetEnemy.SetTargetOutlineActive(true);
            }
        }
        
        if (canAttack && attackCooldown <= 0f)
        {
            if (target == null) return; // Không có đối thủ, không attack
            
            // Lấy vị trí mục tiêu
            Vector3 targetPos = target.transform.position;

            // Giữ nguyên độ cao (Y) của player => chỉ xoay trên mặt phẳng XZ
            targetPos.y = playerTransform.position.y;

            // Xoay player về phía mục tiêu
            playerTransform.LookAt(targetPos);

            
            //playerTransform.transform.rotation = Quaternion.LookRotation(target.bounds.center - playerTransform.transform.position, Vector3.up);
            
            if (animationController != null)
            {
                animationController.OnAttack += () => FireProjectile(target);

                if (!ultimate)
                {
                    animationController.SetAttackAnimation();
                }
                else
                {
                    animationController.SetUltiAnimation();
                }
            }
            
            if (_stats != null)
            {
                float fireRate = Mathf.Max(0.1f, _stats.Get(StatType.FireRate));
                maxAttackCooldown = 1f / fireRate;
            }
            
            // Nếu chỉ muốn kill 1 đối thủ/lần attack, break
            attackCooldown = maxAttackCooldown;
        }
        else
        {
            attackCooldown -= Time.deltaTime; // Giảm cooldown
            if (attackCooldown > 0f) return;
            
            weaponHandVisual.SetActive(true);
            attackCooldown = 0f; // Đảm bảo cooldown không âm

            // if(attackCooldown <= 0f)
            // {
            //     weaponHandVisual.SetActive(true);
            //     attackCooldown = 0f; // Đảm bảo cooldown không âm
            // }
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
        Vector2 forward2D = new(dir.x, dir.z);
        
        IShotPattern effective = _pattern;                       // dùng pattern hiện tại
        if (_stats != null)
        {
            int extraByStat = Mathf.Max(0, Mathf.RoundToInt(_stats.Get(StatType.ProjectileCount) - 1));
            if (effective is FanExtraProjectilesPattern)
            {
                // Nếu đã có FanExtraProjectilesPattern, cộng thêm vào số lượng hiện tại
                extraByStat += ((FanExtraProjectilesPattern)effective).GetExtraCount();
                effective = new FanExtraProjectilesPattern(new BasicForwardPattern(), extraByStat, 60f);
            }
            else
                if (extraByStat > 0) effective = new FanExtraProjectilesPattern(effective, extraByStat, 60f);
        }

        var dirs = effective.GetDirections(forward2D);

        // Tạo projectile
        foreach (var direct in dirs)
        {
            Vector3 direct3D = new Vector3(direct.x, 0, direct.y);
            GameObject projectile = null;
            if (gameObject.CompareTag(Params.BotTag))
                projectile =
                    PoolManager.Instance.GetObjByName(currentWeapon.modelPrefab.name + "(Clone)", currentWeapon.modelPrefab);
            else
                projectile = PoolManager.Instance.GetObj(currentWeapon.modelPrefab);
            
            var projApplier = projectile.GetComponentInChildren<WeaponSkinApplier>();
            if (projApplier)
            {
                string selectedId = currentWeapon.selectedSkinId;
        
                if(gameObject.CompareTag(Params.PlayerTag))
                    selectedId = WeaponSkinSave.LoadSelected(currentWeapon.id, currentWeapon.selectedSkinId);
                var db = currentWeapon.skins;
                var skin = db ? db.GetById(selectedId) : null;

                if (skin && skin.id != "custom")
                {
                    projApplier.ApplySkin(skin);
                }
                else
                    projApplier.ApplyCustomColors(WeaponSkinSave.LoadCustom(currentWeapon.id,
                        projApplier.MaterialCount));
                    
                projectile.name += "_" + skin.id;
            }
            
            // if (gameObject.CompareTag(Params.PlayerTag))
            // {
            //     
            // }
            // else
            // {
            //     if (projectile.name != currentWeapon.modelPrefab.name)
            //     {
            //         
            //     }
            // }
            projectile.transform.position = throwOrigin.position;
            
            // Sai vì dùng throwOrigin → hãy dùng từ chính viên đạn
            projectile.transform.rotation =
                Quaternion.LookRotation(collider.bounds.center - projectile.transform.position, Vector3.up);

        
            projectile.transform.SetParent(weaponInstantiateTransform);
            WeaponProjectile weaponProjectile = projectile.GetComponent<WeaponProjectile>();
            weaponProjectile.Launch(direct3D, targetLayer, currentWeapon, playerTransform.gameObject, ultimate, attackRadius + 1f);
            if (gameObject.CompareTag(Params.BotTag))
            {
                attackCount--;
                if (attackCount <= 0)
                {
                    EnemyAI enemyAI = GetComponentInParent<EnemyAI>();
                    if (enemyAI != null)
                        enemyAI.ChooseRandomDirection();
                }
            }
            
            
            if (ultimate)
            {
                if(gameObject.CompareTag(Params.PlayerTag))
                    GameController.Instance.GetPlayer().EndUltimate();
                else
                {
                    EnemyAI enemyAI = GetComponentInParent<EnemyAI>();
                    if (enemyAI != null)
                        enemyAI.EndUltimate();
                }
            }
        }
    }

    public void ChangeWeapon(WeaponData newWeapon)
    {
        currentWeapon = newWeapon;
        
        Vector3 weaponHandPosition = weaponHandVisual.transform.position;
        Quaternion weaponHandRotation = weaponHandVisual.transform.rotation;
        // if (newWeapon.isBoomerang)
        // {
        //     weaponHandPosition += new Vector3(-0.36f, 0.06f, 0f);
        //     weaponHandRotation = Quaternion.Euler(-180f, 0f, 114.05f);
        // }
        Destroy(weaponHandVisual);

        weaponHandVisual = Instantiate(currentWeapon.visual, weaponHandPosition, weaponHandRotation, handHoldWeaponTransform);

        if (newWeapon.isBoomerang)
        {
            weaponHandVisual.transform.localPosition = new Vector3(-0.3f, -0.05f, 0.04f);
            weaponHandVisual.transform.localRotation = Quaternion.Euler(0f, -180f, -257.1f);
        }
        else
        {
            weaponHandVisual.transform.localPosition = new Vector3(-0.05f, 0.1f, 0.04f);
            weaponHandVisual.transform.localRotation = Quaternion.Euler(-180f, 0f, -42.4f);
        }

        var applier = weaponHandVisual.GetComponent<WeaponSkinApplier>();
        if (applier == null) applier = weaponHandVisual.AddComponent<WeaponSkinApplier>();

        string selectedId = currentWeapon.selectedSkinId;
        
        if(gameObject.CompareTag(Params.PlayerTag))
            selectedId = WeaponSkinSave.LoadSelected(currentWeapon.id, currentWeapon.selectedSkinId);

        var db   = currentWeapon.skins;
        var skin = db ? db.GetById(selectedId) : null;

        if (skin && skin.id != "custom")
        {
            applier.ApplySkin(skin);
        }
        else
        {
            // Load màu custom theo số material của vũ khí
            var colors = WeaponSkinSave.LoadCustom(currentWeapon.id, applier.MaterialCount);
            applier.ApplyCustomColors(colors);
        }

        weaponHandVisual.SetActive(true);
    }

    
    public void UpgradeAttackRadius(float radius)
    {
        attackRadius += radius;
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
    public bool IsUltimate()
    {
        return ultimate;
    }
    public void SetUltimate(bool ultimate)
    {
        this.ultimate = ultimate;
    }
    public WeaponData GetCurrentWeapon()
    {
        return currentWeapon;
    }
    public void SetCurrentWeapon(WeaponData weapon)
    {
        currentWeapon = weapon;
    }
    
    public void SetHoming(bool on) => _homing = on;
    public void AddPatternDecorator(IShotPattern decorator) => _pattern = decorator;
    public void RemoveDecoratorById(string id) { /* chưa build stack ở bản đơn giản */ }
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRadius);
    }
}