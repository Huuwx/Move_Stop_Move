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
    
    [Header("Reference Menu UI")]
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private Animator menuAnimator;
    [SerializeField] TextMeshProUGUI txtAlive;
    [SerializeField] TextMeshProUGUI txtCoin;

    [Header("Reference Shop UI")]
    [SerializeField] private GameObject weaponsHolder;
    [SerializeField] private GameObject uiShopPanel;
    [SerializeField] private Button buyButton;
    [SerializeField] private Button equipButton;
    [SerializeField] private TextMeshProUGUI txtEquipButtonText;
    [SerializeField] private TextMeshProUGUI txtPrice;
    [SerializeField] private TextMeshProUGUI txtWeaponName;
    [SerializeField] private TextMeshProUGUI txtDescription;
    [SerializeField] TextMeshProUGUI txtShopCoin;

    private void Start()
    {
        if(uiPanelGameComplete != null)
        {
            uiPanelGameComplete.SetActive(false);
        }
        
        UpdateCoin();
    }

    private void OnEnable()
    {
        // Đăng ký lắng nghe sự kiện từ EventObserver
        EventObserver.OnAliveChanged += UpdateAliveCount;
        EventObserver.OnGameStateChanged += CompleteGame;
        EventObserver.OnGameStateChanged += OnClickMenu;
    }
    
    private void OnDestroy()
    {
        // Hủy đăng ký lắng nghe sự kiện khi đối tượng bị hủy
        EventObserver.OnAliveChanged -= UpdateAliveCount;
        EventObserver.OnGameStateChanged -= CompleteGame;
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
    
    private void CompleteGame(GameState state)
    {
        if (state == GameState.Win)
        {
            winIcon.SetActive(true);
            loseIcon.SetActive(false);
            txtRank.text = GameController.Instance.Alive.ToString();
            txtNotify.text = "You Win!";
        } else if (state == GameState.Lose)
        {
            winIcon.SetActive(false);
            loseIcon.SetActive(true);
            txtRank.text = GameController.Instance.Alive.ToString();
            txtNotify.text = "You Lose!";
        }
        else
        {
            joystick.SetActive(true);
            return;
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
    private void UpdateCoinCount(int coin)
    {
        if (txtCoin != null)
        {
            txtCoin.text = coin.ToString();
        }
        if (txtShopCoin != null)
        {
            txtShopCoin.text = coin.ToString();
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
    public void OpenShop()
    {
        GameController.Instance.player.gameObject.SetActive(false);
        
        if (uiShopPanel != null)
        {
            weaponsHolder.SetActive(true);
            menuPanel.SetActive(false);
            uiShopPanel.SetActive(true);
            UpdateWeaponInfo();
        }
        
        UpdateCoin();
    }
    
    
    // Shop UI
    public void CloseShop()
    {
        GameController.Instance.player.gameObject.SetActive(true);
        
        if (uiShopPanel != null)
        {
            weaponsHolder.SetActive(false);
            menuPanel.SetActive(true);
            uiShopPanel.SetActive(false);
        }
        
        UpdateCoin();
    }
    
    public void UpdateWeaponInfo()
    {
        WeaponData currentWeaponData = GameController.Instance.GetData().GetCurrentWeaponShopData();

        if (currentWeaponData.isPurchased)
        {
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
}
