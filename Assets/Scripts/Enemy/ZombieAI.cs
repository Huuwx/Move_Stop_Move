using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class ZombieAI : EnemyBase
{
    [Header("Refs")]
    [SerializeField] Transform player;
    [SerializeField] Animator animator;
    [SerializeField] private GameObject hittedEffect;

    [Header("Tuning")]
    [SerializeField] float stoppingDistance = 0.4f;
    [SerializeField] float retargetInterval = 0.2f; // giãn nhịp update destination
    [SerializeField] float detectionRadius = 999f;
    
    NavMeshAgent agent;
    float retargetTimer;
    bool isTouchingPlayer = true;

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
    }

    void Update()
    {
        if (GameController.Instance.State != GameState.Playing)
        {
            agent.isStopped = true;
            animator.SetBool("Start", false);
            return;
        }
        
        if (!player || !agent) return;

        if (!agent.isOnNavMesh)
        {
            EnsureOnNavMesh();
            return;
        }
        
        agent.isStopped = false;

        // Cập nhật đích thưa hơn
        retargetTimer -= Time.deltaTime;
        if (retargetTimer <= 0f)
        {
            retargetTimer = retargetInterval;
            if (Vector3.Distance(transform.position, player.position) <= detectionRadius)
                agent.SetDestination(player.position);
        }

        // Cập nhật Animator từ tốc độ thực
        if (animator)
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
        if (animator)
        {
            animator.SetBool("Start", false);
        }
        
        agent.isStopped = true;
        
        //isTouchingPlayer = true;
    }
    
    public override void Die()
    {
        //rb.isKinematic = true;
        enemyCollider.enabled = false;
        
        //GameObject effect = PoolManager.Instance.GetObj(hittedEffect);
        GameObject effect = Instantiate(hittedEffect);
        effect.transform.SetParent(PoolManager.Instance.transform);
        effect.transform.rotation = Quaternion.identity;
        effect.transform.position = new Vector3(transform.position.x, 41f, transform.position.z);
        effect.GetComponent<ParticleSystem>().Play();
        
        if(spawnPointState != null)
            spawnPointState.state = SpawnState.Idle;
        
        agent.isStopped = true;
        //isTouchingPlayer = true;

        TriggerDeadEvent();
        gameObject.SetActive(false);
    }

    public override void Reset()
    {
        //isTouchingPlayer = false;
        //rb.isKinematic = false;
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
}
