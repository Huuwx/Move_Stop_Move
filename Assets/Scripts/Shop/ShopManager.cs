using System;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    private int currentWeaponIndex = 0;
    [SerializeField] WeaponData currentWeaponShopData;
    [SerializeField] GameObject[] weaponModels;
    [SerializeField] WeaponData[] weaponDatas;
    [SerializeField] private ListWeapon listWeapon;

    void OnEnable()
    {
        var id = GameController.Instance.GetData().GetValueByKey(Params.WeaponKey);
        if (!string.IsNullOrEmpty(id))
        {
            currentWeaponShopData = listWeapon.GetWeaponById(id);
        }

        currentWeaponIndex = currentWeaponShopData.index;

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
        
        GameController.Instance.GetUIController().UpdateWeaponInfo(weaponDatas[currentWeaponIndex]);
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
        
        GameController.Instance.GetUIController().UpdateWeaponInfo(weaponDatas[currentWeaponIndex]);
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
            GameController.Instance.GetUIController().UpdateWeaponInfo(currentWeaponShopData);
            GameController.Instance.GetUIController().UpdateCoin();
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
        GameController.Instance.GetUIController().UpdateWeaponInfo(currentWeaponShopData);
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
            // GameController.Instance.GetData().GetCurrentWeaponData().isEquipped = false;
            // GameController.Instance.GetData().SetCurrentWeaponData(currentWeaponShopData);
            // currentWeaponShopData.isEquipped = true;

            foreach (var weapon in listWeapon.weaponList)
            {
                if(weapon.id == currentWeaponShopData.id)
                {
                    weapon.isEquipped = true;
                }
                else
                {
                    weapon.isEquipped = false;
                }
            }
            GameController.Instance.GetData().AddKeyValue(Params.WeaponKey, currentWeaponShopData.id);
            
            GameController.Instance.GetUIController().UpdateWeaponInfo(currentWeaponShopData);
            GameController.Instance.GetPlayer().GetWeaponAttack().ChangeWeapon(currentWeaponShopData);
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
