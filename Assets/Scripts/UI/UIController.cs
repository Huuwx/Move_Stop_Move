using System;
using TMPro;
using UnityEngine;

public class UIController : MonoBehaviour
{
    [Header("Reference UI Panels")] 
    [SerializeField] private GameObject uiPanelGameComplete;
    [SerializeField] private GameObject winIcon;
    [SerializeField] private GameObject loseIcon;
    [SerializeField] private TextMeshProUGUI txtRank;
    [SerializeField] private TextMeshProUGUI txtNotify;
    [SerializeField] private GameObject joystick;
    [SerializeField] private GameObject inGameUI;
    
    [SerializeField] private Animator menuAnimator;
    [SerializeField] private GameObject inGameUIAnimator;
    
    [Header("Text Alive Count")]
    [SerializeField] TextMeshProUGUI txtAlive;

    private void Start()
    {
        if(uiPanelGameComplete != null)
        {
            uiPanelGameComplete.SetActive(false);
        }
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
        
        joystick.SetActive(false);
        if (uiPanelGameComplete != null)
        {
            uiPanelGameComplete.SetActive(true);
        }
    }
    
    private void OnClickMenu(GameState state)
    {
        if (menuAnimator != null && state == GameState.Playing)
        {
            menuAnimator.SetTrigger("Start");
            inGameUIAnimator.SetActive(true);
        }
    }
}
