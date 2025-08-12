using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class Data
{
    [Header("Player Data")] [SerializeField]
    private WeaponData currentWeaponData;

    [Header("Weapon Shop Data")] [SerializeField]
    private int currentWeaponShopIndex = 0;

    [Header("Game Data")] [SerializeField] private int currentCoin = 1000;
    [SerializeField] private int selectedWeaponInShop = 0;

    public int GetCurrentWeaponShopIndex()
    {
        return currentWeaponShopIndex;
    }

    public void SetCurrentWeaponShopIndex(int currentWeaponShopIndex)
    {
        this.currentWeaponShopIndex = currentWeaponShopIndex;
    }

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

    public WeaponData GetCurrentWeaponData()
    {
        return currentWeaponData;
    }

    public void SetCurrentWeaponData(WeaponData currentWeaponData)
    {
        this.currentWeaponData = currentWeaponData;
    }
}