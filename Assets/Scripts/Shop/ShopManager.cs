using System;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    
    private int currentWeaponIndex = 0;
    [SerializeField] WeaponData currentWeaponShopData;
    [SerializeField] GameObject[] weaponModels;
    [SerializeField] WeaponData[] weaponDatas;
    [SerializeField] UIController uiController;

    void OnEnable()
    {
        currentWeaponShopData = GameController.Instance.GetData().GetCurrentWeaponData();
        
        currentWeaponIndex = GameController.Instance.GetData().GetCurrentWeaponData().id;

        foreach (GameObject weapon in weaponModels)
        {
            weapon.SetActive(false);
        }
        
        weaponModels[currentWeaponIndex].SetActive(true);
    }

    public void ChangeNext()
    {
        weaponModels[currentWeaponIndex].SetActive(false);
        currentWeaponIndex++;
        if (currentWeaponIndex >= weaponModels.Length)
        {
            currentWeaponIndex = 0;
        }
        
        uiController.UpdateWeaponInfo(weaponDatas[currentWeaponIndex]);
        weaponModels[currentWeaponIndex].SetActive(true);
        currentWeaponShopData = weaponDatas[currentWeaponIndex];
        
        GameController.Instance.SaveData();
    }
    
    public void ChangePrevious()
    {
        weaponModels[currentWeaponIndex].SetActive(false);
        currentWeaponIndex--;
        if (currentWeaponIndex < 0)
        {
            currentWeaponIndex = weaponModels.Length - 1;
        }
        
        uiController.UpdateWeaponInfo(weaponDatas[currentWeaponIndex]);
        weaponModels[currentWeaponIndex].SetActive(true);
        currentWeaponShopData = weaponDatas[currentWeaponIndex];
        
        GameController.Instance.SaveData();
    }
    
    public void BuyWeapon()
    {
        if (GameController.Instance.GetData().GetCurrentCoin() >= currentWeaponShopData.price)
        {
            GameController.Instance.GetData().SetCurrentCoin(
                GameController.Instance.GetData().GetCurrentCoin() - currentWeaponShopData.price);
            currentWeaponShopData.isPurchased = true;
            uiController.UpdateWeaponInfo(currentWeaponShopData);
            uiController.UpdateCoin();
            GameController.Instance.SaveData();
        }
        else
        {
            Debug.Log("Not enough coins to buy this weapon.");
        }
    }

    public void WatchAds()
    {
        currentWeaponShopData.isPurchased = true;
        uiController.UpdateWeaponInfo(currentWeaponShopData);
        GameController.Instance.SaveData();
    }
    
    public void EquipWeapon()
    {
        if (currentWeaponShopData.isPurchased)
        {
            if (currentWeaponShopData.isEquipped)
            {
                Debug.Log("This weapon is already equipped.");
                return;
            }
            GameController.Instance.GetData().GetCurrentWeaponData().isEquipped = false;
            GameController.Instance.GetData().SetCurrentWeaponData(currentWeaponShopData);
            currentWeaponShopData.isEquipped = true;
            uiController.UpdateWeaponInfo(currentWeaponShopData);
            GameController.Instance.player.GetWeaponAttack().ChangeWeapon(currentWeaponShopData);
            GameController.Instance.SaveData();
        }
        else
        {
            Debug.Log("You need to buy this weapon first.");
        }
    }

    private void OnDrawGizmosSelected()
    {
        //weaponModels = transform.GetComponentsInChildren<GameObject>();
    }
}
