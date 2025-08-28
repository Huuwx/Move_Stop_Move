using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class Data
{
    [Header("Player Data")]
    [SerializeField] List<string> keys = new();
    [SerializeField] List<string> values = new();

    [Header("Game Data")] 
    [SerializeField] private int currentCoin = 1000;
    [SerializeField] private int selectedWeaponInShop = 0;
    [SerializeField] private int currentLevel = 0;
    [SerializeField] private int best = 1;

    [Header("Can Upgarade Ability data")] 
    [SerializeField] private int shieldCount = 0;
    [SerializeField] private int shieldPrice = 1000;
    [SerializeField] private int speedPercent = 0;
    [SerializeField] private int speedPrice = 250;
    [SerializeField] private int rangePercent = 0;
    [SerializeField] private int rangePrice = 250;
    [SerializeField] private int bulletMax = 2;
    [SerializeField] private int bulletPrice = 500;

    public int GetCurrentCoin()
    {
        return currentCoin;
    }

    public void SetCurrentCoin(int currentCoin)
    {
        this.currentCoin = currentCoin;
    }

    public int GetSelectedWeaponInShop()
    {
        return selectedWeaponInShop;
    }

    public void SetSelectedWeaponInShop(int selectedWeaponInShop)
    {
        this.selectedWeaponInShop = selectedWeaponInShop;
    }

    public void SetCurrentLevel(int currentLevel)
    {
        this.currentLevel = currentLevel;
    }

    public int GetCurrentLevel()
    {
        return currentLevel;
    }
    
    public void AddKeyValue(string key, string value)
    {
        if (!keys.Contains(key))
        {
            keys.Add(key);
            values.Add(value);
        }
        else
        {
            int index = keys.IndexOf(key);
            values[index] = value;
        }
    }
    public string GetValueByKey(string key)
    {
        if (keys.Contains(key))
        {
            int index = keys.IndexOf(key);
            return values[index];
        }
        return null;
    }
    public void RemoveKey(string key)
    {
        if (keys.Contains(key))
        {
            int index = keys.IndexOf(key);
            keys.RemoveAt(index);
            values.RemoveAt(index);
        }
    }
    public int GetBestRank()
    {
        return best;
    }
    public void SetBestRank(int bestRank)
    {
        this.best = bestRank;
    }
    public int GetShieldCount()
    {
        return shieldCount;
    }

    public void SetShieldCount(int shieldCount)
    {
        this.shieldCount = shieldCount;
    }
    public int GetShieldPrice()
    {
        return shieldPrice;
    }
    public void SetShieldPrice(int shieldPrice)
    {
        this.shieldPrice = shieldPrice;
    }

    public int GetSpeedPercent()
    {
        return speedPercent;
    }
    public void SetSpeedPercent(int speedPercent)
    {
        this.speedPercent = speedPercent;
    }

    public int GetSpeedPrice()
    {
        return speedPrice;
    }
    public void SetSpeedPrice(int speedPrice)
    {
        this.speedPrice = speedPrice;
    }
    public int GetRangePercent()
    {
        return rangePercent;
    }

    public void SetRangePercent(int rangePercent)
    {
        this.rangePercent = rangePercent;
    }
    public int GetRangePrice()
    {
        return rangePrice;
    }

    public void SetRangePrice(int rangePrice)
    {
        this.rangePrice = rangePrice;
    }
    public int GetBulletMax()
    {
        return bulletMax;
    }
    public void SetBulletMax(int bulletMax)
    {
        this.bulletMax = bulletMax;
    }

    public int GetBulletPrice()
    {
        return bulletPrice;
    }
    public void SetBulletPrice(int bulletPrice)
    {
        this.bulletPrice = bulletPrice;
    }
}