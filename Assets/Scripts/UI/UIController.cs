using TMPro;
using UnityEngine;

public class UIController : MonoBehaviour
{
    [Header("Reference UI Panels")]
    [SerializeField] GameObject uiReadyPanel;
    [SerializeField] GameObject uiHudPanel;
    [SerializeField] GameObject uiWinPanel;
    [SerializeField] GameObject uiLosePanel;
    [SerializeField] TextMeshProUGUI txtAlive;
    
    private void OnEnable()
    {
        // Đăng ký lắng nghe sự kiện từ EventObserver
        EventObserver.OnAliveChanged += UpdateAliveCount;
    }
    
    private void OnDestroy()
    {
        // Hủy đăng ký lắng nghe sự kiện khi đối tượng bị hủy
        EventObserver.OnAliveChanged -= UpdateAliveCount;
    }
    
    private void UpdateAliveCount(int alive)
    {
        if (txtAlive != null)
        {
            txtAlive.text = alive.ToString();
        }
    }
}
