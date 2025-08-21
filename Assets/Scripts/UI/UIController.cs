using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [Header("Reference In Game UI")] 
    [SerializeField] private GameObject uiPanelGameComplete;
    [SerializeField] private GameObject winIcon;
    [SerializeField] private GameObject loseIcon;
    [SerializeField] private TextMeshProUGUI txtRank;
    [SerializeField] private TextMeshProUGUI txtNotify;
    [SerializeField] private TextMeshProUGUI txtCoinClaimed;
    [SerializeField] private GameObject joystick;
    [SerializeField] private GameObject inGameUI;
    [SerializeField] private WardrobeManager wardrobeManager;
    [SerializeField] private GameObject pauseGamePanel;
    
    [Header("Reference ZombieMode UI")]
    [SerializeField] private GameObject timeCounterPanel;
    [SerializeField] private TextMeshProUGUI timeCounterTxt;
    
    [Header("Reference Menu UI")]
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private Animator menuAnimator;
    [SerializeField] TextMeshProUGUI txtAlive;
    [SerializeField] TextMeshProUGUI txtCoin;
    [SerializeField] private TextMeshProUGUI txtZone;
    [SerializeField] private TextMeshProUGUI txtBest;

    [Header("Reference Shop UI")]
    [SerializeField] private GameObject weaponsHolder;
    [SerializeField] private GameObject uiShopPanel;
    [SerializeField] private Button buyButton;
    [SerializeField] private Button equipButton;
    [SerializeField] private Button watchAdsButton;
    [SerializeField] private TextMeshProUGUI txtEquipButtonText;
    [SerializeField] private TextMeshProUGUI txtPrice;
    [SerializeField] private TextMeshProUGUI txtWeaponName;
    [SerializeField] private TextMeshProUGUI txtDescription;
    [SerializeField] TextMeshProUGUI txtShopCoin;
    
    [Header("Reference Skin Shop")]
    [SerializeField] private GameObject skinShopPanel;
    [SerializeField] TextMeshProUGUI txtSkinShopCoin;

    private void Start()
    {
        if(uiPanelGameComplete != null)
        {
            uiPanelGameComplete.SetActive(false);
        }
        
        UpdateCoin();
        
        if(txtZone != null)
        {
            txtZone.text = "ZONE: " + (GameController.Instance.GetData().GetCurrentLevel() + 1).ToString();
        }
        
        if(txtBest != null)
        {
            txtBest.text = "BEST: #" + GameController.Instance.GetData().GetBestRank().ToString();
        }
    }

    private void OnEnable()
    {
        // Đăng ký lắng nghe sự kiện từ EventObserver
        EventObserver.OnAliveChanged += UpdateAliveCount;
        EventObserver.OnGameStateChanged += ChangeStateEvent;
        EventObserver.OnGameStateChanged += OnClickMenu;
    }
    
    private void OnDestroy()
    {
        // Hủy đăng ký lắng nghe sự kiện khi đối tượng bị hủy
        EventObserver.OnAliveChanged -= UpdateAliveCount;
        EventObserver.OnGameStateChanged -= ChangeStateEvent;
        EventObserver.OnGameStateChanged -= OnClickMenu;
    }
    
    // In Game UI
    private void UpdateAliveCount(int alive)
    {
        if (txtAlive != null)
        {
            txtAlive.text = alive.ToString();
        }
    }
    
    private void ChangeStateEvent(GameState state)
    {
        if (state == GameState.Paused) return;
        
        if(state == GameState.Home || state == GameState.Shop)
        {
            if (uiPanelGameComplete != null)
            {
                uiPanelGameComplete.SetActive(false);
            }
            joystick.SetActive(false);
            winIcon.SetActive(false);
            loseIcon.SetActive(false);
            return;
        }
        
        if (state == GameState.Playing)
        {
            joystick.SetActive(true);
            return;
        }
        
        if (state == GameState.Win)
        {
            winIcon.SetActive(true);
            loseIcon.SetActive(false);
            txtRank.text = GameController.Instance.Alive.ToString();
            txtNotify.text = "You Win!";
            GameController.Instance.GetData().SetBestRank(0);
            GameController.Instance.SaveData();
        } else if (state == GameState.Lose)
        {
            winIcon.SetActive(false);
            loseIcon.SetActive(true);
            txtRank.text = GameController.Instance.Alive.ToString();
            txtNotify.text = "You Lose!";
            if((GameController.Instance.Alive < GameController.Instance.GetData().GetBestRank() || GameController.Instance.GetData().GetBestRank() == 0) && GameController.Instance.mode == GameMode.Normal)
            {
                GameController.Instance.GetData().SetBestRank(GameController.Instance.Alive);
                GameController.Instance.SaveData();
            }
        }
        
        if (txtCoinClaimed != null)
        {
            txtCoinClaimed.text = GameController.Instance.coinCollected.ToString();
        }
        
        GameController.Instance.GetData().SetCurrentCoin(GameController.Instance.GetData().GetCurrentCoin() + GameController.Instance.coinCollected);
        GameController.Instance.SaveData();
        
        joystick.SetActive(false);
        if (uiPanelGameComplete != null)
        {
            uiPanelGameComplete.SetActive(true);
        }
    }
    
    
    // Menu UI
    public void UpdateCoinCount(int coin)
    {
        if (txtCoin != null)
        {
            txtCoin.text = coin.ToString();
        }
        if (txtShopCoin != null)
        {
            txtShopCoin.text = coin.ToString();
        }
        if (txtSkinShopCoin != null)
        {
            txtSkinShopCoin.text = coin.ToString();
        }
    }
    private void OnClickMenu(GameState state)
    {
        if (menuAnimator != null && state == GameState.Playing)
        {
            menuAnimator.SetTrigger("Start");
            inGameUI.SetActive(true);
        }
    }
    public void OpenShop(int index)
    {
        menuPanel.SetActive(false);

        if (index == 0)
        {
            GameController.Instance.GetPlayer().gameObject.SetActive(false);
            if (uiShopPanel != null)
            {
                weaponsHolder.SetActive(true);
                uiShopPanel.SetActive(true);
                var id = GameController.Instance.GetData().GetValueByKey(Params.WeaponKey);
                if (!string.IsNullOrEmpty(id))
                {
                    var currentWeaponShopData = GameController.Instance.GetListWeapon().GetOutfitSetById(id);
                    UpdateWeaponInfo(currentWeaponShopData);
                }
            }
        } else if (index == 1)
        {
            GameController.Instance.SetState(GameState.Shop);
            if (skinShopPanel != null)
            {
                skinShopPanel.SetActive(true);
            }
        }

        UpdateCoin();
    }
    
    
    // Shop UI
    public void CloseShop(int index)
    {
        menuPanel.SetActive(true);

        if (index == 0)
        {
            GameController.Instance.GetPlayer().gameObject.SetActive(true);
            if (uiShopPanel != null)
            {
                weaponsHolder.SetActive(false);
                uiShopPanel.SetActive(false);
            }
        }
        else if (index == 1)
        {
            GameController.Instance.SetState(GameState.Home);
            if (skinShopPanel != null)
            {
                skinShopPanel.SetActive(false);
            }
            wardrobeManager.LoadFromSave();
        }

        UpdateCoin();
    }
    
    public void UpdateWeaponInfo(WeaponData currentWeaponData)
    {
        if (currentWeaponData.isPurchased)
        {
            watchAdsButton.gameObject.SetActive(false);
            buyButton.gameObject.SetActive(false);
            equipButton.gameObject.SetActive(true);
            if (currentWeaponData.isEquipped)
            {
                equipButton.interactable = false;
                txtEquipButtonText.text = "Equipped";
            }
            else
            {
                equipButton.interactable = true;
                txtEquipButtonText.text = "Equip";
            }
        }
        else
        {
            watchAdsButton.gameObject.SetActive(true);
            buyButton.gameObject.SetActive(true);
            equipButton.gameObject.SetActive(false);
        }
        
        if (txtWeaponName != null)
        {
            txtWeaponName.text = currentWeaponData.name;
        }
        
        if (txtPrice != null)
        {
            txtPrice.text = currentWeaponData.price.ToString();
        }
        
        if (txtDescription != null)
        {
            txtDescription.text = currentWeaponData.description;
        }
    }

    public void UpdateCoin()
    {
        UpdateCoinCount(GameController.Instance.GetData().GetCurrentCoin());
    }

    public void UpdateTimeCounter(int time)
    {
        timeCounterTxt.text = time.ToString();
        if (time <= 0)
        {
            SetActiveTimeCounterPanel(false);
        }
    }
    
    public void SetActiveTimeCounterPanel(bool active)
    {
        timeCounterPanel.SetActive(active);
    }

    public void DisplayPauseGamePanel(bool active)
    {
        pauseGamePanel.SetActive(active);
        inGameUI.SetActive(!active);
        joystick.SetActive(!active);
    }
}
