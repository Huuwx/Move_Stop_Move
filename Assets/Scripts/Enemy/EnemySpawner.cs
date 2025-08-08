using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] int maxAliveCount = 6;         // Số bot tối đa tồn tại cùng lúc
    public int maxSpawnCount = 10;         // Tổng số enemy được phép spawn trong trận
    public int totalSpawned = 0;
    
    [SerializeField] Transform poolParent;          // Nơi chứa các enemy đã spawn (để quản lý dễ hơn)
    [SerializeField] private GameController controller;
    [SerializeField] GameObject enemyPrefab;
    [SerializeField] List<SpawnPointState> spawnPoints;        // Các vị trí spawn có thể (hoặc random trong vùng)

    void Start()
    {
        //Spawn enemy ban đầu
         for (int i = 0; i < maxAliveCount && totalSpawned < maxSpawnCount; i++)
         {
             SpawnEnemy();
         }
    }

    void SpawnEnemy()
    {
        // Chọn vị trí spawn ngẫu nhiên
        SpawnPointState spawnPoint;
        do
        {
            spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Count - 1)];
        } while (spawnPoint.state == SpawnState.Spawned);
        spawnPoint.state = SpawnState.Spawned;
        GameObject enemy = PoolManager.Instance.GetObj(enemyPrefab);
        enemy.transform.position = spawnPoint.gameObject.transform.position;
        enemy.transform.rotation = Quaternion.identity; // Hoặc xoay theo hướng nào đó nếu cần
        enemy.transform.SetParent(poolParent);
        
        controller.RegisterAlive();

        // Đăng ký callback chết của enemy
        var ai = enemy.GetComponent<EnemyAI>();
        ai.spawnPoint = spawnPoint;
        // if(!ai.HasListeners())
        //     ai.OnDie += () => OnEnemyDead(enemy); // Đảm bảo không bị null nếu chưa đăng ký
        ai.Reset();
        
        enemy.SetActive(true); // Kích hoạt enemy
        
        totalSpawned++;
    }

    void OnEnemyDead(GameObject enemy)
    {
        //currentAlive--;

        // Spawn mới nếu chưa đủ tổng số lượng
        if (totalSpawned < maxSpawnCount)
        {
            // Có thể spawn sau 0.5s để tự nhiên hơn
            StartCoroutine(SpawnAfterDelay(1f));
        }
    }

    IEnumerator SpawnAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        
        SpawnEnemy();
        
        // if (currentAlive < maxAliveCount && totalSpawned < maxSpawnCount)
        // {
        //     SpawnEnemy();
        // }
    }
    
    public void Configure(GameController controller)
    {
        this.controller = controller;
    }

    public void TrySpawnOneAfterDelay(float delay)
    {
        StartCoroutine(SpawnAfterDelay(delay));
    }


    private void OnDrawGizmosSelected()
    {
        spawnPoints = transform.GetComponentsInChildren<SpawnPointState>().ToList();
    }
}
