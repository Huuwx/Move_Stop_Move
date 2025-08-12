using System;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class EnemyAI : MonoBehaviour
{
    [Header("Variables")]
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float wanderChangeDirTime = 2.5f;
    
    [Header("Refs")]
    [SerializeField] WeaponAttack weaponAttack;
    [SerializeField] AnimationController animationController;
    public SpawnPointState spawnPointState;
    [SerializeField] private Collider enemyCollider;
    [SerializeField] private GameObject targetOutline;
    
    private Rigidbody rb;

    private Vector3 wanderDir;
    private float moveTimer = 0f;
    private float wanderTimer;
    private EnemyState state = EnemyState.Run;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if(enemyCollider == null)
            enemyCollider = GetComponent<Collider>();
        
        rb.isKinematic = false; // Đảm bảo Rigidbody không bị kinematic
    }

    private void Start()
    {
        animationController = GetComponentInChildren<AnimationController>();
        ChooseRandomDirection();
    }

    void Update()
    {
        if (state == EnemyState.Dead || GameController.Instance.State == GameState.Home) return;

        // 1. Tìm mục tiêu gần nhất trong bán kính attack
        Collider[] hits = Physics.OverlapSphere(transform.position, weaponAttack.GetAttackRadius(), weaponAttack.GetTargetLayer());
        Transform target = null;

        foreach (Collider hit in hits)
        {
            if (hit.gameObject == gameObject) continue; // bỏ qua chính mình
            if (!hit.gameObject.activeSelf) continue;   // bỏ qua đối thủ đã chết
            
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
            }
        }
    }

    void ChooseRandomDirection()
    {
        moveTimer = Random.Range(2f, 4f);
        float angle = Random.Range(0f, 360f);
        wanderDir = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)).normalized;
        wanderTimer = wanderChangeDirTime + Random.Range(-0.5f, 0.5f);
    }

    public void Die()
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

    public void Reset()
    {
        state = EnemyState.Idle;
        animationController.SetIdleAnimation();
        weaponAttack.SetCanAttack(false);
        rb.isKinematic = false;
        enemyCollider.enabled = true; // Bật collider khi reset
        
        if(animationController != null)
            animationController.Reset();
        
        moveTimer = 0f;
        wanderTimer = 0f;
    }

    public void TriggerDeadEvent()
    {
        EventObserver.RaiseOnAnyEnemyDead(this);
    }
    
    public void SetTargetOutlineActive(bool isActive)
    {
        if (targetOutline != null)
        {
            targetOutline.SetActive(isActive);
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
