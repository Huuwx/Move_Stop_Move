using System;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class EnemyAI : EnemyBase
{
    [Header("Variables")]
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float wanderChangeDirTime = 2.5f;
    public int points = 0;

    [Header("Refs")] 
    public SkinnedMeshRenderer enemySkin;
    [SerializeField] AnimationController animationController;
    [SerializeField] WeaponAttack weaponAttack;
    [SerializeField] TextMeshProUGUI pointsText; // Hiển thị điểm của người chơi
    [SerializeField] private GameObject ingameUI; // Giao diện trong game

    protected OffscreenIndicatorManager _mgr;
    private Vector3 wanderDir;
    private float moveTimer = 0f;
    private float wanderTimer;
    private EnemyState state = EnemyState.Run;

    public Action OnUpgarde;
    
    protected override void Awake()
    {
        base.Awake();
        rb.isKinematic = false; // Đảm bảo Rigidbody không bị kinematic
    }
    
    void OnEnable()
    {
        _mgr = FindObjectOfType<OffscreenIndicatorManager>();
        if (_mgr) _mgr.RegisterTarget(this);
        
        OnUpgarde += SetPoints; // Đăng ký sự kiện nâng cấp điểm
        EventObserver.OnGameStateChanged += setIngameUIActive;
    }
    void OnDisable()
    {
        if (_mgr) _mgr.UnregisterTarget(this);
        
        OnUpgarde -= SetPoints; // Hủy đăng ký sự kiện nâng cấp điểm
        EventObserver.OnGameStateChanged -= setIngameUIActive;
    }

    protected void Start()
    {
        animationController = GetComponentInChildren<AnimationController>();
        ChooseRandomDirection();
    }

    void Update()
    {
        if (state == EnemyState.Dead || 
            GameController.Instance.State == GameState.Home || 
            GameController.Instance.State == GameState.Shop || 
            GameController.Instance.State == GameState.Ready) return;

        // 1. Tìm mục tiêu gần nhất trong bán kính attack
        Collider[] hits = Physics.OverlapSphere(transform.position, weaponAttack.GetAttackRadius(), weaponAttack.GetTargetLayer());
        Transform target = null;

        if (weaponAttack.attackCount > 0)
        {
            foreach (Collider hit in hits)
            {
                if (hit.gameObject == gameObject) continue; // bỏ qua chính mình
                if (!hit.gameObject.activeSelf) continue; // bỏ qua đối thủ đã chết

                target = hit.transform;
                break;
            }

            if (target != null && weaponAttack.GetAttackCooldown() <= 0f)
            {
                // 2. Chuyển sang Attack (stop, nhìn mục tiêu)
                state = EnemyState.Attack;
                weaponAttack.SetCanAttack(true);
                moveTimer = 0f;
            }
        }
        else
        {
            if (animationController.IsPlayingUnStopAnimation || animationController.IsPlayingSpecialAnimation)
                return;
            
            // 3. Nếu không có đối thủ, quay lại Wander (di chuyển ngẫu nhiên)
            state = EnemyState.Idle;
            weaponAttack.SetCanAttack(false);

            wanderTimer -= Time.deltaTime;
            if (wanderTimer <= 0)
            {
                ChooseRandomDirection();
            }
            // Di chuyển theo hướng ngẫu nhiên
            if(moveTimer > 0)
            {
                state = EnemyState.Run;
                moveTimer -= Time.deltaTime;
                transform.Translate(wanderDir * moveSpeed * Time.deltaTime, Space.World);
                transform.forward = wanderDir;
                animationController.SetRunAnimation();
            }
            else
            {
                moveTimer = 0f;
                animationController.SetIdleAnimation();
                weaponAttack.attackCount = Random.Range(1, 3);
            }
        }
    }

    // Chọn hướng di chuyển ngẫu nhiên mới
    public void ChooseRandomDirection()
    {
        moveTimer = Random.Range(2f, 4f);
        float angle = Random.Range(0f, 360f);
        wanderDir = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)).normalized;
        wanderTimer = wanderChangeDirTime + Random.Range(-0.5f, 0.5f);
    }

    // --- Xử lý trạng thái của enemy ---
    public override void Die()
    {
        state = EnemyState.Dead;
        animationController.SetDeadAnimation();
        
        weaponAttack.SetCanAttack(false);
        rb.isKinematic = true;
        enemyCollider.enabled = false;
        
        if(spawnPointState != null)
            spawnPointState.state = SpawnState.Idle;
        
        //gameObject.SetActive(false);
    }

    public override void Reset()
    {
        state = EnemyState.Idle;
        animationController.SetIdleAnimation();
        weaponAttack.SetCanAttack(false);
        rb.isKinematic = false;
        enemyCollider.enabled = true; // Bật collider khi reset

        SetTargetOutlineActive(false);
        
        if(animationController != null)
            animationController.Reset();
        
        moveTimer = 0f;
        wanderTimer = 0f;
    }
    
    public void Ultimate()
    {
        if (weaponAttack.IsUltimate()) return; // Nếu đã là Ultimate thì không làm gì thêm
        
        transform.localScale += Vector3.one * Values.upgradeScale * 5;
        weaponAttack.UpgradeAttackRadius(Values.upgradeRadius * 5);
        weaponAttack.SetUltimate(true);
    }

    public void EndUltimate()
    {
        if (!weaponAttack.IsUltimate()) return;
        
        weaponAttack.SetUltimate(false);
        
        transform.localScale -= Vector3.one * Values.upgradeScale * 5;
        weaponAttack.UpgradeAttackRadius(-Values.upgradeRadius * 5);
    }
    
    // --- Xử lý sự kiện ---
    public void RaiseOnUpgradeEvent()
    {
        OnUpgarde?.Invoke();
    }
    
    public void SetPoints()
    {
        points += 1; // Tăng điểm mỗi khi người chơi giết được một đối thủ
        
        if (pointsText != null)
        {
            pointsText.text = points.ToString();
        }
        
        transform.localScale += Vector3.one * Values.upgradeScale; 
        weaponAttack.UpgradeAttackRadius(Values.upgradeRadius);
    }
    
    public void setIngameUIActive(GameState state)
    {
        if(state == GameState.Playing)
        {
            ingameUI.SetActive(true);
        }
        else
        {
            ingameUI.SetActive(false);
        }
    }
        
    private void OnCollisionStay(Collision other)
    {
        if (other.gameObject.CompareTag(Params.WallTag) || other.gameObject.CompareTag(Params.BotTag))
        {
            ChooseRandomDirection();
        }
    }
}
