using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemySpawner : MonoBehaviour
{
    [Header("Variables")]
    [SerializeField] private int maxAliveCount = 6;         // Số bot tối đa tồn tại cùng lúc
    [SerializeField] private int maxSpawnCount = 10;         // Tổng số enemy được phép spawn trong trận
    [SerializeField] private int totalSpawned = 0;
    
    [Header("Refs")]
    [SerializeField] Transform poolParent;          // Nơi chứa các enemy đã spawn (để quản lý dễ hơn)
    [SerializeField] GameObject enemyPrefab;
    [SerializeField] private List<GameObject> spawnParent;
    [SerializeField] List<SpawnPointState> spawnPoints;        // Các vị trí spawn có thể (hoặc random trong vùng)
    
    public int TotalSpawned => totalSpawned; // Số lượng enemy đã spawn
    public int MaxAliveCount => maxAliveCount; // Số lượng enemy tối đa có thể tồn tại cùng lúc
    public int MaxSpawnCount => maxSpawnCount; // Tổng số enemy được phép spawn trong trận
    
    public event Action OnEnemySpawned;

    private void OnEnable()
    {
        spawnPoints = 
            spawnParent[GameController.Instance.GetData().GetCurrentLevel()].gameObject.transform.GetComponentsInChildren<SpawnPointState>().ToList();
    }

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
        SpawnPointState spawnPointState;
        do
        {
            spawnPointState = spawnPoints[Random.Range(0, spawnPoints.Count - 1)];
        } while (spawnPointState.state == SpawnState.Spawned);
        spawnPointState.state = SpawnState.Spawned;
        GameObject enemy = PoolManager.Instance.GetObj(enemyPrefab);

        if (totalSpawned > maxSpawnCount)
        {
            enemy.SetActive(false); // Nếu đã đạt maxSpawnCount, không kích hoạt enemy mới
            return;
        }
        
        enemy.transform.position = spawnPointState.gameObject.transform.position;
        enemy.transform.rotation = Quaternion.identity; // Hoặc xoay theo hướng nào đó nếu cần
        enemy.transform.SetParent(poolParent);
        
        var ai = enemy.GetComponent<EnemyAI>();
        ai.spawnPointState = spawnPointState;
        ai.Reset();
        
        enemy.SetActive(true); // Kích hoạt enemy
        
        totalSpawned++;
    }

    IEnumerator SpawnAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        
        SpawnEnemy();
        
        OnEnemySpawned?.Invoke();
    }

    public void TrySpawnOneAfterDelay(float delay)
    {
        StartCoroutine(SpawnAfterDelay(delay));
    }


    // private void OnDrawGizmosSelected()
    // {
    //     spawnPoints = transform.GetComponentsInChildren<SpawnPointState>().ToList();
    // }
}
