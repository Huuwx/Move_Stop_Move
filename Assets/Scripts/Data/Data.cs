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
}