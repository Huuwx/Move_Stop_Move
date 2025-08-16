using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class ZombieAI : EnemyBase
{
    [Header("Refs")]
    [SerializeField] Transform player;
    [SerializeField] Animator animator;

    [Header("Tuning")]
    [SerializeField] float stoppingDistance = 0.4f;
    [SerializeField] float retargetInterval = 0.2f; // giãn nhịp update destination
    [SerializeField] float detectionRadius = 999f;

    NavMeshAgent agent;
    float retargetTimer;
    bool isTouchingPlayer = true;
    

    public System.Action OnTouchPlayer; // gán từ GameManager nếu muốn

    protected void Awake()
    {
        base.Awake();
        agent = GetComponent<NavMeshAgent>();
        if (!animator) animator = GetComponentInChildren<Animator>();

        isTouchingPlayer = false;
    }

    public override void Die()
    {
        if(spawnPointState != null)
            spawnPointState.state = SpawnState.Idle;
        
        TriggerDeadEvent();
        
        gameObject.SetActive(false);
    }

    public override void Reset()
    {
        isTouchingPlayer = false;
    }

    void OnEnable()
    {
        if (!player)
        {
            var p = GameObject.FindGameObjectWithTag(Params.PlayerTag);
            if (p) player = p.transform;
        }
        EnsureOnNavMesh();
        agent.isStopped = false;
        agent.stoppingDistance = stoppingDistance;
    }

    void Update()
    {
        if (!player || !agent) return;

        if (!agent.isOnNavMesh)
        {
            EnsureOnNavMesh();
            return;
        }

        // Cập nhật đích thưa hơn
        retargetTimer -= Time.deltaTime;
        if (retargetTimer <= 0f)
        {
            retargetTimer = retargetInterval;
            if (Vector3.Distance(transform.position, player.position) <= detectionRadius)
                agent.SetDestination(player.position);
        }

        // Cập nhật Animator từ tốc độ thực
        if (animator && !isTouchingPlayer)
            animator.SetBool("Start", true);
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
        if (OnTouchPlayer != null) OnTouchPlayer.Invoke();
        else Debug.Log("GameOver!");
        
        if (animator)
        {
            animator.SetBool("Start", false);
        }
        
        agent.isStopped = true;
        
        isTouchingPlayer = true;
    }
    
    void OnCollisionEnter(Collision other)
    {
        if (other.collider.CompareTag(Params.PlayerTag))
            TouchPlayer();
    }
}
