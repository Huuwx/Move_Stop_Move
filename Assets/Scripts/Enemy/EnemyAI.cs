using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float wanderChangeDirTime = 2.5f;
    
    [SerializeField] WeaponAttack weaponAttack;
    [SerializeField] AnimationController animationController;
    public SpawnPointState spawnPoint;

    private Vector3 wanderDir;
    private float moveTimer = 0f;
    private float wanderTimer;
    private EnemyState state = EnemyState.Run;
    
    //public event Action OnDie;
    
    public static event Action<EnemyAI> OnAnyEnemyDead;

    void Start()
    {
        ChooseRandomDirection();
        animationController = GetComponentInChildren<AnimationController>();
    }

    void Update()
    {
        if (state == EnemyState.Dead) return;

        // 1. Tìm mục tiêu gần nhất trong bán kính attack
        Collider[] hits = Physics.OverlapSphere(transform.position, weaponAttack.GetAttackRadius(), weaponAttack.GetTargetLayer());
        Transform closestTargetTransfrom = null;
        float minDist = float.MaxValue;

        foreach (Collider hit in hits)
        {
            if (hit.gameObject == gameObject) continue; // bỏ qua chính mình
            if (!hit.gameObject.activeSelf) continue;   // bỏ qua đối thủ đã chết

            float dist = Vector3.Distance(transform.position, hit.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                closestTargetTransfrom = hit.transform;
            }
        }

        if (closestTargetTransfrom != null && weaponAttack.GetAttackCooldown() <= 0f)
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
        if(spawnPoint != null)
            spawnPoint.state = SpawnState.Idle; // Đánh dấu spawn point đã chết
        
        //OnDie?.Invoke();
        OnAnyEnemyDead?.Invoke(this);
        // Ẩn bot, phát hiệu ứng, gọi về GameManager,...
        gameObject.SetActive(false);
    }

    public void Reset()
    {
        state = EnemyState.Run;
        
        if(animationController != null)
            animationController.Reset();
        
        moveTimer = 0f;
        wanderTimer = 0f;
    }
    
    // public bool HasListeners()
    // {
    //     return OnDie != null;
    // }
        
    private void OnCollisionStay(Collision other)
    {
        if (other.gameObject.CompareTag(Params.WallTag) || other.gameObject.CompareTag(Params.BotTag))
        {
            Debug.Log("Enemy hit a wall, changing direction");
            ChooseRandomDirection();
        }
    }
}
