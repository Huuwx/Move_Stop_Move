using System;
using UnityEngine;

public class GiftSystem : MonoBehaviour
{
    private Rigidbody rb;
    public SpawnPointState SpawnPointState;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void Reset()
    {
        SpawnPointState.state = SpawnState.Idle;
    }

    private void OnCollisionEnter(Collision other)
    {
        PlayerController player = other.gameObject.GetComponent<PlayerController>();
        if (player != null)
        {
            player.Ultimate();
            
            EventObserver.RaiseOnGiftCollected();
            gameObject.SetActive(false);
            
            if(SpawnPointState)
                SpawnPointState.state = SpawnState.Idle;
            
            return;
        }
        EnemyAI enemy = other.gameObject.GetComponent<EnemyAI>();
        if (enemy != null)
        {
            Debug.Log("Enemy collected gift");
            
            enemy.Ultimate();
            
            EventObserver.RaiseOnGiftCollected();
            gameObject.SetActive(false);
            
            if(SpawnPointState)
                SpawnPointState.state = SpawnState.Idle;
            
            return;
        }
    }
}
