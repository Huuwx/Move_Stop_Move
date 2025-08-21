using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class GiftSpawner : MonoBehaviour
{
    [Header("Variables")]
        [SerializeField] private int maxGiftCount = 6;         // Số bot tối đa tồn tại cùng lúc
        [SerializeField] private int totalSpawned = 0;
        
        [Header("Refs")]
        [SerializeField] Transform poolParent;          // Nơi chứa các enemy đã spawn (để quản lý dễ hơn)
        [SerializeField] GameObject giftPrefab;
        [SerializeField] private List<GameObject> spawnParent;
        [SerializeField] List<SpawnPointState> spawnPoints;        // Các vị trí spawn có thể (hoặc random trong vùng)
    
    
        private void OnEnable()
        {
            EventObserver.OnGiftCollected += TrySpawnOneAfterDelay;
        }
        private void OnDisable()
        {
            EventObserver.OnGiftCollected -= TrySpawnOneAfterDelay;
        }
    
        void Start()
        {
            spawnPoints =
                spawnParent[GameController.Instance.GetData().GetCurrentLevel()].gameObject.transform
                    .GetComponentsInChildren<SpawnPointState>().ToList();
            
             for (int i = 0; i < maxGiftCount; i++)
             {
                 SpawnGift();
             }
        }
    
        void SpawnGift()
        {
            // Chọn vị trí spawn ngẫu nhiên
            SpawnPointState spawnPointState;
            do
            {
                spawnPointState = spawnPoints[Random.Range(0, spawnPoints.Count - 1)];
            } while (spawnPointState.state == SpawnState.Spawned);
            spawnPointState.state = SpawnState.Spawned;
            GameObject giftObj = PoolManager.Instance.GetObj(giftPrefab);
            
            giftObj.transform.position = spawnPointState.gameObject.transform.position;
            giftObj.transform.rotation = Quaternion.identity; // Hoặc xoay theo hướng nào đó nếu cần
            giftObj.transform.SetParent(poolParent);
    
            
            var gift = giftObj.GetComponent<GiftSystem>();
            gift.SpawnPointState = spawnPointState;
            
    
            giftObj.SetActive(true); // Kích hoạt enemy
            
            totalSpawned++;
        }
    
        IEnumerator SpawnAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            
            SpawnGift();
        }
    
        public void TrySpawnOneAfterDelay()
        {
            StartCoroutine(SpawnAfterDelay(4f));
        }
}
