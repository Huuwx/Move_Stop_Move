using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    private static GameController instance;
    public static GameController Instance {get{return instance;}}

    [Header("Refs")]
    [SerializeField] EnemySpawner spawner;          // Spawner bạn đã làm
    [SerializeField] PlayerController player;       // Player (để khóa input khi cần)
    // [SerializeField] GameObject uiReadyPanel;
    // [SerializeField] GameObject uiHudPanel;
    // [SerializeField] GameObject uiWinPanel;
    // [SerializeField] GameObject uiLosePanel;

    [Header("Game Rules")]
    [SerializeField] bool autoStart = true;

    public GameState State { get; private set; } = GameState.Boot;
    public int Alive { get; private set; }          // Enemy còn sống

    // Sự kiện cho HUD/Audio/Logic khác
    public static event Action<GameState> OnGameStateChanged;
    public static event Action<int> OnAliveChanged;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Set UI ban đầu
        // ShowOnly(uiReadyPanel, true);
        // ShowOnly(uiHudPanel,  false);
        // ShowOnly(uiWinPanel,  false);
        // ShowOnly(uiLosePanel, false);

        if (player) SetPlayerControl(false);

        // Đăng ký lắng nghe enemy chết từ Spawner/EnemyAI
        EnemyAI.OnAnyEnemyDead += HandleEnemyDead;

        // Khởi tạo map / spawner
        if (spawner)
        {
            spawner.Configure(controller: this);
        }

        SetState(GameState.Ready);

        if (autoStart) StartGame();
    }

    void OnDestroy()
    {
        EnemyAI.OnAnyEnemyDead -= HandleEnemyDead;
    }

    //========================
    // PUBLIC CONTROLS
    //========================
    public void StartGame()
    {
        if (State == GameState.Playing) return;
        SetState(GameState.Playing);

        if (player) SetPlayerControl(true);

        // ShowOnly(uiReadyPanel, false);
        // ShowOnly(uiHudPanel,  true);
        // ShowOnly(uiWinPanel,  false);
        // ShowOnly(uiLosePanel, false);
    }

    public void PauseGame()
    {
        if (State != GameState.Playing) return;
        SetState(GameState.Paused);
        Time.timeScale = 0f;
        if (player) SetPlayerControl(false);
        // Có thể mở panel pause nếu có
    }

    public void ResumeGame()
    {
        if (State != GameState.Paused) return;
        SetState(GameState.Playing);
        Time.timeScale = 1f;
        if (player) SetPlayerControl(true);
    }

    public void EndGameWin()
    {
        if (State == GameState.Win || State == GameState.Lose) return;
        SetState(GameState.Win);
        if (player) SetPlayerControl(false);
        
        Debug.Log("You Win!");
        
        // ShowOnly(uiWinPanel,  true);
        // ShowOnly(uiHudPanel,  false);
    }

    public void EndGameLose()
    {
        if (State == GameState.Win || State == GameState.Lose) return;
        SetState(GameState.Lose);
        if (player) SetPlayerControl(false);
        
        Debug.Log("You Lose!");
        
        // ShowOnly(uiLosePanel, true);
        // ShowOnly(uiHudPanel, false);
    }

    public void RestartScene()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    //========================
    // ALIVE & SPAWN TRACKING
    //========================

    // Spawner/Enemy gọi khi có 1 entity được thêm vào trận (enemy spawn hoặc player vào trận)
    public void RegisterAlive()
    {
        Alive++;
        OnAliveChanged?.Invoke(Alive);
    }

    // Spawner/Enemy gọi khi 1 entity rời trận (enemy chết, player chết)
    public void UnregisterAlive()
    {
        Alive = Mathf.Max(Alive - 1, 0);
        OnAliveChanged?.Invoke(Alive);

        // Luật thắng/thua cơ bản:
        // - Nếu player chết -> Lose
        // - Nếu Alive == 0 và player còn sống -> Win
        if (player && !player.gameObject.activeSelf)
        {
            EndGameLose();
            return;
        }

        if (Alive <= 0 && State == GameState.Playing && player && player.gameObject.activeSelf)
        {
            EndGameWin();
        }
    }

    //========================
    // INTERNALS
    //========================
    void HandleEnemyDead(EnemyAI ai)
    {
        // EnemyAI.Die() phát OnAnyEnemyDead → GameController cập nhật alive
        UnregisterAlive();

        // Yêu cầu Spawner spawn bù nếu còn quota
        if (spawner && spawner.totalSpawned < spawner.maxSpawnCount && State == GameState.Playing)
        {
            spawner.TrySpawnOneAfterDelay(0.5f);
        }
    }

    void SetState(GameState s)
    {
        State = s;
        OnGameStateChanged?.Invoke(State);
    }

    void SetPlayerControl(bool enabled)
    {
        //player.EnableControl(enabled);  // Hãy đảm bảo PlayerController có hàm này
    }

    void ShowOnly(GameObject go, bool show)
    {
        if (go) go.SetActive(show);
    }
}
