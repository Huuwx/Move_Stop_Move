using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class ZombieAI : EnemyBase
{
    public bool isBoss = false;
    
    [Header("Refs")]
    [SerializeField] Transform player;
    [SerializeField] Animator animator;
    [SerializeField] private GameObject hittedEffect;

    [Header("Tuning")]
    [SerializeField] float stoppingDistance = 0.4f;
    [SerializeField] float retargetInterval = 0.2f; // giãn nhịp update destination
    [SerializeField] float detectionRadius = 999f;

    // ====== Speed by distance ======
    [Header("Speed By Distance")]
    [SerializeField] float nearRadius = 3.0f;    // trong bán kính này: chạy chậm
    [SerializeField] float farRadius  = 12.0f;   // ngoài ~ bán kính này: chạy nhanh
    [SerializeField] float nearSpeed  = 4f;    // tốc độ khi gần
    [SerializeField] float farSpeed   = 6f;    // tốc độ khi xa
    [SerializeField] float speedSmooth = 8f;     // hệ số lerp mượt tốc độ
    [SerializeField] float nearAccel = 8f;       // gia tốc khi gần (mượt)
    [SerializeField] float farAccel  = 20f;      // gia tốc khi xa (bốc)
    [SerializeField] bool  clampY    = true;     // nếu game top-down, không tính chênh Y

    // Hysteresis nhẹ để tránh nhấp nháy khi đứng sát rìa
    [SerializeField] float hysteresis = 0.5f;    // cộng/trừ vào near/far khi xác định vùng

    NavMeshAgent agent;
    float retargetTimer;
    float targetSpeed;      // tốc độ mục tiêu theo khoảng cách
    bool isTouchingPlayer = true;
    private int BossHP = 7;

    protected void Awake()
    {
        base.Awake();
        agent = GetComponent<NavMeshAgent>();
        if (!animator) animator = GetComponentInChildren<Animator>();
    }

    void OnEnable()
    {
        if (!player)
        {
            var p = GameObject.FindGameObjectWithTag(Params.PlayerTag);
            if (p) player = p.transform;
        }
        EnsureOnNavMesh();
        agent.stoppingDistance = stoppingDistance;

        // Khởi tạo tốc độ ban đầu
        targetSpeed = nearSpeed;
        agent.speed = nearSpeed;
        agent.acceleration = nearAccel;
        agent.autoBraking = false; // để đỡ “phanh gấp” trước mục tiêu
    }

    void Update()
    {
        if (GameController.Instance.State != GameState.Playing)
        {
            agent.isStopped = true;
            if (animator) animator.SetBool("Start", false);
            return;
        }

        if (!player || !agent) return;

        if (!agent.isOnNavMesh)
        {
            EnsureOnNavMesh();
            return;
        }

        agent.isStopped = false;

        // === Cập nhật đích theo nhịp thưa ===
        retargetTimer -= Time.deltaTime;
        if (retargetTimer <= 0f)
        {
            retargetTimer = retargetInterval;
            if (Vector3.Distance(transform.position, player.position) <= detectionRadius)
                agent.SetDestination(player.position);
        }

        // === Tính khoảng cách & đặt tốc độ mục tiêu theo khoảng cách ===
        float dist = DistanceXZ(transform.position, player.position, clampY);

        // Nội suy mượt giữa nearSpeed và farSpeed theo khoảng cách
        // t = 0 (≤ nearRadius - hys) → nearSpeed
        // t = 1 (≥ farRadius  + hys) → farSpeed
        float t = Mathf.InverseLerp(nearRadius - hysteresis, farRadius + hysteresis, dist);
        float desiredSpeed = Mathf.Lerp(nearSpeed, farSpeed, t);
        float desiredAccel = Mathf.Lerp(nearAccel,  farAccel,  t);

        // Lerp theo thời gian để mượt
        targetSpeed = Mathf.Lerp(targetSpeed, desiredSpeed, speedSmooth * Time.deltaTime);
        agent.speed = targetSpeed;
        agent.acceleration = desiredAccel;

        // Cập nhật Animator (nếu có)
        if (animator)
        {
            animator.SetBool("Start", true);
            // Nếu bạn có tham số float "MoveSpeed" cho blend-tree:
            // animator.SetFloat("MoveSpeed", agent.velocity.magnitude);
        }
    }

    bool EnsureOnNavMesh()
    {
        if (agent.isOnNavMesh) return true;
        if (NavMesh.SamplePosition(transform.position, out var hit, 5f, NavMesh.AllAreas))
            return agent.Warp(hit.position);
        return false;
    }

    void TouchPlayer()
    {
        if (animator) animator.SetBool("Start", false);
        agent.isStopped = true;
        //isTouchingPlayer = true;
    }

    public override void Die()
    {
        if (isBoss)
        {
            BossHP--;
            if (BossHP > 0)
            {
                GameObject hitEffect = Instantiate(hittedEffect);
                hitEffect.transform.SetParent(PoolManager.Instance.transform);
                hitEffect.transform.rotation = Quaternion.identity;
                hitEffect.transform.position = new Vector3(transform.position.x, 41f, transform.position.z);
                hitEffect.GetComponent<ParticleSystem>().Play();
                return;
            }
        }
        
        enemyCollider.enabled = false;

        GameObject effect = Instantiate(hittedEffect);
        effect.transform.SetParent(PoolManager.Instance.transform);
        effect.transform.rotation = Quaternion.identity;
        effect.transform.position = new Vector3(transform.position.x, 41f, transform.position.z);
        effect.GetComponent<ParticleSystem>().Play();

        if (spawnPointState != null)
            spawnPointState.state = SpawnState.Idle;

        agent.isStopped = true;
        if(isBoss)
            TriggerDeadEvent(5);
        else
            TriggerDeadEvent(1);
        gameObject.SetActive(false);
    }

    public override void Reset()
    {
        //isTouchingPlayer = false;
        enemyCollider.enabled = true;
    }

    void OnCollisionEnter(Collision other)
    {
        PlayerController playerController = other.gameObject.GetComponent<PlayerController>();
        if (playerController)
        {
            TouchPlayer();
            playerController.Die();
        }
    }

    // ===== Helpers =====
    float DistanceXZ(Vector3 a, Vector3 b, bool removeY)
    {
        if (!removeY) return Vector3.Distance(a, b);
        a.y = 0f; b.y = 0f;
        return Vector3.Distance(a, b);
    }
}
