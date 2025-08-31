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
    [SerializeField] List<GameObject> enemyPrefabs;
    [SerializeField] private GameObject bossPrefab;
    [SerializeField] private List<GameObject> spawnParent;
    [SerializeField] List<SpawnPointState> spawnPoints;        // Các vị trí spawn có thể (hoặc random trong vùng)
    [SerializeField] private Color[] enemyColors;
    [SerializeField] private WeaponData[] weaponPrefabs;
    [SerializeField] private string[] enemyNames;

    
    public int TotalSpawned => totalSpawned; // Số lượng enemy đã spawn
    public int MaxAliveCount => maxAliveCount; // Số lượng enemy tối đa có thể tồn tại cùng lúc
    public int MaxSpawnCount => maxSpawnCount; // Tổng số enemy được phép spawn trong trận
    
    public event Action OnEnemySpawned;

    void Start()
    {
        if (GameController.Instance.mode == GameMode.Normal)
        {
            spawnPoints =
                spawnParent[GameController.Instance.GetData().GetCurrentLevel()].gameObject.transform
                    .GetComponentsInChildren<SpawnPointState>().ToList();
        }
        else
        {
            spawnPoints =
                spawnParent[0].gameObject.transform
                    .GetComponentsInChildren<SpawnPointState>().ToList();
        }
        
        //Spawn enemy ban đầu
         for (int i = 0; i < maxAliveCount && totalSpawned < maxSpawnCount; i++)
         {
             SpawnEnemy();
         }
    }

    // Xử lý sự kiện SpawnEnemy
    void SpawnEnemy()
    {
        // Chọn vị trí spawn ngẫu nhiên
        SpawnPointState spawnPointState;
        do
        {
            spawnPointState = spawnPoints[Random.Range(0, spawnPoints.Count - 1)];
        } while (spawnPointState.state == SpawnState.Spawned);
        spawnPointState.state = SpawnState.Spawned;
        if (totalSpawned == (maxSpawnCount / 2) && bossPrefab != null && GameController.Instance.mode == GameMode.Zombie)
        {
            GameObject boss = PoolManager.Instance.GetObj(bossPrefab);
            boss.transform.position = spawnPointState.gameObject.transform.position;
            boss.transform.rotation = Quaternion.identity; // Hoặc xoay theo hướng nào đó nếu cần
            boss.transform.SetParent(poolParent);
            var bossAi = boss.GetComponent<EnemyBase>();
            bossAi.spawnPointState = spawnPointState;
            bossAi.Reset();
            boss.SetActive(true); // Kích hoạt enemy
            totalSpawned++;
            return;
        }
        
        int randomEnemyIndex = Random.Range(0, enemyPrefabs.Count);
        GameObject enemyPrefab = enemyPrefabs[randomEnemyIndex];
        GameObject enemy = PoolManager.Instance.GetObj(enemyPrefab);
        
        // Gắn màu ngẫu nhiên
        Color randomColor = enemyColors[Random.Range(0, enemyColors.Length)];
        enemy.GetComponentInChildren<Renderer>().material.color = randomColor;
        
        // Gắn vũ khí ngẫu nhiên
        if (GameController.Instance.mode == GameMode.Normal)
        {
            int randomIndex = Random.Range(0, weaponPrefabs.Length);
            WeaponData weapon = Instantiate(weaponPrefabs[randomIndex]);
            EnemyAI enemyAI = enemy.GetComponent<EnemyAI>();
            enemyAI.GetWeaponAttack().SetCurrentWeapon(weapon);
            enemyAI.GetWeaponAttack().ChangeWeapon(weapon);
            enemyAI._mgr = FindObjectOfType<OffscreenIndicatorManager>();
            if (enemyAI._mgr) enemyAI._mgr.RegisterTarget(enemyAI);
            enemyAI.bgPointsText.color = randomColor;
            enemyAI.nameText.color = randomColor;
            
            // Gắn tên ngẫu nhiên
            string randomName = enemyNames[Random.Range(0, enemyNames.Length)];
            if (enemyAI.nameText != null)
            {
                enemyAI.nameText.text = randomName;
            }
        }

        if (totalSpawned > maxSpawnCount)
        {
            enemy.SetActive(false); // Nếu đã đạt maxSpawnCount, không kích hoạt enemy mới
            return;
        }
        
        enemy.transform.position = spawnPointState.gameObject.transform.position;
        enemy.transform.rotation = Quaternion.identity; // Hoặc xoay theo hướng nào đó nếu cần
        enemy.transform.SetParent(poolParent);

        
        var ai = enemy.GetComponent<EnemyBase>();
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
