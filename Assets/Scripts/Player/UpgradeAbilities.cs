using TMPro;
using UnityEngine;

public class UpgradeAbilities : MonoBehaviour
{
    [Header("UI Refs")]
    [SerializeField] private TextMeshProUGUI shieldCountText;
    [SerializeField] private TextMeshProUGUI speedPercentText;
    [SerializeField] private TextMeshProUGUI rangePercentText;
    [SerializeField] private TextMeshProUGUI bulletMaxText;
    [SerializeField] private TextMeshProUGUI shieldPriceText;
    [SerializeField] private TextMeshProUGUI speedPriceText;
    [SerializeField] private TextMeshProUGUI rangePriceText;
    [SerializeField] private TextMeshProUGUI bulletPriceText;
    
    [Header("Player Refs")]
    [SerializeField] private PlayerController playerController;
    
    [Header("Camera Refs")]
    [SerializeField] private CameraFollow cameraFollow;
    
    [Header("Abilities")]
    public int shieldCount = 0;
    public int speedPercent = 0;
    public int rangePercent = 0;
    
    void Start()
    {
        shieldCount = GameController.Instance.GetData().GetShieldCount();
        shieldCountText.text = shieldCount.ToString("F0") + " Time";
        speedPercent = GameController.Instance.GetData().GetSpeedPercent();
        speedPercentText.text = "+" + speedPercent.ToString("F0") + "% Speed";
        rangePercent = GameController.Instance.GetData().GetRangePercent();
        rangePercentText.text = "+" + rangePercent.ToString("F0") + "% Range";
        bulletMaxText.text = "Max: " + GameController.Instance.GetData().GetBulletMax();
        shieldPriceText.text = GameController.Instance.GetData().GetShieldPrice().ToString("F0");
        speedPriceText.text = GameController.Instance.GetData().GetSpeedPrice().ToString("F0");
        rangePriceText.text = GameController.Instance.GetData().GetRangePrice().ToString("F0");
        bulletPriceText.text = GameController.Instance.GetData().GetBulletPrice().ToString("F0");
        
        for(int i = 0; i < speedPercent / 10; i++)
        {
            playerController.UpgradeSpeed( + 0.5f);
        }

        for (int i = 0; i < rangePercent / 10; i++)
        {
            playerController.UpgradeRangeScale();
            cameraFollow.UpgradeOffset();
        }
        playerController.maxBullets = GameController.Instance.GetData().GetBulletMax();
    }
    
    public void AddShield()
    {
        shieldCount += 1;
        GameController.Instance.GetData().SetShieldCount(shieldCount);
        shieldCountText.text = shieldCount.ToString("F0") + " Time";
        GameController.Instance.SaveData();
        shieldPriceText.text = GameController.Instance.GetData().GetShieldPrice().ToString("F0");
    }
    
    public void UpgradeSpeed()
    {
        playerController.UpgradeSpeed( + 0.5f);
        speedPercent += 10; // Tăng 10%
        GameController.Instance.GetData().SetSpeedPercent(speedPercent);
        GameController.Instance.GetData().SetSpeedPrice(GameController.Instance.GetData().GetSpeedPrice() * 2);
        speedPercentText.text = "+" + speedPercent.ToString("F0") + "% Speed";
        GameController.Instance.SaveData();
        speedPriceText.text = GameController.Instance.GetData().GetSpeedPrice().ToString("F0");
    }
    public void UpgradeRange()
    {
        playerController.UpgradeRangeScale();
        cameraFollow.UpgradeOffset();
        rangePercent += 10; // Tăng 10%
        GameController.Instance.GetData().SetRangePercent(rangePercent);
        GameController.Instance.GetData().SetRangePrice(GameController.Instance.GetData().GetRangePrice() * 2);
        GameController.Instance.SaveData();
        rangePriceText.text = GameController.Instance.GetData().GetRangePrice().ToString("F0");
        rangePercentText.text = "+" + rangePercent.ToString("F0") + "% Range";
    }
    public void UpgradeBulletMax()
    {
        playerController.maxBullets += 1;
        GameController.Instance.GetData().SetBulletMax(playerController.maxBullets);
        GameController.Instance.GetData().SetBulletPrice(GameController.Instance.GetData().GetBulletPrice() * 2);
        GameController.Instance.SaveData();
        bulletPriceText.text = GameController.Instance.GetData().GetBulletPrice().ToString("F0");
        bulletMaxText.text = "Max: " + playerController.maxBullets.ToString("F0");
    }
}
