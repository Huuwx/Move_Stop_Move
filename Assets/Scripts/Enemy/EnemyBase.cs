using System;
using UnityEngine;

public abstract class EnemyBase : MonoBehaviour
{
    [Header("Refs Base")]
    public SpawnPointState spawnPointState;
    [SerializeField] protected Collider enemyCollider;
    [SerializeField] protected GameObject targetOutline;
    
    protected Rigidbody rb;
    
    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if(enemyCollider == null)
            enemyCollider = GetComponent<Collider>();
    }

    public void SetTargetOutlineActive(bool isActive)
    {
        if (targetOutline != null)
        {
            targetOutline.SetActive(isActive);
        }
    }
    
    public void TriggerDeadEvent()
    {
        EventObserver.RaiseOnAnyEnemyDead(this);
    }

    public abstract void Die();
    public abstract void Reset();
}
