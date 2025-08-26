using System;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    private int currentWeaponIndex = 0;

    [Header("Skin UI")]
    [SerializeField] private WeaponSkinSelector _weaponSkinSelector; // panel điều khiển skin (đã có)
    [SerializeField] private WeaponSkinListUI _skinListUI;                 // grid preset (script ở trên)
    [SerializeField] private CustomSkinPanel _customSkinPanel; // gắn trong Inspector

    [Header("Data / Preview")]
    [SerializeField] private WeaponData currentWeaponShopData;
    [SerializeField] private GameObject[] weaponModels; // mỗi phần tử là model preview của 1 vũ khí trong shop
    [SerializeField] private WeaponData[] weaponDatas;  // khớp thứ tự với weaponModels
    [SerializeField] private ListWeapon listWeapon;

    void OnEnable()
    {
        var id = GameController.Instance.GetData().GetValueByKey(Params.WeaponKey);
        if (!string.IsNullOrEmpty(id))
            currentWeaponShopData = listWeapon.GetWeaponById(id);

        if (!currentWeaponShopData && weaponDatas != null && weaponDatas.Length > 0)
            currentWeaponShopData = weaponDatas[0];

        currentWeaponIndex = currentWeaponShopData.index;

        SetActiveWeaponModel(currentWeaponIndex, true);  // bật model hiện tại (các model khác tắt)
        GameController.Instance.GetUIController().UpdateWeaponInfo(weaponDatas[currentWeaponIndex]);

        // >>> NEW: đồng bộ UI skin cho vũ khí đang hiển thị
        RefreshSkinUIForCurrentWeapon();
    }

    private void OnDisable()
    {
        GameController.Instance.GetPlayer().GetWeaponAttack().ChangeWeapon(listWeapon.GetWeaponById(GameController.Instance.GetData().GetValueByKey(Params.WeaponKey)));
    }

    public void ChangeNext()
    {
        SetActiveWeaponModel(currentWeaponIndex, false);
        currentWeaponIndex++;
        if (currentWeaponIndex >= weaponModels.Length) currentWeaponIndex = 0;

        currentWeaponShopData = weaponDatas[currentWeaponIndex];
        SetActiveWeaponModel(currentWeaponIndex, true);

        GameController.Instance.GetUIController().UpdateWeaponInfo(currentWeaponShopData);
        GameController.Instance.SaveData();

        // >>> NEW: đổi vũ khí thì đổi luôn UI skin + áp skin đã lưu lên model preview
        RefreshSkinUIForCurrentWeapon();
    }

    public void ChangePrevious()
    {
        SetActiveWeaponModel(currentWeaponIndex, false);
        currentWeaponIndex--;
        if (currentWeaponIndex < 0) currentWeaponIndex = weaponModels.Length - 1;

        currentWeaponShopData = weaponDatas[currentWeaponIndex];
        SetActiveWeaponModel(currentWeaponIndex, true);

        GameController.Instance.GetUIController().UpdateWeaponInfo(currentWeaponShopData);
        GameController.Instance.SaveData();

        // >>> NEW
        RefreshSkinUIForCurrentWeapon();
    }

    public void BuyWeapon()
    {
        if (GameController.Instance.GetData().GetCurrentCoin() >= currentWeaponShopData.price)
        {
            GameController.Instance.GetData().SetCurrentCoin(
                GameController.Instance.GetData().GetCurrentCoin() - currentWeaponShopData.price);
            currentWeaponShopData.isPurchased = true;
            _skinListUI.gameObject.SetActive(true);
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
        _skinListUI.gameObject.SetActive(true);
        GameController.Instance.GetUIController().UpdateWeaponInfo(currentWeaponShopData);
        GameController.Instance.SaveData();
    }
    
    public void EquipWeapon()
    {
        if (currentWeaponShopData.isPurchased)
        {
            // if (currentWeaponShopData.isEquipped)
            // {
            //     Debug.Log("This weapon is already equipped.");
            //     return;
            // }
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

    // ----------------- helpers -----------------

    private void SetActiveWeaponModel(int index, bool active)
    {
        // tắt tất cả, chỉ bật model tại index
        for (int i = 0; i < weaponModels.Length; i++)
            weaponModels[i].SetActive(i == index && active);
    }

    // private void RefreshSkinUIForCurrentWeapon()
    // {
    //     var model = weaponModels[currentWeaponIndex];
    //     // Bảo đảm model preview có applier
    //     var applier = model.GetComponent<WeaponSkinApplier>();
    //     if (applier == null) applier = model.AddComponent<WeaponSkinApplier>();
    //
    //     // 1) Setup selector => áp ngay skin đã lưu (preset/custom) lên model preview
    //     if (_weaponSkinSelector)
    //         _weaponSkinSelector.Setup(currentWeaponShopData, applier);
    //
    //     // 2) Build lại grid preset đúng database skin của vũ khí này
    //     if (_skinListUI)
    //         _skinListUI.Build(currentWeaponShopData);
    // }
    
    private void RefreshSkinUIForCurrentWeapon()
    {
        var model = weaponModels[currentWeaponIndex];
        var applier = model.GetComponent<WeaponSkinApplier>() ?? model.AddComponent<WeaponSkinApplier>();

        // Preset selector + grid
        _weaponSkinSelector?.Setup(currentWeaponShopData, applier);
        if (!currentWeaponShopData.isPurchased)
        {
            _skinListUI.gameObject.SetActive(false);
            return;
        }
        _skinListUI.gameObject.SetActive(true);
        
        _skinListUI?.Build(currentWeaponShopData);

        // >>> NEW: mở panel Custom cho đúng vũ khí & đúng số part
        _customSkinPanel?.OpenFor(currentWeaponShopData, applier);
    }
}
