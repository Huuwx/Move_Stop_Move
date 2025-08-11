using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class Data
{
    [SerializeField] private int currentCoin = 1000;
    [SerializeField] private int currentWeaponIndex = 0;
    [SerializeField] private int selectedWeaponInShop = 0;
    [SerializeField] private WeaponData currentWeaponData;
    [SerializeField] private WeaponData currentWeaponShopData;
    
    public int GetCurrentCoin() { return currentCoin;}
    public void SetCurrentCoin(int currentCoin) {  this.currentCoin = currentCoin; }
    public int GetCurrentWeaponIndex() { return currentWeaponIndex;}
    public void SetCurrentWeaponIndex(int currentWeaponIndex) {  this.currentWeaponIndex = currentWeaponIndex; }
    public int GetSelectedWeaponInShop() { return selectedWeaponInShop;}
    public void SetSelectedWeaponInShop(int selectedWeaponInShop) {  this.selectedWeaponInShop = selectedWeaponInShop; }
    public WeaponData GetCurrentWeaponData() { return currentWeaponData; }
    public void SetCurrentWeaponData(WeaponData currentWeaponData) {  this.currentWeaponData = currentWeaponData; }
    public WeaponData GetCurrentWeaponShopData() { return currentWeaponShopData; }
    public void SetCurrentWeaponShopData(WeaponData currentWeaponShopData) {  this.currentWeaponShopData = currentWeaponShopData; }
}